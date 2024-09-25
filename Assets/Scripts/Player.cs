using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;
using Org.BouncyCastle.Bcpg;
using UnityEngine.UIElements;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed;
    private Pan pan;
    private Rigidbody2D rigid;
    private BoxCollider2D box;
    private float horizontalInput;
    private float verticalInput;
    private Ingredient ingredient;
    private PlayerPlate playerPlate;
    public Knife knife;

    private void Awake()
    {
        // get components
        rigid = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        FollowCrosshair();

        if (Input.GetMouseButtonDown(0))
        {
            if(handsFree()) {
                if(! PickupPan()) {
                    if (! PickupPlayerPlate()) {
                        if (!PickupKnife())
                        {
                            PickupIngredient();
                        }
                    }
                }
            } else if(ingredient != null) {
                PickupIngredient();
            } else {
                PickupPan();
                PickupPlayerPlate();
                PickupKnife();
            }
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            PickupIngredient();
        }
    }

    private void FixedUpdate()
    {
        if (horizontalInput != 0 && verticalInput != 0)
        {
            horizontalInput *= 0.7f;
            verticalInput *= 0.7f;
        }
        rigid.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);

    }

    private void PickupIngredient()
    {
        // If holding a plate or pan, interact with those rather than the ingredient
        if (pan != null || playerPlate != null) { return; } // Use right click to get ingredient out of pan or plate, otherwise if it's in the pan or plate, transport it.
        // pick up closest ingredient to player cursor
        if (ingredient == null)
        {
            Ingredient[] ingredients = FindObjectsOfType<Ingredient>();
            List<Ingredient> pickupables = new();
            foreach (Ingredient i in ingredients)
            {
                float distance = Vector2.Distance(transform.position, i.transform.position);
                if (distance < 2f)
                {
                    pickupables.Add(i);
                }
            }
            if (!(pickupables.Count == 0))
            {
                Ingredient closestIngredient = null;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float lowestDistance = float.PositiveInfinity;
                foreach (Ingredient i in pickupables)
                {
                    float distance = Vector2.Distance(mousePos, i.transform.position);
                    if (distance < lowestDistance && i.stacked == false)
                    {
                        closestIngredient = i;
                        lowestDistance = distance;
                    }
                }

                if (closestIngredient != null)
                {
                    StackingBench[] stackingBenches = FindObjectsOfType<StackingBench>();
                    foreach (StackingBench sb in stackingBenches)
                    {
                        if (sb.bottomIngredient == closestIngredient)
                        {
                            sb.bottomIngredient = null;
                            break;
                        }
                    }

                    OrderAcceptor[] orderAcceptors = FindObjectsOfType<OrderAcceptor>();
                    foreach (OrderAcceptor orderAcceptor in orderAcceptors)
                    {
                        if (orderAcceptor.bottomIngredient == closestIngredient)
                        {
                            orderAcceptor.bottomIngredient = null;
                            break;
                        }
                    }

                    if (closestIngredient.pattyStove != null)
                    {
                        closestIngredient.pattyStove = null;
                    }

                    ingredient = closestIngredient;
                }
            }
            // if no existing ingredients can be found, check if user clicked on new ingredient
            else
            {
                IngredientBin[] ingredientBins = FindObjectsOfType<IngredientBin>();
                List<IngredientBin> closeBins = new();
                foreach (IngredientBin i in ingredientBins)
                {
                    float distance = Vector2.Distance(transform.position, i.transform.position);
                    if (distance < 2f) { closeBins.Add(i); }
                }
                if (!(closeBins.Count == 0))
                {
                    IngredientBin closestIngredientBin = null;
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    float lowestDistance = float.PositiveInfinity;
                    foreach (IngredientBin i in closeBins)
                    {
                        float distance = Vector2.Distance(mousePos, i.transform.position);
                        if (distance < lowestDistance)
                        {
                            closestIngredientBin = i;
                            lowestDistance = distance;
                        }
                    }
                    ingredient = closestIngredientBin.takeIngredient();
                }
            }
        }
        else
        {
            // drop ingredient
            if (ingredient.pan != null) { ingredient.pan.ingredient = null; ingredient.pan = null; return; }
            if (ingredient.playerPlate != null) { ingredient.playerPlate.ingredient = null; ingredient.playerPlate = null; return; }

            // Get the closest stacking bench
            Pan pan = FindObjectOfType<Pan>();
            StackingBench[] stackingBenches = FindObjectsOfType<StackingBench>();
            StackingBench closestBench = null;
            float closestDistance = float.PositiveInfinity;
            foreach(StackingBench sb in stackingBenches) {
                float distance = Vector2.Distance(transform.position, sb.transform.position);
                if(distance < closestDistance) {
                    closestDistance = distance;
                    closestBench = sb;
                }
            }
            
            // Drop the ingredient on the closest stacking bench if close enough
            if (closestDistance < 2f)
            {
                if (closestBench.bottomIngredient == null)
                {
                    closestBench.bottomIngredient = ingredient;
                    ingredient.transform.position = closestBench.transform.position;
                }
                else
                {
                    Ingredient temp = closestBench.bottomIngredient;
                    while (temp.next != null)
                    {
                        temp = temp.next;
                        temp.stacked = true;
                    }
                    
                    temp.next = ingredient;
                    temp.next.stacked = true;
                }
            }

            // drop pan if mouse clicked
            // if near pan put on pan
            else if (Vector2.Distance(transform.position, pan.transform.position) < 2f) {
                if (pan.ingredient == null) {
                    pan.ingredient = ingredient;
                    ingredient.pan = pan;
                    ingredient.transform.position = pan.transform.position;
                    if(pan.stove != null) {
                        if(pan.ingredient.ingredientName == Ingredient.IngredientName.patty) {
                            pan.ingredient.pattyStove = pan.stove;
                        }
                    }
                }
            }

            // Get closest plate
            PlayerPlate[] plates = FindObjectsOfType<PlayerPlate>();
            closestDistance = float.PositiveInfinity;
            PlayerPlate closestPlate = null;
            foreach (PlayerPlate plate in plates)
            {
                float distance = Vector2.Distance(transform.position, plate.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlate = plate;
                }
            }

            if (closestDistance < 2f)
            {
                if (closestPlate.ingredient == null)
                {
                    closestPlate.ingredient = ingredient;
                    ingredient.playerPlate = closestPlate;
                    ingredient.transform.position = closestPlate.transform.position;
                }
            }

            // Get closest order acceptor
            OrderAcceptor[] orderAcceptors = FindObjectsOfType<OrderAcceptor>();
            closestDistance = float.PositiveInfinity;
            OrderAcceptor closestOrderAcceptor = null;
            foreach(OrderAcceptor orderAcceptor in orderAcceptors) {
                float distance = Vector2.Distance(transform.position, orderAcceptor.transform.position);
                if(distance < closestDistance) {
                    closestDistance = distance;
                    closestOrderAcceptor = orderAcceptor;
                }
            }

            if(closestDistance < 2f) {
                if(closestOrderAcceptor.bottomIngredient == null) {
                    closestOrderAcceptor.bottomIngredient = ingredient;
                    ingredient.transform.position = closestOrderAcceptor.transform.position;
                }
            }

            ingredient = null;
        }
    }

    // pan script
    private bool PickupPan()
    {
        if (handsFree())
        {
            Pan groundPan = FindObjectOfType<Pan>();
            float distance = Vector2.Distance(transform.position, groundPan.transform.position);
            if (distance < 2f)
            {
                pan = groundPan;
                if(pan.stove != null) {
                    Stove stove = pan.stove;
                    stove.pan = null;
                    pan.stove = null;
                    if(pan.ingredient != null) {
                        if(pan.ingredient.ingredientName == Ingredient.IngredientName.patty) {
                            pan.ingredient.pattyStove = null;
                        }
                    }
                }
                return true;
            }
        }
        else
        {
            // drop pan if mouse clicked
            // if near stove put on stove
            Stove[] stoves = FindObjectsOfType<Stove>();
            foreach(Stove stove in stoves) {
                float distance = Vector2.Distance(transform.position, stove.transform.position);
                if (distance < 2f)
                {
                    pan.transform.position = stove.transform.position;
                    stove.pan = pan;
                    pan.stove = stove;

                    if (pan.ingredient != null) { 
                        pan.ingredient.transform.position = pan.transform.position; 
                        if(pan.ingredient.ingredientName == Ingredient.IngredientName.patty) {
                            pan.ingredient.pattyStove = stove;
                        }
                    }
                    break;
                }
            }
            pan = null;
        }
        return false;
    }

    // point towards crosshair
    private void FollowCrosshair()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;

        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (pan != null)
        {
            Vector3 objectPosition = transform.position + (Vector3)direction * 1.25f;
            pan.transform.position = new Vector3(objectPosition.x, objectPosition.y, 0f);

            pan.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));

            if (pan.ingredient != null)
            {
                pan.ingredient.transform.position = new Vector3(objectPosition.x, objectPosition.y, 0f);

                pan.ingredient.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
            }
        }

        if(ingredient != null) {
            Vector3 objectPosition = transform.position + (Vector3)direction * 1.25f;
            ingredient.transform.position = new Vector3(objectPosition.x, objectPosition.y, 0f);

            ingredient.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
        }

        carryPlate(direction, angle);
        carryKnife(direction, angle);
    }

    // Player Plate Behaviour
    private bool PickupPlayerPlate() {
        if (playerPlate == null)
        {
            // Look for existing plate first
            PlayerPlate plate = FindObjectOfType<PlayerPlate>();
            if (plate != null) {
                float distance = Vector2.Distance(transform.position, plate.transform.position);
                if (distance < 2f)
                {
                    playerPlate = plate;
                    return true;
                }
            }
            
            StackOfPlates stack = FindObjectOfType<StackOfPlates>();
            float stackDistance = Vector2.Distance(transform.position, stack.transform.position);
            if (stackDistance < 2f)
            {
                playerPlate = stack.takePlayerPlate(); return true;
            }
            return false;
        }
        else { dropPlayerPlate(); return false; }
    }

    private void dropPlayerPlate() {
        playerPlate = null;
    }
    
    public void carryPlate(Vector2 direction, float angle) {
        if (playerPlate != null)
        {
            Vector3 objectPosition = transform.position + (Vector3)direction * 1.25f;
            playerPlate.transform.position = new Vector3(objectPosition.x, objectPosition.y, 0f);
            playerPlate.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));

            if (playerPlate.ingredient != null) {
                playerPlate.ingredient.transform.position = new Vector3(objectPosition.x, objectPosition.y, 0f);
                playerPlate.ingredient.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
            }
        }
    }


    // Knife behaviour
    private bool PickupKnife() 
    {
        if (knife != null) {
            bool sliced = slice(); // try to cut ingredient
            if (!sliced) {
                knife = null; // if nothing to cut, drop knife
                return true;
            }
            return sliced;
        }

        // Look for the closest knife to pick up
        Knife[] knives = FindObjectsOfType<Knife>();
        List<Knife> pickupables = new();
        foreach (Knife i in knives)
        {
            float distance = Vector2.Distance(transform.position, i.transform.position);
            if (distance < 2f) { pickupables.Add(i); }
        }
        if (!(pickupables.Count == 0)) {
            Knife closestKnife = null;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float lowestDistance = float.PositiveInfinity;
            foreach (Knife i in pickupables)
            {
                float distance = Vector2.Distance(mousePos, i.transform.position);
                if (distance < lowestDistance) {
                    closestKnife = i;
                    lowestDistance = distance;
                }
            }

            knife = closestKnife;
            return true;
        }
        return false;
     
    }

    // If player is holding
    private bool slice()
    {
        Debug.LogWarning("slice() called");
        // Look for the closest stacking/cutting bench to pick up
        StackingBench[] stackingBenches = FindObjectsOfType<StackingBench>();
        List<StackingBench> pickupables = new();
        foreach (StackingBench i in stackingBenches)
        {
            float distance = Vector2.Distance(transform.position, i.transform.position);
            if (distance < 2f) { pickupables.Add(i); }
        }
        if (!(pickupables.Count == 0))
        {
            StackingBench closestStackingBench = null;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float lowestDistance = float.PositiveInfinity;
            foreach (StackingBench i in pickupables)
            {
                float distance = Vector2.Distance(mousePos, i.transform.position);
                if (distance < lowestDistance)
                {
                    closestStackingBench = i;
                    lowestDistance = distance;
                }
            }

            return closestStackingBench.slice(); // return true if ingredient is successfully sliced
        }
        return false; // return false, no stacking benches found
    }

    public void carryKnife(Vector2 direction, float angle)
    {
        if (knife == null) { return; }
            Vector3 objectPosition = transform.position + (Vector3)direction * 1.25f;
            knife.transform.position = new Vector3(objectPosition.x, objectPosition.y, 0f);
            knife.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-30));
    }


    // Return true if player isn't holding anything
    public bool handsFree() {
        if (pan != null) { return false; }
        if (ingredient != null) { return false; }
        if (playerPlate != null) { return false; }
        if (knife != null) { return false; }
        return true;
    }
}



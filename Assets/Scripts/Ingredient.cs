using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] public Ingredient next;
    [SerializeField] public IngredientName ingredientName;
    [SerializeField] public Plate plate;
    public Stove pattyStove;
    private SpriteRenderer spriteRenderer;
    public int cookedLevel = 0;
    public bool stacked = false;
    public Pan pan;
    public PlayerPlate playerPlate;
    private Vector3 originalScale;
    private Color originalColor;
    private Sprite originalSprite;
    [SerializeField] private Sprite cookedPatty;
    public Ingredient slicedVersion;

    public enum IngredientName {
        patty,
        lettuce,
        tomato,
        cheese,
        topbun,
        bottombun,
        wholelettuce,
        wholetomato
    }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;
        originalSprite = this.spriteRenderer.sprite;
    }

    private void Update() {
        if(next != null) {
            next.transform.position = this.transform.position;
        }
        if(ingredientName == IngredientName.patty && pattyStove != null) {
            cookedLevel++;

            if(cookedLevel < 8000) {
                // every 2000 frames, the patty is cooked a bit more
                if(cookedLevel % 2000 == 0) {
                    Color currentColor = spriteRenderer.color;
                    Color darkenedColor = new Color(
                        currentColor.r * 0.9f, // Reduce red by 10%
                        currentColor.g * 0.9f, // Reduce green by 10%
                        currentColor.b * 0.9f, // Reduce blue by 10%
                        currentColor.a); 
                        spriteRenderer.color = darkenedColor;
                }
            } else {
                spriteRenderer.sprite = cookedPatty;
                this.transform.localScale = new Vector3(0.0380598f, 0.0380598f, 0.0380598f);
                if(cookedLevel % 2000 == 0) {
                    Color currentColor = spriteRenderer.color;
                    Color darkenedColor = new Color(
                    currentColor.r * 0.9f, // Reduce red by 10%
                    currentColor.g * 0.9f, // Reduce green by 10%
                    currentColor.b * 0.9f, // Reduce blue by 10%
                    currentColor.a); 
                    spriteRenderer.color = darkenedColor;
                }
            }
        }
    }

    // Return true if patty is cooked, and not too overcooked
    // Otherwise return false
    public bool Cooked() {
        if(ingredientName != IngredientName.patty) {
            return false;
        }
        if(cookedLevel > 8000 && cookedLevel < 14000) {
            return true;
        }
        return false;
    }



    public void Reset() {
        stacked = false;
        pan = null;
        pattyStove = null;
        next = null;
        cookedLevel = 0;
        this.transform.position = plate.transform.position;
        this.transform.rotation = Quaternion.identity;
        spriteRenderer.color = originalColor;
        this.transform.localScale = originalScale;
        this.spriteRenderer.sprite = originalSprite;
    }
}

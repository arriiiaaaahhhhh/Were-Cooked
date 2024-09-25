using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAcceptor : MonoBehaviour
{
    public static int orderNumber = 1;
    private List<Order> orders = new List<Order>();
    public GameObject prefab;
    private int ordersOnScreen = -1;
    public Ingredient bottomIngredient;
    private float elapsedTime = 0f;
    public float beltSpeed = 1.0f;

    public void Awake() {
        int numberOfOrders = UnityEngine.Random.Range(1, 4);
        for(int i = 0; i < numberOfOrders; i++) {
            ordersOnScreen++;
            Order order = Instantiate(prefab).GetComponent<Order>();
            order.xOffset = (ordersOnScreen * 210) + 10;
            orders.Add(order);
            orderNumber++;
        }
    }

    public void Update() {
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= 1f) {
            for(int i = 0; i < orders.Count; i++) {
                Order order = orders[i];
                order.timer--;
                if(order.timer <= 0) {
                    orders.Remove(order);
                    Destroy(order.gameObject);
                    Destroy(order);
                    Order newOrder = Instantiate(prefab).GetComponent<Order>(); 
                    orders.Insert(i, newOrder);
                    newOrder.xOffset = (i * 210) + 10;
                    orderNumber++;
                }
            }

            elapsedTime = 0f;
        }

        // Sliding on the conveyor belt
        if(bottomIngredient != null) {
            if(bottomIngredient.transform.position.y >= 0f && bottomIngredient.transform.position.y <= 2.5f) { 
                bottomIngredient.transform.position += Vector3.up * beltSpeed * Time.deltaTime;
                CheckOrders();
            } else if(bottomIngredient.transform.position.y >= 2.5f) {
                List<Ingredient> resets = new List<Ingredient>();
                resets.Add(bottomIngredient);
                Ingredient current = bottomIngredient;
                while(current != null) {
                    resets.Add(current);
                    current = current.next;
                }
                foreach(Ingredient ingredient in resets) {
                    Destroy(ingredient.gameObject);
                    Destroy(ingredient);
                }
                bottomIngredient = null;
            } else {
                bottomIngredient.transform.position += Vector3.up * beltSpeed * Time.deltaTime;
            }
        }
        
    }

    public void CheckOrders() {
        if(bottomIngredient != null && bottomIngredient.ingredientName == Ingredient.IngredientName.bottombun && bottomIngredient.next !=null) {
            for(int i = 0; i < orders.Count; i++) {
                Order order = orders[i];
                List<String> ingredients = order.ingredients;
                List<Ingredient> resets = new List<Ingredient>();
                resets.Add(bottomIngredient);
                Ingredient current = bottomIngredient.next;
                Boolean accepted = true;
                foreach(String s in ingredients) {
                    // avoid null reference
                    if(current.next == null) {
                        accepted = false;
                        break;
                    }

                    // do not accept if wrong ingredient
                    if(current.ingredientName.ToString() != s) {
                        accepted = false;
                        break;
                    }
                    
                    // do not accept if patty is under or overcooked
                    if(current.ingredientName == Ingredient.IngredientName.patty) {
                        if(! current.Cooked()) {
                            accepted = false;
                            break;
                        }
                    }
                    resets.Add(current);
                    current = current.next;
                }

                if(accepted) {
                    // order accepted
                    Destroy(order.gameObject);
                    Destroy(order);
                    Order newOrder = Instantiate(prefab).GetComponent<Order>();
                    orders.Remove(order);
                    orders.Insert(i, newOrder);
                    newOrder.xOffset = (i * 210) + 10;
                    orderNumber++;
                    foreach(Ingredient ingredient in resets) {
                        Destroy(ingredient.gameObject);
                        Destroy(ingredient);
                    }
                    bottomIngredient = null;
                    GameController.addScore();
                    return;
                }
            }

        }
    }
}

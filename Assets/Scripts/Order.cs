using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public List<String> ingredients;
    private Texture2D grayTexture;
    private int orderNumber = OrderAcceptor.orderNumber;
    private GUIStyle customStyle;
    public int xOffset;
    public int timer;

    public void Start() {
        grayTexture = new Texture2D(1, 1);
        grayTexture.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 0.5f)); // RGB(128, 128, 128) with 50% opacity
        grayTexture.Apply();
        customStyle = new GUIStyle();
        customStyle.normal.textColor = Color.black;
        customStyle.fontStyle = FontStyle.Bold;
        timer = UnityEngine.Random.Range(30, 61);
        ingredients = new List<string>();

        int numberOfIngredients = UnityEngine.Random.Range(1, 6);
        for(int i = 0; i < numberOfIngredients; i++) {
            int ingredientNum = UnityEngine.Random.Range(0,4);
            Ingredient.IngredientName ingredientName = (Ingredient.IngredientName)ingredientNum;
            String ingredientString = ingredientName.ToString();
            ingredients.Add(ingredientString);
        }
    }

    void OnGUI()
    {
        // Define a position and size for the label
        Rect labelRect = new Rect(xOffset, 400, 200, 300);
        
        // Display a label with text on the screen
        GUI.DrawTexture(labelRect, grayTexture);
        String orderText = "Order " + orderNumber + "                  Time left: " + timer;
        orderText += "\nBurger with the\nfollowing ingredients:";
        foreach(String s in ingredients) {
            orderText += "\n" + s;
        }
        GUI.Label(labelRect, orderText, customStyle);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBin : MonoBehaviour
{
    public Ingredient ingredient;
    void Start()
    {
        
    }
    public Ingredient takeIngredient()
    {
        return Instantiate(ingredient);
    }
}

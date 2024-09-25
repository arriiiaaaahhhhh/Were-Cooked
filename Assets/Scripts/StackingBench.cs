using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ingredient;

public class StackingBench : MonoBehaviour
{
    public Ingredient bottomIngredient;

    // Check if the ingredient can be cut
    public bool slice()
    {
        // Check if there's an ingredient available to cut
        Debug.Log("test1");
        if (bottomIngredient == null) { return false; }

        // Check ingredient is cuttable
        if (bottomIngredient.slicedVersion == null) { return false; }

        // check ingredients are not stacked
        //if (bottomIngredient.stacked == true) { return; }

        Debug.Log("test");

        Ingredient slicedVersion = Instantiate(bottomIngredient.slicedVersion);
        slicedVersion.transform.position = this.transform.position;
        Destroy(bottomIngredient.gameObject);
        bottomIngredient = slicedVersion;

        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class StackOfPlates : MonoBehaviour
{
    public PlayerPlate PlayerPlate; // Assign the plate prefab in the Inspector

    void Start()
    {
    }
    public PlayerPlate takePlayerPlate() {
        return Instantiate(PlayerPlate);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] LayerMask npcLayer; // Layer for NPCs
    [SerializeField] LayerMask solidObjectLayer;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] LayerMask floorLayer;

    public static GameLayer i { get; set; }

    public void Awake()
    {
        i = this;
    }
    public LayerMask SolidLayer { get => solidObjectLayer; }
    public LayerMask InteractLayer {  get => interactLayer; }
    public LayerMask FloorLayer { get => floorLayer; }

    public bool IsInLayerMask(LayerMask layerMask, int layer)
    {
        return (layerMask & (1 << layer)) != 0;
    }



}

//this is an check from the gpt:
/**
 * using UnityEngine;

public class GameLayer : MonoBehaviour
{
    public LayerMask npcLayer; // Layer for NPCs
    public LayerMask environmentLayer; // Layer for environment objects
    public LayerMask interactableLayer; // Layer for interactable objects
    public LayerMask floorLayer; // Layer for floor

    private void Start()
    {
        // Example usage of layer masks
        // Check if a layer belongs to the NPC layer
        if (IsLayerInMask(gameObject.layer, npcLayer))
        {
            Debug.Log("This object is in the NPC layer.");
        }
    }

    public bool IsLayerInMask(int layer, LayerMask mask)
    {
        return (mask & (1 << layer)) != 0;
    }

    public int GetLayerMask(LayerType layerType)
    {
        switch (layerType)
        {
            case LayerType.NPC:
                return npcLayer;
            case LayerType.Environment:
                return environmentLayer;
            case LayerType.Interactable:
                return interactableLayer;
            case LayerType.Floor:
                return floorLayer;
            default:
                return 0;
        }
    }

    public enum LayerType
    {
        NPC,
        Environment,
        Interactable,
        Floor
    }
}

 */
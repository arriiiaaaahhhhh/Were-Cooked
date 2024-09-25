using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject npcPrefab; // Assign a prefab with the PlayerControllers and Npc scripts attached
    public Transform[] customerSeats; // Assign seats for customers
    public Transform[] waiterStations; // Assign stations for waiters
    public Transform doorPosition; // Assign the front door position
    public int numberOfCustomers = 12; // Number of customers to spawn
    public int numberOfWaiters = 6; // Number of waiters to spawn

    private void Start()
    {
        StartCoroutine(InitializeScene());
    }

    private IEnumerator InitializeScene()
    {
        // Spawn and initialize waiters
        for (int i = 0; i < numberOfWaiters; i++)
        {
            SpawnNpc(Npc.NpcType.Waiter);
        }

        // Wait before spawning customers
        yield return new WaitForSeconds(2f);

        // Spawn and initialize customers
        for (int i = 0; i < numberOfCustomers; i++)
        {
            SpawnNpc(Npc.NpcType.Customer);
        }
    }

    private void SpawnNpc(Npc.NpcType npcType)
    {
        GameObject npc = Instantiate(npcPrefab, GetSpawnPosition(npcType), Quaternion.identity);
        PlayerControllers playerController = npc.GetComponent<PlayerControllers>();
        if (playerController != null)
        {
            playerController.SetNPCStatus(true);
            Npc npcScript = npc.GetComponent<Npc>();
            if (npcScript != null)
            {
                npcScript.npcType = npcType;
                npcScript.customerSeats = new List<Transform>(customerSeats);
                npcScript.waiterStations = new List<Transform>(waiterStations);
                npcScript.doorPosition = doorPosition;
            }
        }
    }

    private Vector3 GetSpawnPosition(Npc.NpcType npcType)
    {
        // Determine spawn position based on NPC type
        if (npcType == Npc.NpcType.Waiter)
        {
            // Randomly choose a position for waiters
            return waiterStations[Random.Range(0, waiterStations.Length)].position;
        }
        else
        {
            // Spawn customers at the door
            return doorPosition.position;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public enum NpcType { Customer, Waiter }

    public NpcType npcType; // Set this in the Inspector or via script
    public List<Transform> customerSeats; // List of chair positions for customers
    public List<Transform> waiterStations; // List of station positions for waiters
    public Transform doorPosition; // Position of the front door

    private PlayerControllers playerController;
    private static int seatedCustomerCount = 0; // Static counter for seated customers

    private void Start()
    {
        playerController = GetComponent<PlayerControllers>();
        playerController.SetNPCStatus(true);

        if (npcType == NpcType.Waiter)
        {
            HandleWaiterInitialization();
        }
        else if (npcType == NpcType.Customer)
        {
            StartCoroutine(HandleCustomerArrival());
        }
    }

    private void HandleWaiterInitialization()
    {
        if (waiterStations.Count > 0)
        {
            if (waiterStations.Count >= 6)
            {
                // 2 waiters at counters
                StartCoroutine(MoveToTarget(waiterStations[0].position));
                StartCoroutine(MoveToTarget(waiterStations[1].position));

                // 1 waiter at the door
                StartCoroutine(MoveToTarget(doorPosition.position));

                // 3 waiters waiting near counters
                StartCoroutine(MoveToTarget(waiterStations[2].position));
                StartCoroutine(MoveToTarget(waiterStations[3].position));
                StartCoroutine(MoveToTarget(waiterStations[4].position));
            }
            else
            {
                Debug.LogWarning("Not enough waiter stations set up!");
            }
        }
    }

    private IEnumerator HandleCustomerArrival()
    {
        // Wait for a short duration before customers start arriving
        yield return new WaitForSeconds(2f);

        // Move customers to random seats
        for (int i = 0; i < 12; i++) // Assuming you want 12 customers
        {
            if (customerSeats.Count > 0)
            {
                Transform targetSeat = customerSeats[Random.Range(0, customerSeats.Count)];
                StartCoroutine(MoveToTarget(targetSeat.position));
                seatedCustomerCount++;
            }
            yield return new WaitForSeconds(1f); // Stagger customer arrivals
        }

        // After seating 12 customers, activate the waiting waiters
        yield return new WaitUntil(() => seatedCustomerCount >= 12);

        StartCoroutine(ActivateWaiters());
    }

    private IEnumerator ActivateWaiters()
    {
        // Logic to move waiters (you might need to reference specific waiters in your game)
        // For simplicity, this example assumes the NPC script for waiters is also handling movement
        foreach (Transform station in waiterStations)
        {
            StartCoroutine(MoveToTarget(station.position));
            yield return new WaitForSeconds(1f); // Delay between waiters starting
        }
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            playerController.Move(direction);
            yield return null;
        }

        // Stop moving once the target is reached
        playerController.Move(Vector3.zero);
    }

    private void HandleError(string message)
    {
        Debug.LogError(message);
    }

}

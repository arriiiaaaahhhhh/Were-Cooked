using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllers : MonoBehaviour
{
    public float moveSpeed = 5f;


    private bool isMoving;
    private Vector2 input;

    private bool isNPC; // Flag to check if the character is an NPC
    private float npcMoveCooldown = 2f; // Time between random movements for NPCs
    private float npcMoveTimer;

    private Animator animator; // Animator for handling animations (if you have one)

    public Animator Animator { get => animator; }
    public void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found! Animations won't be handled.");
        }
    }

    public void SetNPCStatus(bool isNPC)
    {
        this.isNPC = isNPC;
    }

    public  void Update()
    {
        if (isNPC)
        {
            HandleNPCMovement();
        }
        else
        {
            HandlePlayerInput();
        }
    }

    public void HandlePlayerInput()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Remove diagonal movement
            if (input.x != 0) { input.y = 0; }

            if (input != Vector2.zero)
            {
                StartCoroutine(Move(Vector3.zero));
            }
        }
    }

    public  void HandleNPCMovement()
    {
        if (!isMoving)
        {
            npcMoveTimer -= Time.deltaTime;
            if (npcMoveTimer <= 0f)
            {
                // Generate random direction
                input.x = UnityEngine.Random.Range(-1, 2); // Random value between -1 and 1
                input.y = UnityEngine.Random.Range(-1, 2);

                // Remove diagonal movement
                if (input.x != 0) { input.y = 0; }

                if (input != Vector2.zero)
                {
                    StartCoroutine(Move(Vector3.zero));
                }

                npcMoveTimer = npcMoveCooldown;
            }
        }
    }

    public IEnumerator Move(Vector3 direction)
    {
        isMoving = true;

        Vector3 targetPos = transform.position;
        targetPos.x += input.x;
        targetPos.y += input.y;

        if (IsWalkable(targetPos))
        {
            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;

            if (animator != null)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                animator.SetBool("isMoving", true);
            }
        }

        isMoving = false;

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, GameLayer.i.SolidLayer | GameLayer.i.InteractLayer) != null)
        {
            return false;
        }

        return true;
    }
}

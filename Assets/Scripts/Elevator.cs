using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode; // Import Netcode for GameObjects

public class Elevator : NetworkBehaviour
{
    public float targetHeight = 5f; // Height the elevator will move to
    public float speed = 10f; // Increased speed to make the elevator move faster
    private Vector3 originalPosition;
    private Rigidbody rb; // Reference to the Rigidbody component
    private BoxCollider boxCollider; // Reference to the BoxCollider component

    // Network variables to synchronize movement state
    private NetworkVariable<bool> isMovingUp = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isMovingDown = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        originalPosition = transform.position; // Store the original position of the elevator
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        boxCollider = GetComponent<BoxCollider>(); // Get the BoxCollider component

        // Freeze the Y position initially
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        // Subscribe to changes in the network variables
        isMovingUp.OnValueChanged += OnIsMovingUpChanged;
        isMovingDown.OnValueChanged += OnIsMovingDownChanged;
    }

    void Update()
    {
        if (isMovingUp.Value)
        {
            UnlockYPosition(); // Unlock Y position when moving up
            SetColliderTrigger(false); // Disable trigger while moving
            MoveUp();
        }
        else if (isMovingDown.Value)
        {
            UnlockYPosition(); // Unlock Y position when moving down
            SetColliderTrigger(false); // Disable trigger while moving
            MoveDown();
        }
        else if (transform.position.y <= originalPosition.y)
        {
            FreezeYPosition(); // Freeze Y position when idle
            SetColliderTrigger(true); 
        }
    }

    public void ActivateElevator()
    {
        if (!isMovingUp.Value && !isMovingDown.Value)
        {
            StartCoroutine(ElevatorSequence());
        }
    }

    private IEnumerator ElevatorSequence()
    {
        isMovingUp.Value = true;
        yield return new WaitUntil(() => Mathf.Abs(transform.position.y - (originalPosition.y + targetHeight)) < 0.01f);
        isMovingUp.Value = false;

        yield return new WaitForSeconds(2f);

        isMovingDown.Value = true;
        yield return new WaitUntil(() => Mathf.Abs(transform.position.y - originalPosition.y) < 0.01f);
        isMovingDown.Value = false;
    }

    private void MoveUp()
    {
        Vector3 targetPosition = new Vector3(originalPosition.x, originalPosition.y + targetHeight, originalPosition.z);

        // Calculate the direction to move
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Apply force to the Rigidbody
        rb.velocity = direction * speed;

        // Stop the elevator when it reaches the target position
        if (Mathf.Abs(transform.position.y - targetPosition.y) < 0.01f)
        {
            rb.velocity = Vector3.zero;
            FreezeYPosition(); // Freeze Y position when the elevator stops
        }
    }

    private void MoveDown()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;

        // Apply force to the Rigidbody
        rb.velocity = direction * speed;

        // Stop the elevator when it reaches the original position
        if (Mathf.Abs(transform.position.y - originalPosition.y) < 0.01f)
        {
            rb.velocity = Vector3.zero;
            FreezeYPosition(); // Freeze Y position when the elevator stops
        }
    }

    private void FreezeYPosition()
    {
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    private void UnlockYPosition()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation; // Unlock Y position while keeping rotation frozen
    }

    private void SetColliderTrigger(bool isTrigger)
    {
        if (boxCollider != null)
        {
            boxCollider.isTrigger = isTrigger;
        }
    }

    private void OnIsMovingUpChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"isMovingUp changed from {previousValue} to {newValue}");
    }

    private void OnIsMovingDownChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"isMovingDown changed from {previousValue} to {newValue}");
    }
}
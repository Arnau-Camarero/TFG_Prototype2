using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ExitBlock : NetworkBehaviour
{
    public float targetY = 2f; // Target Y position
    public float speed = 2f;  // Speed of movement

    private bool moveDown = false;

    void Update()
    {
        if (moveDown && transform.position.y > targetY)
        {
            float newY = Mathf.Max(transform.position.y - speed * Time.deltaTime, targetY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            if (IsServer)
            {
                UpdatePositionClientRpc(transform.position);
            }
        }
    }

    public void StartMovingDown()
    {
        moveDown = true;
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}

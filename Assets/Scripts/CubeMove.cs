using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CubeMove : NetworkBehaviour
{
    private List<GameObject> playersColliding = new List<GameObject>();
    public float moveForce = 10f;
    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = true; // Allow clients to send data to the server
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the Y position is higher than 2.9
        if (transform.position.y > 2.9f)
        {
            // Unfreeze X rotation
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Collision detected with {col.gameObject.name}");
            NotifyCollisionServerRpc(col.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Collision ended with {col.gameObject.name}");
            NotifyCollisionExitServerRpc(col.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void NotifyCollisionServerRpc(ulong playerNetworkObjectId)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
        if (playerNetworkObject != null)
        {
            GameObject player = playerNetworkObject.gameObject;
            if (!playersColliding.Contains(player))
            {
                playersColliding.Add(player);
            }

            CheckPlayersCollision();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void NotifyCollisionExitServerRpc(ulong playerNetworkObjectId)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
        if (playerNetworkObject != null)
        {
            GameObject player = playerNetworkObject.gameObject;
            if (playersColliding.Contains(player))
            {
                playersColliding.Remove(player);
            }
        }
    }

    void CheckPlayersCollision()
    {
        if (playersColliding.Count > 0)
        {
            Vector3 combinedDirection = Vector3.zero;

            foreach (GameObject player in playersColliding)
            {
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    combinedDirection += (transform.position - player.transform.position).normalized;
                    Debug.Log($"Player {player.name} contributing direction: {(transform.position - player.transform.position).normalized}");
                }
            }

            combinedDirection.Normalize();
            Debug.Log($"Final combined direction: {combinedDirection}");

            rb.AddForce(-combinedDirection * moveForce, ForceMode.Impulse);
            Debug.Log($"Applying force: {-combinedDirection * moveForce}");

            UpdateCubePositionClientRpc(transform.position, rb.velocity);
        }
    }

    [ClientRpc]
    void UpdateCubePositionClientRpc(Vector3 position, Vector3 velocity)
    {
        Debug.Log($"Updating cube position to {position} and velocity to {velocity}");
        if (!IsOwner)
        {
            rb.position = position;
            rb.velocity = velocity;
        }
    }
}

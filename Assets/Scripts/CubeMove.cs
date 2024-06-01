using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CubeMove : NetworkBehaviour
{
    private List<GameObject> playersColliding = new List<GameObject>();
    public float moveForce = 10f;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (!playersColliding.Contains(col.gameObject))
            {
                playersColliding.Add(col.gameObject);
            }
            CheckPlayersCollision();
        }
        if(col.CompareTag("Door")){
            GameObject door = GameObject.FindGameObjectWithTag("Door");
            door.GetComponent<DoorBehaviour>();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (playersColliding.Contains(col.gameObject))
            {
                playersColliding.Remove(col.gameObject);
            }
        }
    }

    void CheckPlayersCollision()
    {
        if (playersColliding.Count == 2)
        {
            foreach (GameObject player in playersColliding)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = rb.velocity.normalized;
                    rb.AddForce(direction * moveForce, ForceMode.Impulse);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyBehaviour : NetworkBehaviour
{
    // El cuerpo del enemigo
    private Rigidbody rb;
    

    // Objeto del Jugador
    private GameObject closestPlayer;
    private GameObject[] players;
    public Spawner spawner;
    
    // Velocidad del enemigo
    public float speed;

    // Tag del Jugador
    public string playerTag = "Player";

    public override void OnNetworkSpawn(){
        if(!IsServer){
            enabled = false;
            //GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        players = GameObject.FindGameObjectsWithTag(playerTag);
    }

    void Update()
    {

        float closestDistance = Mathf.Infinity;
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }
        // Dirección enemigo
        Vector3 direction = (closestPlayer.transform.position - transform.position).normalized * speed * Time.deltaTime;
        
        rb.MovePosition(transform.position + direction);

    }

    public override void OnDestroy(){
        spawner.enemyCounter.Remove(this.gameObject);
    }
}

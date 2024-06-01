using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    // Prefab to spawn
    public GameObject objectToSpawn;

    // Spawn interval in seconds
    public float spawnInterval = 2f;
    public int maxEnemies = 10;
    public List<GameObject> enemyCounter;

    // Minimum and maximum values for random x-coordinate offset
    public float minZOffset = -5.0f;
    public float maxZOffset = 5.0f;

    public override void OnNetworkSpawn(){
        if(!IsOwner){
            enabled = false;
            return;
        }

        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if( enemyCounter.Count <= maxEnemies){
            // Get the current position of the spawner object
            Vector3 spawnPosition = transform.position;

            // Generate a random z-coordinate offset within the specified range
            float randomZOffset = Random.Range(minZOffset, maxZOffset);

            // Apply the random offset only to the z-coordinate
            spawnPosition.z += randomZOffset;

            // Spawn the prefab at the modified spawn position with the spawner's rotation
            GameObject enemy = Instantiate(objectToSpawn, spawnPosition, transform.rotation, transform);

            enemy.GetComponent<EnemyBehaviour>().spawner = this;
            enemy.GetComponent<NetworkObject>().Spawn(true);

            enemyCounter.Add(enemy);
        }
    }
}

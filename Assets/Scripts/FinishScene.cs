using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FinishScene : NetworkBehaviour
{

    GameObject player1;
    GameObject player2;
    GameObject sceneManager;
    GameObject[] players;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {   
        if(!IsServer){
            enabled = false;
            return;
        }

        sceneManager = GameObject.FindGameObjectWithTag("Scene");
        players = GameObject.FindGameObjectsWithTag("Player");

        player1 = players[0];
        player2 = players[1];
    }

    // Update is called once per frame
    void Update()
    {
        if(players == null && (player1 == null || player2 == null)){
            players = GameObject.FindGameObjectsWithTag("Player");
            player1 = players[0];
            player2 = players[1];
        }else{
            if(player1.GetComponent<PlayerMovement>().isFinished && player2.GetComponent<PlayerMovement>().isFinished){
                sceneManager.GetComponent<SceneManagement>().ChangeSceneServerRpc("Game2");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public override void OnNetworkSpawn(){
        if(!IsOwner){
            enabled = false;
            return;
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Weapon" || col.gameObject.tag == "Door"){
            return;
        }
        if(col.gameObject.tag == "Enemy"){
            Destroy(col.gameObject);
        }
        Destroy(this.gameObject);
    }
}

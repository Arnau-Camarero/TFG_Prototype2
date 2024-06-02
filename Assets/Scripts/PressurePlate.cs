using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PressurePlate : NetworkBehaviour
{
    public bool isPressed = false;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner){
            enabled = false;
            return;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Prop")
        {
            PlatePressedRpc(true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Prop")
        {
            PlatePressedRpc(false);
        }
    }

    [Rpc(SendTo.Everyone)]
    void PlatePressedRpc(bool pressed)
    {
        isPressed = pressed;
    }
}

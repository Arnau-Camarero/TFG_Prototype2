using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PressurePlateElevator : NetworkBehaviour
{
    public bool isPressed = false;
    public GameObject exitBlock;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Prop")
        {
            Debug.Log("Plate Pressed: " + col.gameObject.name);
            PlatePressedServerRpc(true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Prop")
        {
            Debug.Log("Plate Released: " + col.gameObject.name);
            PlatePressedServerRpc(false);
        }
    }

    [ServerRpc]
    void PlatePressedServerRpc(bool pressed)
    {
        Debug.Log("Plate Pressed: " + pressed);
        isPressed = pressed;

        if (pressed)
        {
            Debug.Log("Activating exit block movement");
            exitBlock.GetComponent<ExitBlock>().StartMovingDown(); // Trigger ExitBlock movement
            NotifyClientsToMoveBlockClientRpc();
        }
    }

    [ClientRpc]
    void NotifyClientsToMoveBlockClientRpc()
    {
        if (!IsServer)
        {
            exitBlock.GetComponent<ExitBlock>().StartMovingDown(); // Trigger movement on clients
        }
    }
}

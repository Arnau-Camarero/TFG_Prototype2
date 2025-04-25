using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ButtonManager : NetworkBehaviour
{
    [SerializeField] private GameObject elevator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsServer)
            {
                if (elevator != null)
                {
                    Debug.Log("Button pressed. Activating elevator.");
                    ActivateElevatorServerRpc();
                }
                else
                {
                    Debug.LogError("Elevator reference is null in ButtonManager.");
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActivateElevatorServerRpc()
    {
        if (elevator != null)
        {
            elevator.GetComponent<Elevator>().ActivateElevator();
            NotifyClientsElevatorActivatedClientRpc();
        }
    }

    [ClientRpc]
    private void NotifyClientsElevatorActivatedClientRpc()
    {
        if (elevator != null)
        {
            Debug.Log("Elevator activated on client.");
            elevator.GetComponent<Elevator>().ActivateElevator();
        }
    }
}
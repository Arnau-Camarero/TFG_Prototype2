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
                    elevator.GetComponent<Elevator>().ActivateElevator();
                }
                else
                {
                    Debug.LogError("Elevator reference is null in ButtonManager.");
                }
            }
        }
    }
}
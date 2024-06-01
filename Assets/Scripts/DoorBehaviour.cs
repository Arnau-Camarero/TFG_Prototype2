using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorBehaviour : NetworkBehaviour
{
    public PressurePlate pressurePlate1;
    public PressurePlate pressurePlate2;
    public GameObject doorRight;
    public GameObject doorLeft;

    private Vector3 doorLeftInitPos = new Vector3(-0.9f, 0.9f, 6.856f);
    private Vector3 doorRightInitPos = new Vector3(0.9f, 0.9f, 6.856f);
    private float distanceToTravel;
    private float targetDoorRight;
    private float targetDoorLeft;

    private NetworkVariable<bool> platesPressed = new NetworkVariable<bool>(false);
    private NetworkVariable<Vector3> doorLeftPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> doorRightPosition = new NetworkVariable<Vector3>();

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        doorLeft.transform.position = doorLeftInitPos;
        doorRight.transform.position = doorRightInitPos;

        distanceToTravel = Mathf.Abs(doorRight.transform.position.x * doorRight.transform.localScale.x);
        targetDoorRight = doorRight.transform.position.x + distanceToTravel;
        targetDoorLeft = doorLeft.transform.position.x - distanceToTravel;
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            bool arePlatesPressed = pressurePlate1.isPressed && pressurePlate2.isPressed;
            if (arePlatesPressed != platesPressed.Value)
            {
                Debug.Log("IN RPC: ");
                Debug.Log(arePlatesPressed);
                platesPressed.Value = arePlatesPressed;
                //SetDoorsPositionServerRpc(platesPressed.Value);
            }

            if (platesPressed.Value)
            {
                Debug.Log("MOVINGDOORS");
                //MoveDoors();
                SetDoorsPositionServerRpc(platesPressed.Value);
            }
        }
        else
        {
            doorLeft.transform.position = doorLeftPosition.Value;
            doorRight.transform.position = doorRightPosition.Value;
        }
    }

    private void MoveDoors()
    {
        if (doorLeft.transform.position.x >= targetDoorLeft)
        {
            Vector3 newPosLeft = new Vector3(doorLeft.transform.position.x - Time.deltaTime, doorLeft.transform.position.y, doorLeft.transform.position.z);
            Vector3 newPosRight = new Vector3(doorRight.transform.position.x + Time.deltaTime, doorRight.transform.position.y, doorRight.transform.position.z);
            doorLeft.transform.position = newPosLeft;
            doorRight.transform.position = newPosRight;

            doorLeftPosition.Value = newPosLeft;
            doorRightPosition.Value = newPosRight;
        }
    }

    [ServerRpc]
    private void SetDoorsPositionServerRpc(bool platesPressed)
    {
        if (platesPressed)
        {
            MoveDoors();
        }
    }
}

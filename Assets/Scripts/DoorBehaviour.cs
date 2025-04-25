using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorBehaviour : MonoBehaviour
{
    public PressurePlate pressurePlatePlayer;
    public PressurePlateElevator pressurePlateCube;
    public GameObject doorRight;
    public GameObject doorLeft;

    private Vector3 doorLeftInitPos = new Vector3(-0.9f, 0.9f, 6.856f);
    private Vector3 doorRightInitPos = new Vector3(0.9f, 0.9f, 6.856f);
    private float distanceToTravel;
    private float targetDoorRight;
    private float targetDoorLeft;

    void Start(){

        pressurePlatePlayer = GameObject.FindGameObjectWithTag("Plate").GetComponent<PressurePlate>();
        pressurePlateCube = GameObject.FindGameObjectWithTag("PlateCube").GetComponent<PressurePlateElevator>();

        doorLeft.transform.position = doorLeftInitPos;
        doorRight.transform.position = doorRightInitPos;

        distanceToTravel = Mathf.Abs(doorRight.transform.position.x * doorRight.transform.localScale.x);
        targetDoorRight = doorRight.transform.position.x + distanceToTravel;
        targetDoorLeft = doorLeft.transform.position.x - distanceToTravel;
    }
    void Update()
    {
        if(!pressurePlatePlayer || !pressurePlateCube){
            pressurePlatePlayer = GameObject.FindGameObjectWithTag("Plate").GetComponent<PressurePlate>();
            pressurePlateCube = GameObject.FindGameObjectWithTag("PlateCube").GetComponent<PressurePlateElevator>();
        }else{
            bool arePlatesPressed = pressurePlatePlayer.isPressed && pressurePlateCube.isPressed;
            if (arePlatesPressed)
            {
                MoveDoors();
            }
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
        }
    }
}

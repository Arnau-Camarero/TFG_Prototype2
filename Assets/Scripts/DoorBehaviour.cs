using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorBehaviour : MonoBehaviour
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

    void Start(){

        pressurePlate1 = GameObject.FindGameObjectWithTag("Plate").GetComponent<PressurePlate>();
        pressurePlate2 = GameObject.FindGameObjectWithTag("PlateCube").GetComponent<PressurePlate>();

        doorLeft.transform.position = doorLeftInitPos;
        doorRight.transform.position = doorRightInitPos;

        distanceToTravel = Mathf.Abs(doorRight.transform.position.x * doorRight.transform.localScale.x);
        targetDoorRight = doorRight.transform.position.x + distanceToTravel;
        targetDoorLeft = doorLeft.transform.position.x - distanceToTravel;
    }
    void Update()
    {
        if(!pressurePlate1 || !pressurePlate2){
            pressurePlate1 = GameObject.FindGameObjectWithTag("Plate").GetComponent<PressurePlate>();
            pressurePlate2 = GameObject.FindGameObjectWithTag("PlateCube").GetComponent<PressurePlate>();
        }else{
            bool arePlatesPressed = pressurePlate1.isPressed && pressurePlate2.isPressed;
            if (arePlatesPressed)
            {
                Debug.Log("MOVINGDOORS");
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

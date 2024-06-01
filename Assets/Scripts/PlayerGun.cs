using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerGun : NetworkBehaviour
{

    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    private Camera mainCamera;
    private Vector3 aimDirection;

    // public override void OnNetworkSpawn(){
        // if(!IsOwner){
        //     enabled = false;
        //     return;
        // }

    // }

    void Start(){
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if(!IsOwner){
            enabled = false;
            return;
        }
        //Vector2 PlayerPos = new Vector2(transform.position.x, transform.position.z);
        //Vector2 MousePos = new Vector2(MousePos3D.x, MousePos3D.y);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldMousePos = hit.point;
            worldMousePos.y = transform.position.y; // Fix player Y position

            aimDirection = worldMousePos - transform.position;
            aimDirection.Normalize();

            transform.rotation = Quaternion.LookRotation(aimDirection);

            if(Input.GetButtonDown("Fire1")){
                //Shoot a projectile from player position in that direction
                ShootRpc(aimDirection);
            }        
        }
    }

    private void Shoot(Vector3 dir){
        
        //ShootServerRpc(dir);
        
        if (projectilePrefab == null)
        {
            Debug.LogError("No projectile prefab!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        projectile.GetComponent<Rigidbody>().AddRelativeForce(dir * projectileSpeed);
        
        // if (projectileRigidbody != null){
        //     print("SHOOT");
        //     projectileRigidbody.velocity = directionToShoot * projectileSpeed * Time.deltaTime;
        // }
    }

    [Rpc(SendTo.Everyone)]
    private void ShootRpc(Vector3 dir){
        
        if (projectilePrefab == null)
        {
            Debug.LogError("No projectile prefab!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        projectile.GetComponent<Rigidbody>().AddRelativeForce(dir * projectileSpeed);

    }
}

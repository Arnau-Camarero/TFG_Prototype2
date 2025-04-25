using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5.0f;
    public float sprint = 1.3f;

    public float jumpForce = 8.0f;

    public float gravity = 9.81f;

    private Rigidbody rb;

    public bool isGrounded = true;
    public bool canJump = true;
    public bool isFinished = false;

    public override void OnNetworkSpawn(){
        if(!IsOwner){
            enabled = false;
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Obtener la referencia al componente Rigidbody del jugador
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Obtener la entrada del jugador para el movimiento horizontal
        float horitzontalMovement = Input.GetAxis("Horizontal");

        // Obtener la entrada del jugador para el movimiento vertical
        float verticalMovement = Input.GetAxis("Vertical");

        // Aplicar el movimiento horizontal al jugador
        Vector3 movement = new Vector3(horitzontalMovement, 0.0f, verticalMovement);
        if(Input.GetKey(KeyCode.LeftShift)) movement = movement.normalized * speed * sprint * Time.deltaTime;
        else{
            movement = movement.normalized * speed * Time.deltaTime;
        }
        rb.MovePosition(transform.position + movement);

        // Si el jugador está en el suelo y presiona la tecla de salto
        if (isGrounded && Input.GetKey(KeyCode.Space) && canJump)
        {
            // Aplicar la fuerza de salto al jugador
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Indicar que el jugador ya no está en el suelo
            isGrounded = false;
        }

        // Aplicar la gravedad al jugador
        rb.AddForce(Vector3.down * gravity * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si el jugador choca con el suelo
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
        {
            isGrounded = true;
            canJump = true;
        }

        if (collision.gameObject.tag == "Prop" && this.gameObject.transform.position.y < 1)
        {
            canJump = false;
        }
        // else{
        //     canJump = true;
        //     isGrounded = true;
        // }
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Finish"){
            isFinished = true;
        }
    }

    void OnCollisionExit(Collision col){
        if (col.gameObject.tag == "Prop")
        {
            canJump = true;
        }
    }
}

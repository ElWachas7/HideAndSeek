using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3;
    private Vector2 movementInputs;
    private float stamina = 10;
    private bool recharging;

    //hacer la stateMachine
    void Start()
    {
        if (recharging)
            Idle();
        else
            Movement();
    }
    //detectar el input
    //

    void Update()
    {
        movementInputs = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {

        GetComponent<Rigidbody>().velocity = new Vector3(movementInputs.x, 0 , movementInputs.y) * speed;
        stamina -= Time.deltaTime;
    }
    void ManageStamina()
    {
        if (!recharging && stamina <= 0)
        {
            recharging = true;
        }
        else if (stamina >= 10)
        {
            recharging = false;
        }
    }
    void Idle()
    {
        stamina += Time.deltaTime;
    }
}

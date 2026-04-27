using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour: MonoBehaviour
{
    public float speed = 3f;
    public float maxStamina = 10f;
    public float minStamina = 2.0f;

    [HideInInspector] public float currentStamina;
    //si el booleano no está seteado, por default es FALSO
    [HideInInspector] public bool isRecharging;
    [HideInInspector] public Vector3 movementInput;
    [HideInInspector] public Rigidbody rb;

    private StateMachine<PlayerStates> sm;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
    }

    void Start()
    {
        sm = new StateMachine<PlayerStates>();

        var idle = new PlayerIdleState(this, sm);
        var move = new PlayerMoveState(this, sm);

        sm.AddState(idle, PlayerStates.Idle);
        sm.AddState(move, PlayerStates.Move);

        idle.AddTransition(move, PlayerStates.Move);
        move.AddTransition(idle, PlayerStates.Idle);

        sm.SetCurrent(idle);
    }

    void Update()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        ManageStamina();

        sm.Update();
    }

    void ManageStamina()
    {
        if (!isRecharging && currentStamina <= 0)
        {
            isRecharging = true;
        }
        else if (currentStamina >= minStamina)
        {
            isRecharging = false;
        }
    }
}

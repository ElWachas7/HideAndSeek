using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour: MonoBehaviour, ISteering
{
    public float speed = 3f;
    public Vector3 Velocity => rb.velocity;
    public float maxStamina = 10f;
    public float minStamina = 2.0f;
    public UIManager UIManager => uiManager;

    [HideInInspector] public float currentStamina;
    //si el booleano no est� seteado, por default es FALSO 
    [HideInInspector] public bool isRecharging;
    [HideInInspector] public Vector3 movementInput;
    [HideInInspector] public Rigidbody rb;
    private StateMachine<PlayerStates> sm;
    [SerializeField] private UIManager uiManager;

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
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        movementInput = (transform.forward * verticalInput) + (transform.right * horizontalInput);
        movementInput.Normalize();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.OnTryPause();
        }
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
    public void Kill()
    {
        GameManager.Instance.LoseGame();
        gameObject.SetActive(false);
    }
}

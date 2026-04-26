using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerBehaviour player, StateMachine<PlayerStates> sm) : base(player, sm)
    {
        this.player = player;
    }
    public override void Execute()
    {
        //base.Execute();

        player.rb.velocity = player.movementInput.normalized * player.speed;

        player.currentStamina -= Time.deltaTime;

      
        if (player.movementInput.magnitude <= 0f || player.currentStamina <= 0)
        {
            Debug.Log("kgf");
            _sm.ChangeState(PlayerStates.Idle);
        }
    }
}

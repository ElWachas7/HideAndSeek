using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerBehaviour player, StateMachine<PlayerStates> sm) : base(player, sm)
    {
        this.player = player;
    }
    public override void Execute()
    {

        player.rb.velocity = player.movementInput * player.speed;

        if (player.movementInput.magnitude <= 0.1f || player.currentStamina <= 0)
        {
            stateMachine.ChangeState(PlayerStates.Idle);
        }
        else
        {
            player.currentStamina -= Time.fixedDeltaTime;
            player.UIManager.UpdateStamina(player.currentStamina);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    //cuando una variable se retorna a si misma se conoce como un 'constructor'
    public PlayerIdleState(PlayerBehaviour player, StateMachine<PlayerStates> sm) : base(player, sm)
    {
        this.player = player;
    }

    public override void Execute()
    {

        player.rb.velocity = Vector3.zero;

        if (player.movementInput.magnitude > 0.1f && !player.isRecharging)
        {
            stateMachine.ChangeState(PlayerStates.Move);
        }
        else if (player.currentStamina <= player.maxStamina)
        {
            player.currentStamina += Time.fixedDeltaTime * 2;
        }
    }
}

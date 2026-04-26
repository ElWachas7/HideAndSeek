using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerIdleState : PlayerState
{
    //cuando una variable se retorna a si misma se conoce como un 'constructor'
    public PlayerIdleState(PlayerBehaviour player, StateMachine<PlayerStates> sm) : base(player, sm)
    {
        this.player = player;
    }

    public override void Execute()
    {
        //base.Execute();

        player.rb.velocity = Vector3.zero;

        player.currentStamina += Time.deltaTime;

        if (player.movementInput.magnitude > 0.1f && !player.isRecharging)
        {
            _sm.ChangeState(PlayerStates.Move);
        }
    }
}

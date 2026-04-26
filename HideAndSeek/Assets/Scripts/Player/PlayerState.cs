using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum PlayerStates
    {
        Idle,
        Move
    }
public class PlayerState : State<PlayerStates>
{
    protected PlayerBehaviour player;

    public PlayerState(PlayerBehaviour player, StateMachine<PlayerStates> sm ) : base(sm)
    {
        this.player = player;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    protected State state;

    public void SetState(State state)
    {
        this.state = state;
        state.Start();
    }
}

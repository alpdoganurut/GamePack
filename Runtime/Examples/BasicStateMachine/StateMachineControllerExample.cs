using GamePack.BasicStateMachineSystem;
using UnityEngine;

namespace GamePack.Examples.BasicStateMachine
{
    // States
    public enum State
    {
        Idle, Attack, Defense
    }
    
    class StateMachineControllerExample : StateMachineControllerBase<State>
    {
        protected override void StateUpdate(State state)
        {
            
        }

        protected override void StateChange(State currentState, State lastState)
        {
            Debug.Log($"StateChanged to {currentState}");

        }
    }
}
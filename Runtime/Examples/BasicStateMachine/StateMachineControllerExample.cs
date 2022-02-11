using GamePack.Tools.BasicStateMachineSystem;
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
        protected override void StateUpdate(State state, BasicStateMachine<State> stateMachine)
        {
            
        }

        protected override void StateChange(State currentState, State lastState, BasicStateMachine<State> stateMachine)
        {
            Debug.Log($"StateChanged to {currentState}");

        }
    }
}
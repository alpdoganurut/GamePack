using System;
using GamePack.Boilerplate.Structure;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace GamePack.BasicStateMachineSystem
{
    public abstract class StateMachineControllerBase<T>: ControllerBase where T: Enum
    {
        [ShowInInspector] private BasicStateMachine<T> _stateMachine;

        protected BasicStateMachine<T> StateMachine => _stateMachine;

        protected virtual void Awake()
        {
            _stateMachine = new BasicStateMachine<T>(StateChange, StateUpdate);
        }

        protected abstract void StateUpdate(T state);

        protected abstract void StateChange(T currentState, [CanBeNull] T lastState);
    }
}
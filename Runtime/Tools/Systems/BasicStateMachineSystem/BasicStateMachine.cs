using System;
using Sirenix.OdinInspector;

namespace GamePack.Tools.BasicStateMachineSystem
{
    public delegate void StateMachineChange<T>(T currentState, T oldState, BasicStateMachine<T> stateMachine) where T : Enum;

    public delegate void StateMachineUpdate<T>(T currentState, BasicStateMachine<T> stateMachine)
        where T : Enum;

    public abstract class BasicStateMachineBase
    {
        public abstract void Update();
    }

    [Serializable]
    public class BasicStateMachine<T> : BasicStateMachineBase where T : Enum
    {
        private StateMachineChange<T> _stateChange;
        private StateMachineUpdate<T> _stateUpdate;

        [ShowInInspector] private T _currentState;
        [ShowInInspector] private T _lastState;

        public BasicStateMachine(StateMachineChange<T> stateChange,
            StateMachineUpdate<T> stateUpdate,
            T initialState = default)
        {
            _stateChange = stateChange;
            _stateUpdate = stateUpdate;
            _currentState = initialState;

            BasicStateMachineEngine.AddStateMachine(this);
        }

        public override void Update()
        {
            if (!Equals(_currentState, _lastState))
            {
                _stateChange?.Invoke(_currentState, _lastState, this);
            }

            _stateUpdate?.Invoke(_currentState, this);

            _lastState = _currentState;
        }

        [Button]
        protected void SetState(T state)
        {
            _currentState = state;
        }
    }
}
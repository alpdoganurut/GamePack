using System;
using Sirenix.OdinInspector;

namespace GamePack.BasicStateMachineSystem
{
    public delegate void StateMachineChange<T>(T currentState, T oldState) where T : Enum;

    public delegate void StateMachineUpdate<T>(T currentState)
        where T : Enum;

    public abstract class BasicStateMachineBase
    {
        internal abstract void Update();
    }

    [Serializable]
    public class BasicStateMachine<T> : BasicStateMachineBase where T : Enum
    {
        private StateMachineChange<T> _stateChange;
        private StateMachineUpdate<T> _stateUpdate;

        [ShowInInspector, ReadOnly] private T _currentState;
        [ShowInInspector, ReadOnly] private T _lastState;

        private bool _isInitial = true;
        
        public T State => _currentState;

        public BasicStateMachine(StateMachineChange<T> stateChange,
            StateMachineUpdate<T> stateUpdate,
            T initialState = default)
        {
            _stateChange = stateChange;
            _stateUpdate = stateUpdate;
            _currentState = initialState;

            BasicStateMachineEngine.AddStateMachine(this);
        }

        internal override void Update()
        {
            _stateUpdate?.Invoke(_currentState);

            _lastState = _currentState;
        }

        [Button]
        public void SetState(T state)
        {
            if(!_isInitial && Equals(_currentState, state)) return;
            
            _currentState = state;
            
            _stateChange?.Invoke(_currentState,  _lastState);
            
            /*if (_isInitial || !Equals(_currentState, _lastState))
            {
                _isInitial = false;
            }*/
        }
    }
}
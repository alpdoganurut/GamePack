using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GamePack.Timer
{
    public class Operation
    {
        public delegate void OperationAction();
        public delegate void OperationUpdateAction(float? tVal);
        public delegate void OperationEndAction();
        public delegate bool OperationFinishCondition();
        public delegate bool OperationWaitForCondition();
        public delegate bool OperationSkipCondition();

        private readonly OperationAction _action;
        private readonly OperationUpdateAction _updateAction;
        private readonly OperationEndAction _endAction;
        private readonly OperationFinishCondition _finishCondition;
        private readonly OperationWaitForCondition _waitForCondition;
        private readonly OperationSkipCondition _skipCondition;

        private Operation Parent { get; set; }
        public float? Duration { get; }
        public float Delay { get; }
        public OperationState State { get; private set; }
        public List<Operation> Children { get; } = new List<Operation>();
        public string Name { get; }

        public Operation(
            string name = null,
            float? duration = null,
            float delay = 0,
            OperationAction action = null,
            OperationUpdateAction updateAction = null,
            OperationEndAction endAction = null,
            OperationWaitForCondition waitForCondition = null,
            OperationSkipCondition skipCondition = null,
            OperationFinishCondition finishCondition = null)
        {
            Delay = delay;
            Duration = duration;
            
            _action = action;
            _updateAction = updateAction;
            _endAction = endAction;
            _finishCondition = finishCondition;
            _waitForCondition = waitForCondition;
            _skipCondition = skipCondition;
            
            State = OperationState.Waiting;

            Name = name;
        }

        public OperationTreeDescription Save()
        {
            return GetDescription();
        }
        
        public OperationTreeDescription Start()
        {
            var description = GetDescription();
            description.Start();
            return description;
        }
        
        public Operation Add(Operation operation)
        {
            operation.Parent = this;
            Children.Add(operation);
            return operation;
        }
        
        public Operation Add(
            string name = null,
            float? duration = null,
            float delay = 0,
            OperationAction action = null,
            OperationUpdateAction updateAction = null,
            OperationEndAction endAction = null,
            OperationWaitForCondition waitForCondition = null,
            OperationSkipCondition skipCondition = null,
            OperationFinishCondition finishCondition = null)
        {
            var newOp = new Operation(name, duration, delay, action, updateAction, endAction,  waitForCondition, skipCondition,
                finishCondition);
            return Add(newOp);
        }

        #region Internal API - This is used by Engine

        internal void Run()
        {
            _action?.Invoke();
            State = OperationState.Running;
        }

        internal void Update(float? tVal)
        {
            _updateAction?.Invoke(tVal);
        }
        
        internal void SetFinished()
        {
            _endAction?.Invoke();
            State = OperationState.Finished;
        }

        internal bool IsFinished()
        {
            return _finishCondition?.Invoke() ?? false;
        }

        internal bool WaitForCondition()
        {
            return _waitForCondition?.Invoke() ?? true;
        }

        internal bool ShouldSkip()
        {
            return _skipCondition?.Invoke() ?? false;
        }

        internal bool ShouldResolveImmediately()
        {
            return !Duration.HasValue && _finishCondition == null;
        }

        #endregion

        #region Private

        private Operation GetRoot()
        {
            if (Parent != null) return Parent.GetRoot();
            else return this;
        }

        private void AddSelfAndDistribute(ref List<Operation> operations)
        {
            if(operations.Contains(this)) return;

            operations.Add(this);
            
            Parent?.AddSelfAndDistribute(ref operations);
            foreach (var child in Children)
            {
                child.AddSelfAndDistribute(ref operations);
            }
        }
        
        private OperationTreeDescription GetDescription()
        {
            var root = GetRoot();
            var operations = new List<Operation>();
            AddSelfAndDistribute(ref operations);

            var operationTreeDescription = new OperationTreeDescription
            {
                Operations = operations,
                Root = root
            };
            return operationTreeDescription;
        }

        #endregion
        
    }
}
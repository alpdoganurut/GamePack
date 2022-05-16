using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.TimerSystem
{
    public readonly struct OperationTreeDescription
    {
        private readonly Operation _root;
        private readonly List<Operation> _operations;
        private readonly List<Operation> _tips;
        
        public Operation Root => _root;

        public OperationTreeDescription(Operation memberOperation) 
        {
            _root = memberOperation.GetRoot();
            _operations = new List<Operation>();
            _tips = new List<Operation>();
            memberOperation.RecursiveFindAllInTree(ref _operations, ref _tips);
        }

        public OperationTreeDescription Start(bool ignoreTimeScale = false)
        {
            if (IsWaitingOrRunning())
            {
                Debug.LogError($"{this} is already started. Not starting again.");
                return this;
            }
            
            foreach (var operation in _operations)
            {
                operation.SetWaitingToRun();
            }
            
            TimerEngine.AddOperation(_root);
            SetIgnoreTimeScale(ignoreTimeScale);

            return this;
        }

        public OperationTreeDescription Repeat(int? count = null)
        {
            OperationRepeater.Repeat(this, count);
            return this;
        }

        public OperationTreeDescription BindTo(Object obj)
        {
            foreach (var operation in _operations) operation.BindTo(obj);

            return this;
        }
        
        public void Cancel()
        {
            foreach (var operation in _operations)
            {
                operation.Cancel();
            }
        }

        private void SetIgnoreTimeScale(bool isIgnore)
        {
            foreach (var operation in _operations)
            {
                operation.SetIgnoreTimeScale(isIgnore);
            }
        }

        internal OperationTreeDescription AddOperation(Operation operation)
        {
            var tip = GetSingleTip();
            Assert.IsNotNull(tip, $"{nameof(OperationTreeDescription)} has more than 1 tips. Can't add operation.");

            if(IsWaitingOrRunning()) operation.SetWaitingToRun();
            
            tip.Add(operation);
            _operations.Add(operation);

            if (tip.BindObjExists)
                operation.BindTo(tip.BindObj);
            
            return this;
        }

        public bool IsCancelled() => _operations.Any(operation => operation.State == OperationState.Cancelled);

        public bool IsWaitingOrRunning() => _operations != null && _operations.Any(operation => operation.State is OperationState.Waiting or OperationState.Running);

        public bool IsFinished() => _operations.All(operation => operation.State == OperationState.Finished);
        
        public override string ToString() => $"OpTDesc, root: {_root.Name}";
        
        private Operation GetSingleTip()
        {
            if (_tips.Count != 1) return null;
            return _tips[0];
        }

    }
}
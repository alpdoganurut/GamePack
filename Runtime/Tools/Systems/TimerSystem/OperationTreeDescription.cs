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

        public void Start(bool ignoreTimeScale = false)
        {
            if (IsStarted())
            {
                Debug.LogError($"{this} is already started. Not starting again.");
                return;
            }
            
            foreach (var operation in _operations)
            {
                operation.SetWaiting();
            }
            
            TimerEngine.AddOperation(_root);
            SetIgnoreTimeScale(ignoreTimeScale);
        }
        
        public void StartRepeating(bool ignoreTimeScale = false)
        {
            Start(ignoreTimeScale);
            OperationRepeater.Repeat(this);
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

        public OperationTreeDescription AddOperation(Operation operation)
        {
            var tip = GetSingleTip();
            Assert.IsNotNull(tip, $"{nameof(OperationTreeDescription)} has more than 1 tips. Can't add operation.");

            if(IsStarted()) operation.SetWaiting();
            
            tip.Add(operation);
            _operations.Add(operation);
            
            return this;
        }

        private Operation GetSingleTip()
        {
            if (_tips.Count != 1) return null;
            return _tips[0];
        }

        public bool IsCancelled()
        {
            return _operations.Any(operation => operation.State == OperationState.Cancelled);
        }
        
        public bool IsStarted()
        {
            return _operations.Any(operation => operation.State == OperationState.Waiting);
        }

        public override string ToString()
        {
            return $"OpTDesc, root: {_root.Name}";
        }
    }
}
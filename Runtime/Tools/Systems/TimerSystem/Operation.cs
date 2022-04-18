using System.Collections.Generic;
using GamePack.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable InconsistentNaming

namespace GamePack.TimerSystem
{
    public class Operation
    {
        protected const float NullFloatValue = -1f;

        public delegate void OperationAction();
        public delegate void OperationUpdateAction(float tVal);
        public delegate void OperationEndAction();
        public delegate bool OperationFinishCondition();
        public delegate bool OperationWaitForCondition();
        public delegate bool OperationSkipCondition();

        #region Readonly State

        internal string Name { get; set; }
        protected float? _duration;
        internal float Delay { get; set; }
        protected EaseCurve? _ease;
        // protected AnimationCurve _easeCurve;

        private readonly OperationAction _action;
        protected OperationUpdateAction _updateAction;
        private readonly OperationEndAction _endAction;
        private readonly OperationFinishCondition _finishCondition;
        private readonly OperationWaitForCondition _waitForCondition;
        protected OperationSkipCondition _skipCondition;

        private Operation Parent { get; set; }
        
        #endregion

        #region Mutating State

        internal OperationState State { get; private set; } = OperationState.NotStarted;
        internal List<Operation> Children { get; } = new List<Operation>();
        
        internal bool IsIgnoreTimeScale { get; private set; }
        
        internal bool IsRunEndActionBeforeCancel { get; private set; }

        internal float? Duration => _duration < 0 ? null : _duration;
        
        #endregion

        public Operation(
            string name = null,
            float? duration = null,
            float delay = 0,
            bool ignoreTimeScale = false,
            EaseCurve? ease = null,
            // AnimationCurve easeCurve = null,
            OperationAction action = null,
            OperationUpdateAction updateAction = null,
            OperationEndAction endAction = null,
            OperationWaitForCondition waitForCondition = null,
            OperationSkipCondition skipCondition = null,
            OperationFinishCondition finishCondition = null)
        {
            #if UNITY_EDITOR
            // Can't start on editor mode
            Assert.IsTrue(Application.isPlaying);  
            #endif
            
            // Validity checks
            
            Assert.IsTrue( !duration.HasValue || duration > 0, $"duration must be > 0 ({duration})");
            // Check if duration and finish condition both supplied
            var isDurationSupplied = duration.HasValue;
            Assert.IsFalse(isDurationSupplied && finishCondition != null, "Duration and finish condition both can't be supplied!"); // Botch can't be supplied
            // Ease can't be used if no duration is set
            Assert.IsTrue(ease == null || isDurationSupplied, "Ease can't be used if no duration is set!");
            // There can't be two easing
            // Assert.IsFalse(ease != null && easeCurve != null, "There can't be two easing method!");
            
            Name = name;
            _duration = duration;
            Delay = delay;
            IsIgnoreTimeScale = ignoreTimeScale;
            
            _ease = ease;
            // _easeCurve = easeCurve;
            
            _action = action;
            _updateAction = updateAction;
            _endAction = endAction;
            _finishCondition = finishCondition;
            _waitForCondition = waitForCondition;
            _skipCondition = skipCondition;
        }

        #region Public API

        public OperationTreeDescription Save()
        {
            return new OperationTreeDescription(this);
        }
        
        public OperationTreeDescription Start(bool ignoreTimeScale = false)
        {
            var description = Save();
            description.Start(ignoreTimeScale);
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
            bool ignoreTimeScale = false,
            EaseCurve? ease = null,
            AnimationCurve easeCurve = null,
            OperationAction action = null,
            OperationUpdateAction updateAction = null,
            OperationEndAction endAction = null,
            OperationWaitForCondition waitForCondition = null,
            OperationSkipCondition skipCondition = null,
            OperationFinishCondition finishCondition = null)
        {
            var newOp = new Operation(name, duration, delay, ignoreTimeScale, ease, action, updateAction, endAction,  waitForCondition, skipCondition,
                finishCondition);
            return Add(newOp);
        }

        #endregion

        #region Internal API - These are used by Engine and OperationTreeDescription

        internal void Run()
        {
            _action?.Invoke();
            OnRun();
            State = OperationState.Running;
        }

        protected virtual void OnRun() {}

        internal void Update(float? tVal)
        {
            if (!tVal.HasValue)
            {
                _updateAction?.Invoke(NullFloatValue);
                return;
            }
            Assert.IsTrue(tVal.HasValue);
            Assert.IsTrue(tVal >= 0 && tVal <= 1, $"tVal is not between [0,1]! tVal: {tVal}");
            
            // Apply easing
            if (_ease != null)
                _ease.Value.Evaluate(tVal.Value);
                // tVal = EasingFunction.GetEasingFunction(_ease.Value)(0, 1, tVal.Value);
            // else if (_easeCurve != null)
                // tVal = _easeCurve.Evaluate(tVal.Value);
            
            _updateAction?.Invoke(tVal.Value);
        }
        
        internal void SetWaiting() => State = OperationState.Waiting;

        internal void Finish()
        {
            State = OperationState.Finished;
            _endAction?.Invoke();
        }

        internal bool IsFinisCondition() => _finishCondition?.Invoke() ?? false;

        internal bool WaitForCondition() => _waitForCondition?.Invoke() ?? true;

        internal bool ShouldSkip() => _skipCondition?.Invoke() ?? false;

        internal bool ShouldResolveImmediately() => !Duration.HasValue && _finishCondition == null;

        internal void SetIgnoreTimeScale(bool isIgnore) => IsIgnoreTimeScale = isIgnore;

        internal void Cancel(bool isRunEndActionBeforeCancel = false)
        {
            IsRunEndActionBeforeCancel = isRunEndActionBeforeCancel; 
            State = OperationState.Cancelled;
        }

        #endregion

        #region Private

        public Operation GetRoot()
        {
            if (Parent != null) return Parent.GetRoot();
            else return this;
        }

        public void RecursiveFindAllInTree(ref List<Operation> operations, ref List<Operation> tips)
        {
            if(operations.Contains(this)) return;

            operations.Add(this);
            if (Children.Count == 0) tips.Add(this);
            
            Parent?.RecursiveFindAllInTree(ref operations, ref tips);
            
            foreach (var child in Children)
            {
                child.RecursiveFindAllInTree(ref operations, ref tips);
            }
        }
        
        /*
        private OperationTreeDescription GetDescription()   // TODO: Move this to OperationTreeDescription constructor
        {
            var root = GetRoot();
            var operations = new List<Operation>();
            var tips = new List<Operation>();
            RecursiveFindAllInTree(ref operations, ref tips);

            var operationTreeDescription = new OperationTreeDescription
            {
                Operations = operations,
                Root = root,
                Tips = tips
            };
            return operationTreeDescription;
        }
        */
        
        #endregion

    }
}
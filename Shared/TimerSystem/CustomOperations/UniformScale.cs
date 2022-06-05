using System;
using GamePack.Logging;
using GamePack.Utilities;
using UnityEngine;

namespace GamePack.TimerSystem
{
    public class UniformScale: Operation
    {
        private readonly Transform _transform;
        private readonly float _targetScale;
        private Vector3 _initialScale;

        public UniformScale(
            Transform transform,
            float duration,
            float targetScale,
            float delay = 0f,
            EaseCurve? ease = null,
            string name = null,
            OperationSkipCondition skipCondition = null)
        {
            _targetScale = targetScale;
            _transform = transform;
            _duration = duration;
            Delay = delay;
            _skipCondition = skipCondition;

            _ease = ease;

            _updateAction = UpdateAction;
            
            Name = name ?? $"{(_transform ? _transform.name + " "  : "")} {nameof(UniformScale)}";
            
            BindTo(transform);

#if UNITY_EDITOR    // Check if initial scale is uniform.
            if (Math.Abs(_transform.localScale.x - _transform.localScale.y) > Mathf.Epsilon ||
                Math.Abs(_transform.localScale.x - _transform.localScale.z) > Mathf.Epsilon)
            {
                ManagedLog.LogError($"{transform.name} is scale is not uniform.");
            }
#endif
        }

        private protected override void OnRun() => _initialScale = _transform.localScale;

        private void UpdateAction(float tVal) => 
            _transform.localScale = Vector3.Lerp(_initialScale, new Vector3(_targetScale, _targetScale, _targetScale), tVal);
    }
}
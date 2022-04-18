using GamePack.Utilities;
using UnityEngine;

namespace GamePack.TimerSystem
{
    public class UniformScaleOp: Operation
    {
        private readonly Transform _transform;
        private readonly float _targetScale;
        private Vector3 _initialScale;

        public UniformScaleOp(
            Transform transform,
            float duration,
            float targetScale,
            float delay = 0f,
            EaseCurve? ease = null,
            AnimationCurve easeCurve = null,
            string name = null,
            OperationSkipCondition skipCondition = null)
        {
            _targetScale = targetScale;
            _transform = transform;
            _duration = duration;
            Delay = delay;
            _skipCondition = skipCondition;

            _ease = ease;
            // _easeCurve = easeCurve;

            _updateAction = UpdateAction;
            
            Name = name ?? $"{(_transform ? _transform.name + " "  : "")} {nameof(UniformScaleOp)}";
        }

        protected override void OnRun()
        {
            _initialScale = _transform.localScale;
        }

        private void UpdateAction(float tVal)
        {
            _transform.localScale = Vector3.Lerp(_initialScale, new Vector3(_targetScale, _targetScale, _targetScale), tVal);
        }
    }
}
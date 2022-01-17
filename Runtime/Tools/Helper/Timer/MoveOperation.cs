using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.Timer
{
    public class MoveOperation: Operation
    {
        private Vector3 _initialPos;
        private readonly Transform _transform;
        private readonly float? _suppliedDuration;
        private readonly float? _speed;
        private readonly Vector3? _targetPos;
        private readonly Transform _targetPosRef;
        
        // private float _moveDuration;

        public MoveOperation(Transform transform = null,
            float? duration = null, float? speed = null,
            Vector3? targetPos = null, [CanBeNull] Transform targetPosRef = null,
            EasingFunction.Ease? ease = null,
            AnimationCurve easeCurve = null,
            string name = null)
        {
            Assert.IsTrue(targetPos != null || targetPosRef, "targetPos or targetPosRef must have value!");
            Assert.IsTrue(speed > 0 || duration > 0, "Either speed or duration must have value and bigger than zero!");

            // Cache initialization parameters to calculate initial movement values OnStart.
            _transform = transform;
            
            _suppliedDuration = duration;
            _speed = speed;
            
            _targetPos = targetPos;
            _targetPosRef = targetPosRef;

            _ease = ease;
            _easeCurve = easeCurve;
            
            Name = name ?? $"{(_transform ? _transform.name + " "  : "")}Move Operation";

            
            _updateAction = UpdateAction;
        }

        protected override void OnRun()
        {
            _initialPos = _transform.position;
            var finalDuration = _suppliedDuration ?? (Vector3.Distance(_targetPos ?? _targetPosRef.position, _initialPos) / _speed);
            _duration = finalDuration.Value;
        }

        private void UpdateAction(float tVal)
        {
            var tPos = _targetPos ?? _targetPosRef.position;
            var pos = Vector3.Lerp(_initialPos, tPos, tVal);
            _transform.position = pos;
        }

    }
}
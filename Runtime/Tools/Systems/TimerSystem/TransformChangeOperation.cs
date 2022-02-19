using GamePack.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.TimerSystem
{
    public class TransformChangeOperation: Operation
    {
        private readonly Transform _transform;
        private readonly float? _suppliedDuration;
        private readonly float? _moveSpeed;
        
        // Position
        private Vector3 _initialPos;
        private readonly Vector3? _targetPos;
        private readonly Transform _targetPosRef;
        
        // Rotation
        private Quaternion _initialRot;
        private readonly Quaternion? _targetRot;
        private readonly Transform _targetRotRef;
        
        // Scale
        private Vector3 _initialScale;
        private readonly Vector3? _targetScale;
        private readonly Transform _targetScaleRef;
        
        private readonly bool _isMove;
        private readonly bool _isRotate;
        private readonly bool _isScale;

        public TransformChangeOperation(
            Transform transform = null,
            float? duration = null,
            float? moveSpeed = null,
            Vector3? targetPos = null, [CanBeNull] Transform targetPosRef = null,
            Quaternion? targetRot = null, [CanBeNull] Transform targetRotRef = null,
            Vector3? targetScale = null, [CanBeNull] Transform targetScaleRef = null,
            EasingFunction.Ease? ease = null, AnimationCurve easeCurve = null,
            string name = null)
        {
            _isMove = targetPos != null || targetPosRef;
            _isRotate = targetRot != null || targetRotRef;
            _isScale = targetScale != null || targetScaleRef;
            
            Assert.IsTrue(_isMove || _isRotate || _isScale, "Operation is not Move, Rotate or Scale, supply target for one of them.");
            Assert.IsTrue(moveSpeed > 0 || duration > 0, "Either speed or duration must have value and bigger than zero!");

            // Cache initialization parameters to calculate initial movement values OnStart.
            _transform = transform;
            
            _suppliedDuration = duration;
            _moveSpeed = moveSpeed;
            
            _targetPos = targetPos;
            _targetPosRef = targetPosRef;
            
            _targetRot = targetRot;
            _targetRotRef = targetRotRef;
            
            _targetScale = targetScale;
            _targetScaleRef = targetScaleRef;

            _ease = ease;
            _easeCurve = easeCurve;
            
            Name = name ?? $"{(_transform ? _transform.name + " "  : "")} {nameof(TransformChangeOperation)}";
            
            _updateAction = UpdateAction;
        }

        protected override void OnRun()
        {
            _initialPos = _transform.position;
            _initialRot = _transform.rotation;
            _initialScale = _transform.lossyScale;
            
            var finalDuration = _suppliedDuration ?? (Vector3.Distance(_targetPos ?? _targetPosRef.position, _initialPos) / _moveSpeed);
            Assert.IsTrue(finalDuration != null, nameof(finalDuration) + " != null");
            _duration = finalDuration.Value;
        }

        private void UpdateAction(float tVal)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Assert.IsTrue(tVal != NullFloatValue, $"tVal has no value in {nameof(TransformChangeOperation)}. This points to an internal err ");
            if (!_transform)
            {
                Debug.LogError($"{_transform} is destroyed.");
                return;
            }
            
            if(_isMove && _targetPos == null && !_targetPosRef)
            {
                Debug.LogError($"_targetPosRef no longer exists.");
                return;
            }
            if(_isRotate && _targetRot == null && !_targetRotRef)
            {
                Debug.LogError($"_targetRotRef no longer exists.");
                return;
            }
            if(_isScale && _targetScale == null && !_targetScaleRef)
            {
                Debug.LogError($"_targetRotRef no longer exists.");
                return;
            }
            
            var pos = default(Vector3);
            var rot = default(Quaternion);
            var scale = default(Vector3);
            
            // Get Update Values
            if(_isMove)
                pos = GetUpdatedPos(tVal);
            if(_isRotate)
                rot = GetUpdatedRotation(tVal);
            if(_isScale)
            {
                scale = GetUpdatedScale(tVal);
            }

            // Apply values
            if(_isMove && _isRotate)
                _transform.SetPositionAndRotation(pos, rot);
            else if(_isMove)
                _transform.position = pos;
            else if (_isRotate)
                _transform.rotation = rot;
            if (_isScale)
            {
                _transform.SetGlobalScale(scale);
            }
        }

        private Vector3 GetUpdatedPos(float tVal)
        {
            var tPos = _targetPos ?? _targetPosRef.position;
            return Vector3.Lerp(_initialPos, tPos, tVal);
        }

        private Quaternion GetUpdatedRotation(float tVal)
        {
            var tRot = _targetRot ?? _targetRotRef.rotation;
            return Quaternion.Lerp(_initialRot, tRot, tVal);
        }

        private Vector3 GetUpdatedScale(float tVal)
        {
            var tPScale = _targetScale ?? _targetScaleRef.lossyScale;
            return Vector3.Lerp(_initialScale, tPScale, tVal);
        }
    }
}
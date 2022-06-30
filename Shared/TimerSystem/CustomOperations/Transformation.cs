using System;
using System.Diagnostics.CodeAnalysis;
using GamePack.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.TimerSystem
{
    public class Transformation: Operation
    {
        private readonly Transform _transform;
        private readonly float? _suppliedDuration;
        private readonly float? _moveSpeed;
        
        // Position
        private Vector3 _initialPos;
        private readonly Vector3? _targetPos;
        private readonly Transform _targetPosRef;
        private readonly Func<Vector3> _targetPosAction;

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
        
        private readonly VerticalCurveMovement? _curveMovement;

        public Transformation(
            Transform transform = null,
            float? duration = null,
            float delay = 0,
            float? moveSpeed = null,
            Vector3? targetPos = null, [CanBeNull] Transform targetPosRef = null, Func<Vector3> targetPosAction = null,
            Quaternion? targetRot = null, [CanBeNull] Transform targetRotRef = null,
            Vector3? targetScale = null, [CanBeNull] Transform targetScaleRef = null,
            EaseCurve? ease = null,
            string name = null,
            VerticalCurveMovement? curveMovement = null)
        {
            _isMove = targetPos != null || targetPosRef || targetPosAction != null;
            _isRotate = targetRot != null || targetRotRef;
            _isScale = targetScale != null || targetScaleRef;

            // Make sure only one method of position is true TODO: This might be better done with overloads, this method gets a bit more complicated than intended
            if (targetPos != null)
                Assert.IsTrue(targetPosRef == null && targetPosAction == null);
            if(targetPosRef != null)
                Assert.IsTrue(targetPos == null && targetPosAction == null);
            if(targetPosAction != null)
                Assert.IsTrue(targetPos == null && targetPosRef == null);
            
            Assert.IsTrue(_isMove || _isRotate || _isScale, "Operation is not Move, Rotate or Scale, supply target for one of them.");
            Assert.IsTrue(moveSpeed > 0 || duration > 0, "Either speed or duration must have value and bigger than zero!");

            // Cache initialization parameters to calculate initial movement values OnStart.
            _transform = transform;
            
            _suppliedDuration = duration;
            Delay = delay;
            _moveSpeed = moveSpeed;
            
            _targetPos = targetPos;
            _targetPosRef = targetPosRef;
            _targetPosAction = targetPosAction;
            
            _targetRot = targetRot;
            _targetRotRef = targetRotRef;
            
            _targetScale = targetScale;
            _targetScaleRef = targetScaleRef;
            _curveMovement = curveMovement;

            _ease = ease;
            
            Name = name ?? $"{(_transform ? _transform.name + " "  : "")} {nameof(Transformation)}";
            
            _updateAction = UpdateAction;

            BindTo(_transform);
        }

        private protected override void OnRun()
        {
            _initialPos = _transform.position;
            _initialRot = _transform.rotation;
            _initialScale = _transform.lossyScale;
            
            var duration = _suppliedDuration ?? (Vector3.Distance(GetUpdatedPos(1), _initialPos) / _moveSpeed);
            Assert.IsTrue(duration != null, nameof(duration) + " != null");
            _duration = duration.Value;
        }

        private void UpdateAction(float tVal)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Assert.IsTrue(tVal != NullFloatValue, $"tVal has no value in {nameof(Transformation)}. This points to an internal err ");
            if (!_transform)
            {
                Debug.LogError($"{_transform} is destroyed.");
                return;
            }
            
            if(_isMove && _targetPos == null && !_targetPosRef && _targetPosAction == null)
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
                scale = GetUpdatedScale(tVal);

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
            var targetPos = _targetPos ?? (_targetPosRef ? _targetPosRef.position : _targetPosAction?.Invoke());
            System.Diagnostics.Debug.Assert(targetPos != null, nameof(targetPos) + " != null");


            var updatedPos = Vector3.Lerp(_initialPos, targetPos.Value, tVal);

            if (_curveMovement.HasValue) updatedPos.y += GetCurveMovementHeightOffset(tVal);
            
            return updatedPos;
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

        private float GetCurveMovementHeightOffset(float tVal)
        {
            Assert.IsTrue(_curveMovement.HasValue);

            return _curveMovement.Value.GetHeight(tVal);
        }
        
        public static Transformation Match(Transform transform, Transform targetTransform, float duration)
        {
            return new Transformation(transform, duration: duration,
                targetPosRef: targetTransform,
                targetRotRef: targetTransform, 
                targetScaleRef: targetTransform);
        }
        
    }

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct VerticalCurveMovement
    {
        public float MaxHeight;
        public AnimationCurve Curve;

        public float GetHeight(float tVal)
        {
            return Curve.Evaluate(tVal) * MaxHeight;
        }
    }
}
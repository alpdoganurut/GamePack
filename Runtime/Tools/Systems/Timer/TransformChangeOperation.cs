using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.Timer
{
    public class TransformChangeOperation: Operation
    {
        private readonly Transform _transform;
        private readonly float? _suppliedDuration;
        private readonly float? _moveSpeed;
        
        private Vector3 _initialPos;
        private readonly Vector3? _targetPos;
        private readonly Transform _targetPosRef;
        
        private Quaternion _initialRot;
        private readonly Quaternion? _targetRot;
        private readonly Transform _targetRotRef;
        
        private readonly bool _isMove;
        private readonly bool _isRotate;

        public TransformChangeOperation(Transform transform = null,
            float? duration = null,
            float? moveSpeed = null,
            Vector3? targetPos = null, [CanBeNull] Transform targetPosRef = null,
            Quaternion? targetRot = null, [CanBeNull] Transform targetRotRef = null,
            EasingFunction.Ease? ease = null,
            AnimationCurve easeCurve = null,
            string name = null)
        {
            _isMove = targetPos != null || targetPosRef;
            _isRotate = targetRot != null || targetRotRef;
            
            Assert.IsTrue(_isMove || _isRotate, "Operation is neither Move nor Rotate, supply one of the targets for either of them.");
            // Assert.IsTrue(targetPos != null || targetPosRef, "targetPos or targetPosRef must have value!");
            // Assert.IsTrue(targetRot != null || targetRotRef, "targetPos or targetPosRef must have value!");
            Assert.IsTrue(moveSpeed > 0 || duration > 0, "Either speed or duration must have value and bigger than zero!");

            // Cache initialization parameters to calculate initial movement values OnStart.
            _transform = transform;
            
            _suppliedDuration = duration;
            _moveSpeed = moveSpeed;
            
            _targetPos = targetPos;
            _targetPosRef = targetPosRef;
            
            _targetRot = targetRot;
            _targetRotRef = targetRotRef;

            _ease = ease;
            _easeCurve = easeCurve;
            
            Name = name ?? $"{(_transform ? _transform.name + " "  : "")} {nameof(TransformChangeOperation)}";
            
            _updateAction = UpdateAction;
        }

        protected override void OnRun()
        {
            _initialPos = _transform.position;
            _initialRot = _transform.rotation;
            
            var finalDuration = _suppliedDuration ?? (Vector3.Distance(_targetPos ?? _targetPosRef.position, _initialPos) / _moveSpeed);
            _duration = finalDuration.Value;
        }

        private void UpdateAction(float tVal)
        {
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
            
            var pos = default(Vector3);
            var rot = default(Quaternion);
            
            if(_isMove)
                pos = GetUpdatedPos(tVal);
            if(_isRotate)
                rot = GetUpdatedRotation(tVal);
            
            if(_isMove && _isRotate)
                _transform.SetPositionAndRotation(pos, rot);
            else if(_isMove)
                _transform.position = pos;
            else if (_isRotate)
                _transform.rotation = rot;
            else
            {
                Assert.IsTrue(false, "_isMove and _isRotate are false");
            }
        }

        private Quaternion GetUpdatedRotation(float tVal)
        {
            var tRot = _targetRot ?? _targetRotRef.rotation;
            var rot = Quaternion.Lerp(_initialRot, tRot, tVal);
            return rot;
        }

        private Vector3 GetUpdatedPos(float tVal)
        {
            var tPos = _targetPos ?? _targetPosRef.position;
            var pos = Vector3.Lerp(_initialPos, tPos, tVal);
            return pos;
        }
    }
}
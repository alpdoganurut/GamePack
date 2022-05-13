using GamePack.UnityUtilities;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    public class SlingCarSlideController : SlingCarControllerBase
    {
        [SerializeField, Required] private InputDragHandler _InputDragHandler;
        [SerializeField] private float _InputMultiplier = 1;
        
        private Range<float> _posRange;
        private float _currentPos;
        private float _lastInputDirection;


        protected virtual void Awake()
        {
            _InputDragHandler.DragNormalized += InputDragHandlerOnDrag;
            
            _posRange = new Range<float>{Minimum = _Road.GetPosForLane(0), Maximum = _Road.GetPosForLane(1)};
            _currentPos = _SlingCar.TargetSidePos;
        }

        private void InputDragHandlerOnDrag(Vector3 deltaPointerNorm)
        {
            if(!IsActive) return;
            
            if (_lastInputDirection * deltaPointerNorm.x < 0) _currentPos = _SlingCar.Rigidbody.position.x;
            _currentPos += deltaPointerNorm.x * _InputMultiplier;
            _currentPos = Mathf.Clamp(_currentPos, _posRange.Minimum, _posRange.Maximum);
            _SlingCar.TargetSidePos = _currentPos;

            _lastInputDirection = Mathf.Sign(deltaPointerNorm.x);
        }

        public void SetSidePos(float val)
        {
            _SlingCar.TargetSidePos = val;
        }
    }
}
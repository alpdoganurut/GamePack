using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack
{
    public class Joystick : MonoBehaviour
    {
        public event Action<Vector3> Dragged;
        public event Action<Vector3> DragEnd;

        [SerializeField] private GameObject _Finger;
        [SerializeField] private GameObject _Background;
        [Space]
        [SerializeField, Required] private bool _DisableOnPointerUp;
        [SerializeField, Required] private bool _MoveWithFinger = true;
        [Space]
        [FormerlySerializedAs("_Radius")] 
        [SerializeField, ShowIf("@_MoveWithFinger")] private float _RadiusInPixel = 1;
        [Space, SerializeField, ShowIf("@!_MoveWithFinger")] private float _ScreenToWorldMultiplier = 0.0005f;
        [FormerlySerializedAs("_InitialLocalPos")]
        [SerializeField, ShowIf("@!_MoveWithFinger")] private Vector3 _NeutralLocalPos = new Vector3(0, -0.5f, 1.1f);

        #region Development
#if UNITY_EDITOR
        [Button, ShowIf("@!_MoveWithFinger")]
        private void GetNeutralPos()
        {
            _NeutralLocalPos = Camera.gameObject.transform.worldToLocalMatrix.MultiplyPoint(_Finger.transform.position);
            
        }
#endif
        #endregion
        
        private Vector3? _lastPointerPos;
        private Vector3 _dragStartPos;

        [SerializeField] private Camera _Camera;

        private Quaternion _worldOrientation;

        private Camera Camera
        {
            get
            {
                if (!_Camera) _Camera = Camera.main;
                return _Camera;
            }
        }

        private static Vector3? PointerPos
        {
            get
            {
                if (!Input.GetMouseButton(0)) return null;
                return Input.mousePosition;
            }
        }

        private void Start()
        {
            var cameraTransform = Camera.transform;
            var transformRotation = cameraTransform.rotation;
            _Finger.transform.rotation = transformRotation;
            _Background.transform.rotation = transformRotation;
            _Finger.transform.SetParent(cameraTransform, true);
            _Background.transform.SetParent(Camera.transform, true);

            _Finger.transform.localPosition = _NeutralLocalPos;
            _Background.transform.localPosition = _NeutralLocalPos;
            
            _Finger.SetActive(!_DisableOnPointerUp);
            _Background.SetActive(!_DisableOnPointerUp);

        }

        private void Update()
        {
            _worldOrientation =
                Quaternion.Euler(0, 0 ,Camera.transform.rotation.eulerAngles.y + 90);
            
            var pointerPos = PointerPos;
            // Drag Start
            if (pointerPos.HasValue && !_lastPointerPos.HasValue)
            {
                if (_DisableOnPointerUp)
                {
                    _Finger.SetActive(true);
                    _Background.SetActive(true);
                }

                _dragStartPos = pointerPos.Value;

                if(_MoveWithFinger)
                    _Background.transform.position = Camera.ScreenToWorldPoint(_dragStartPos + Vector3.forward);
            }
            // Drag Continue
            if (pointerPos.HasValue && _lastPointerPos.HasValue)
            {
                DragContinue(pointerPos.Value);
            }
            // Drag Stop
            if (!pointerPos.HasValue && _lastPointerPos.HasValue)
            {
                if(!_MoveWithFinger)
                    _Finger.transform.localPosition = _NeutralLocalPos;
                
                if (_DisableOnPointerUp)
                {
                    _Finger.SetActive(false);
                    _Background.SetActive(false);
                }

                var delta = _lastPointerPos.Value - _dragStartPos;
                DragEnd?.Invoke(delta);
            }

            _lastPointerPos = pointerPos;
        }

        private void DragContinue(Vector3 pointerPos)
        {
            var neutralPos = _MoveWithFinger ? _dragStartPos :_NeutralLocalPos;
            
            var delta = pointerPos - _dragStartPos;
            
            Dragged?.Invoke(_worldOrientation * delta);

            if(_MoveWithFinger)
                MoveDragStartCloser();

            var deltaFinger = delta;
            if (deltaFinger.magnitude > _RadiusInPixel)
            {
                deltaFinger = deltaFinger.normalized * _RadiusInPixel;
            }

            if(_MoveWithFinger)
                _Finger.transform.position = Camera.ScreenToWorldPoint(neutralPos + deltaFinger + Vector3.forward);
            else
            {
                var localPos = neutralPos + (deltaFinger * _ScreenToWorldMultiplier);
                _Finger.transform.localPosition = localPos;
            }
        }

        private void MoveDragStartCloser()
        {
            // ReSharper disable once PossibleInvalidOperationException
            var magnitude = (_dragStartPos - PointerPos.Value).magnitude;
            var currentDeltaToRadius = magnitude / _RadiusInPixel;
            if (currentDeltaToRadius > 1)
            {
                _dragStartPos = Vector3.Lerp(PointerPos.Value, _dragStartPos, _RadiusInPixel / magnitude);
                _Background.transform.position = Camera.ScreenToWorldPoint(_dragStartPos + Vector3.forward);

            }
        }

        private void OnDisable()
        {
            if (_Finger != null) _Finger.SetActive(false);
            if (_Background != null) _Background.SetActive(false);
        }

        private void OnEnable()
        {
            _Finger.SetActive(true);
            _Background.SetActive(true);
        }
    }
}
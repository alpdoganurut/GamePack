using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePack
{
    public class InputDragHandler: MonoBehaviour
    { 
        public event Action<Vector3> DragStart;
        public event Action DragEnd;
        public event Action<Vector3> Drag;
        public event Action<Vector3> DragNormalized;
        
        private Vector3? _lastPos;
        [ShowInInspector, ReadOnly] private float _screenWidth;
        [ShowInInspector, ReadOnly] private float _screenHeight;

        public Vector3 NormalizedDeltaInput { get; private set; }
        public Vector3 NormalizedGroundAlignedDeltaInput { get; private set; }

#if UNITY_EDITOR
        private static bool IsInput => Input.GetMouseButton(0);
        private static Vector3 InputPos => Input.mousePosition;
        #else
        private static bool IsInput => Input.touchCount > 0;
        private static Vector3 InputPos => Input.touches[0].position;
        #endif
        
        
        private void Start()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
        }

        private void Update()
        {
            if(EventSystem.current && EventSystem.current.currentSelectedGameObject) return;
            
            NormalizedDeltaInput = Vector3.zero;
            NormalizedGroundAlignedDeltaInput = Vector3.zero;
            var isInput = IsInput;

            switch (isInput)
            {
                case true:
                    var inputPos = InputPos;
                    if (_lastPos.HasValue)  // Continue Drag
                    {
                        var delta = inputPos - _lastPos.Value;
                        var normalizedDelta = delta/_screenWidth;
                    
                        Drag?.Invoke(delta);
                        DragNormalized?.Invoke(normalizedDelta);
                    
                        NormalizedDeltaInput = normalizedDelta;
                        NormalizedGroundAlignedDeltaInput = new Vector3(NormalizedDeltaInput.x, 0, NormalizedDeltaInput.y);
                    }
                    else    // Start Drag
                    {
                        DragStart?.Invoke(inputPos);
                    }

                    _lastPos = inputPos;
                    break;
                // Stop Drag
                case false when _lastPos.HasValue:
                    _lastPos = null;
                    DragEnd?.Invoke();
                    break;
            }
        }
    }
}
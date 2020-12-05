using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WoodToyShop.Common
{
    public class InputDragHandler: MonoBehaviour
    { 
        public  event Action<Vector3> Drag;
        public event Action<Vector3> DragNormalized;
        
        [ShowInInspector, ReadOnly] private Vector3? _lastPos;
        [ShowInInspector, ReadOnly] private float _screenWidth;
        [ShowInInspector, ReadOnly] private float _screenHeight;
        
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
            if(EventSystem.current.currentSelectedGameObject) return;
            
            if (IsInput)
            {
                var inputPos = InputPos;
                if (_lastPos.HasValue)
                {
                    var delta = inputPos - _lastPos.Value;
                    Drag?.Invoke(delta);
                    // DraggedScreenNormalized?.Invoke(new Vector3(delta.x / _screenWidth, delta.y / _screenHeight));
                    DragNormalized?.Invoke(delta/_screenWidth);
                }

                _lastPos = inputPos;
            }
            else if (_lastPos.HasValue)
            {
                _lastPos = null;
            }
        }
    }
}
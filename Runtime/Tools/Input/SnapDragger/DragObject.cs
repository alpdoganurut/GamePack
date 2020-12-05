using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBePrivate.Global

namespace GamePack.SnapDrag
{
    [RequireComponent(typeof(Collider))]
    public class DragObject: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const float CameraDistance = 5f;
     
        private static Camera _camera;

        private static Camera MainCamera
        {
            get
            {
                if(!_camera || !_camera.gameObject.activeInHierarchy || !_camera.enabled) _camera = Camera.main;
                return _camera;
            }
        }
        
        public event Action<Vector2, Vector3> DidDrag;
        public event  Action<Vector2> DidEndDrag;
        public event  Action DidStartDrag;
        
        [SerializeField] private float _LerpSpeed = 12;

        private Vector3 _snapForce;
        private Vector3 _targetPosition;
        private Vector3 _beforeDragPos;
        private Quaternion _beforeDragRot;
        private Vector3 _beforeDragLocalScale;
        private static Camera _mainCamera;
        public bool IsDragActive { get; set; }

        public Quaternion BeforeDragRot => _beforeDragRot;
        public Vector3 BeforeDragLocalScale => _beforeDragLocalScale;
        
        [ReadOnly, ShowInInspector]
        public Vector2 DragOffset { get; set; } = Vector2.up;

        private void Awake()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _LerpSpeed * Time.deltaTime);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!IsDragActive) return;
            var transform1 = transform;
            var position = transform1.position;
            _targetPosition = position;
            _beforeDragPos = position;
            _beforeDragRot = transform1.rotation;
            _beforeDragLocalScale = transform1.localScale;
            DidStartDrag?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!IsDragActive) return;
            
            var offsetInputPos = eventData.position + DragOffset;
            var worldPos =
                GetWorldPos(offsetInputPos);

            _targetPosition = worldPos;
            
            DidDrag?.Invoke(offsetInputPos, _targetPosition);
            
            _targetPosition += _snapForce;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(!IsDragActive) return;
            var offsetInputPos = eventData.position + DragOffset;
            DidEndDrag?.Invoke(offsetInputPos);
        }
        
        private static Vector3 GetWorldPos(Vector2 inputPos)
        {
            return MainCamera.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, CameraDistance));
        }

        public void SetSnapOffset(Vector3 snapForce)
        {
            _snapForce = snapForce;
        }

        public void SetTargetPos(Vector3 pos)
        {
            _targetPosition = pos;
        }
        
        public void SetPos(Vector3 pos)
        {
            SetTargetPos(pos);
            transform.position = pos;
        }

        public void CancelDrag()
        {
            _targetPosition = _beforeDragPos;
            transform.rotation = BeforeDragRot;
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public void SetLocalScale(Vector3 snapLocalScale)
        {
            transform.localScale = snapLocalScale;
        }
    }
}
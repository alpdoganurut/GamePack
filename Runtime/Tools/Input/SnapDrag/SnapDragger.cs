using System;
using UnityEngine;

namespace GamePack.SnapDrag
{
    public class SnapDragger
    {
        public event Action<SnapDragger> DragStart;
        public event Action<SnapDragger, DragObject, bool> DragEnd;
        
        private Camera _camera;
        private DragObject _dragObject;
        private Transform _dropPoint;
        private float _getCloseDistance;
        private float _snapDistance;
        private bool _wasSnapping;

        public SnapDragger Initiate(Camera camera, DragObject dragObject, Transform dropPoint, float getCloseDistance = 4, float snapDistance = 2)
        {
            if (_dragObject)
            {
                _dragObject.DidDrag -= PartDidDrag;
                _dragObject.DidEndDrag -= PartDidEndDrag;
            }
            
            _camera = camera;
            _dragObject = dragObject;
            _dropPoint = dropPoint;
            _getCloseDistance = getCloseDistance;
            _snapDistance = snapDistance;

            _dragObject.IsDragActive = true;
            _dragObject.DidStartDrag += PartDidStartDrag;
            _dragObject.DidDrag += PartDidDrag;
            _dragObject.DidEndDrag += PartDidEndDrag;

            return this;
        }

        private void PartDidStartDrag()
        {
            DragStart?.Invoke(this);
            _dropPoint.gameObject.SetActive(true);
        }

        private void PartDidEndDrag(Vector2 inputPos)
        {
            var inputDistanceToDrop = InputDistanceToDrop(inputPos);
            
            if (inputDistanceToDrop <= _snapDistance)
            {
                _dragObject.IsDragActive = false;
                _dragObject.SetPos(_dropPoint.transform.position);
                _dragObject.SetRotation(_dropPoint.rotation);
                _dragObject.SetLocalScale(_dropPoint.localScale);
                
                DragEnd?.Invoke(this, _dragObject, true);
            }
            else
            {
                _dragObject.CancelDrag();
                DragEnd?.Invoke(this, _dragObject, false);
            }
            
            _dropPoint.gameObject.SetActive(false);
        }

        private void PartDidDrag(Vector2 inputPos, Vector3 currentPosition)
        {
            var inputDistanceToDrop = InputDistanceToDrop(inputPos);
            
            if (inputDistanceToDrop < _snapDistance)
            {
                if (!_wasSnapping)
                {
#if UNITY_IOS
                    TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Medium);
#endif
                }
                
                var fullSnapForce = _dropPoint.transform.position - currentPosition;
                _dragObject.SetSnapOffset(fullSnapForce);
                _dragObject.SetRotation(_dropPoint.rotation);

                _wasSnapping = true;
            }
            else if (inputDistanceToDrop < _getCloseDistance)
            {
                var distanceRatio = 1 - (inputDistanceToDrop / _getCloseDistance);
                var fullSnapForce = _dropPoint.transform.position - currentPosition;
                var snapForce = Vector3.Lerp(Vector3.zero, fullSnapForce, distanceRatio);

                var snapRotation = Quaternion.Lerp(_dragObject.BeforeDragRot, _dropPoint.rotation, distanceRatio);
                var snapLocalScale = Vector3.Lerp(_dragObject.BeforeDragLocalScale, _dropPoint.localScale, distanceRatio);
                
                _dragObject.SetSnapOffset(snapForce);
                _dragObject.SetRotation(snapRotation);
                _dragObject.SetLocalScale(snapLocalScale);
                _wasSnapping = false;
            }
            else
            {
                _dragObject.SetSnapOffset(Vector3.zero);
                _dragObject.SetRotation(_dragObject.BeforeDragRot);
                _dragObject.SetLocalScale(_dragObject.BeforeDragLocalScale);
                _wasSnapping = false;
            }
        }

        private float InputDistanceToDrop(Vector2 inputPos)
        {
            var (perpendicularVectorToCamera, perpendicularDistanceToCamera) = PerpendicularVectorToCamera();
            var inputWorldPos = _camera.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, perpendicularDistanceToCamera));

            #region Debug

#if UNITY_EDITOR
            Debug.DrawRay(_camera.transform.position, perpendicularVectorToCamera, Color.cyan, 4);
            Debug.DrawRay(inputWorldPos, Vector3.forward, Color.blue, 2);
#endif

            #endregion

            var distanceToDrop = (inputWorldPos - _dropPoint.transform.position).magnitude;
            return distanceToDrop;
        }

        private (Vector3 perpendicularVectorToCamera, float perpendicularDistanceToCamera) PerpendicularVectorToCamera()
        {
            var cameraTransform = _camera.transform;
            var perpendicularVectorToCamera = Vector3.Project(_dropPoint.transform.position - cameraTransform.position,
                cameraTransform.forward);
            var perpendicularDistanceToCamera = perpendicularVectorToCamera.magnitude;
            return (perpendicularVectorToCamera, perpendicularDistanceToCamera);
        }
    }
}
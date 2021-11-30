using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    public class PolyLinePathFollower: MonoBehaviour
    {
        [SerializeField, Required] private float _FollowSpeed = 10;
        [SerializeField]private float _RotationSpeed = 15;

        private PolyLinePath _path;
        
        public float PathPos { get; set; }

        private void LateUpdate()
        {
            UpdatePathPosition();
        }

        private void UpdatePathPosition()
        {
            if (!_path) return;
            
            var pos = _path.GetWorldPosAtPathPos(PathPos, out var direction);
            var t = transform;
            // t.position = pos;
            t.position = Vector3.Lerp(t.position, pos, _FollowSpeed * Time.deltaTime);
            t.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(direction), _RotationSpeed * Time.deltaTime);
        }

        [Button, DisableInEditorMode]
        public void SetPathAndPos(PolyLinePath path, float pathPos, bool rotationStaysInitially = false)
        {
            _path = path;
            PathPos = pathPos;
            
            if(PathPos > _path.TotalLength)
                Debug.LogError($"pathPos is out of bounds. pathPos: {pathPos}, Path TotalLength: {path.TotalLength}");
            
            // Set initial rotation
            _path.GetWorldPosAtPathPos(PathPos, out var direction);
            if(!rotationStaysInitially) transform.rotation = Quaternion.LookRotation(direction);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void ClearPath()
        {
            _path = null;
        }
    }
}
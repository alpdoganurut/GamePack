using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.Tools.Helper
{
    public class PolyLinePathFollower: MonoBehaviour
    {
        [SerializeField]private float _RotationSpeed = 15;

        private PolyLinePath _path;
        private bool _pathExists;

        public float PathPos { get; set; }
        public bool Path => _path;

        private void LateUpdate()
        {
            UpdatePathPosition();
        }

        private void UpdatePathPosition()
        {
            if (!_pathExists) return;
            
            var pos = _path.GetWorldPosAtPathPos(PathPos, out var direction);
            var t = transform;
            t.position = pos;
            t.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(direction), _RotationSpeed * Time.deltaTime);
        }

        [Button, DisableInEditorMode]
        public void SetPathAndPos(PolyLinePath path, float pathPos, bool rotationStaysInitially = false)
        {
            Assert.IsNotNull(path);
            _path = path;
            PathPos = pathPos;
            _pathExists = true;
            
            if(PathPos > _path.TotalLength)
                Debug.LogError($"pathPos is out of bounds. pathPos: {pathPos}, Path TotalLength: {path.TotalLength}");
            
            // Set initial rotation
            _path.GetWorldPosAtPathPos(PathPos, out var direction);
            if(!rotationStaysInitially) transform.rotation = Quaternion.LookRotation(direction);
        }

        public void ClearPath()
        {
            _path = null;
            _pathExists = false;
        }
    }
}
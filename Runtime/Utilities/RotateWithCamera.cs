using UnityEngine;

namespace GamePack.UnityUtilities
{
    public class RotateWithCamera: MonoBehaviour
    {
        [SerializeField] private Camera _Camera;
        
        private Camera Camera
        {
            get
            {
                if(!_Camera || !_Camera.gameObject.activeInHierarchy || !_Camera.enabled) _Camera = Camera.main;
                return _Camera;
            }
        }

        private void LateUpdate()
        {
            transform.rotation = Camera.transform.rotation * Quaternion.AngleAxis(180, Vector3.up);
        }
    }
}
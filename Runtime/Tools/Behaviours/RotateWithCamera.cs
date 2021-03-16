using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.UnityUtilities
{
    public class RotateWithCamera: MonoBehaviour
    {
        [SerializeField] private Camera _Camera;
        [SerializeField] private int _Angle = 180;

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
            transform.rotation = Camera.transform.rotation * Quaternion.AngleAxis(_Angle, Vector3.up);
        }

    }
}
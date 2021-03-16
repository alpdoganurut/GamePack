using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    public class RotateAround:MonoBehaviour
    {
        [SerializeField, Required] private Vector3 _Rotation = Vector3.up;

        private void Update()
        {
            transform.Rotate(_Rotation * Time.deltaTime);
        }

        public void ResetRotation()
        {
            transform.localRotation = Quaternion.identity;
        }
    }
}
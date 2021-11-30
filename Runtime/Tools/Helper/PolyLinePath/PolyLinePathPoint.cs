using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    [DisallowMultipleComponent]
    public class PolyLinePathPoint: MonoBehaviour
    {
        [SerializeField, Required] private Vector3 _Offset;
        
        public Vector3 Position => transform.parent ? transform.parent.TransformPoint( transform.localPosition) + transform.TransformDirection( _Offset) : transform.position + _Offset;
    }
}
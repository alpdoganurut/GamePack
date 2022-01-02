using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    [DisallowMultipleComponent]
    public class PolyLinePathPoint: MonoBehaviour
    {
        [SerializeField, Required] private Vector3 _Offset;
        
        // TODO: This might not be working at all cases. Test this, somehow. 
        public Vector3 Position => transform.parent ?
            transform.parent.TransformPoint( transform.localPosition) + transform.TransformDirection( _Offset) :    // This is because: Offset is in local space but position is in world space
            transform.position + _Offset;
    }
}
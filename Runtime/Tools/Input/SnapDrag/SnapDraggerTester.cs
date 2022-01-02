using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.SnapDrag
{
    public class SnapDraggerTester:MonoBehaviour
    {
        [SerializeField, Required] private Camera _Camera;
        [FormerlySerializedAs("DragObject")] [SerializeField, Required] private DragObject _DragObject;
        [FormerlySerializedAs("DropPoint")] [SerializeField, Required] private Transform _DropPoint;
        
        private void Start()
        {
            new SnapDragger().Initiate(_Camera, _DragObject, _DropPoint);
        }
    }
}
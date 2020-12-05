using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.SnapDrag
{
    public class SnapDraggerTester:MonoBehaviour
    {
        [SerializeField, Required] private Camera _Camera;
        [SerializeField, Required] private DragObject DragObject;
        [SerializeField, Required] private Transform DropPoint;
        
        private void Start()
        {
            new SnapDragger().Initiate(_Camera, DragObject, DropPoint);
        }
    }
}
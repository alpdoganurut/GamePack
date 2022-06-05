#if USING_SHAPES
using GamePack.DebugDrawSystem.DrawingMethods;
#endif
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.DebugDrawSystem.Helpers
{
    public class AxisDrawer: MonoBehaviour
    {
#if UNITY_EDITOR && USING_SHAPES
        [SerializeField, Required, InfoBox("Name is visible in Play Mode.")] private bool _ShowName;
        private void OnDrawGizmos()
        {
            Draw.Axis(Vector3.zero, transform);
            if(_ShowName) Draw.Text(Vector3.zero, name, localTransform: transform);
        }
#endif
    }
}
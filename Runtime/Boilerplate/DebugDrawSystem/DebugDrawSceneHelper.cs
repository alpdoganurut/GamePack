#if USING_SHAPES
using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem
{
    public class DebugDrawSceneHelper: MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            DebugDraw.OnHelperDrawGizmos();
        }
    }
}
#endif
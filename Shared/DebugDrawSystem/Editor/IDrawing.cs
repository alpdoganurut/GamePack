#if USING_SHAPES

using UnityEngine;

// Debug.DrawLine();    *
// Debug.DrawRay();     /
// Gizmos.DrawSphere(); *
// DrawArrow()  *
// DrawPoint()  *
// DrawText()   *
// DrawCircle() *
// DrawAxis()   *

// Gizmos.DrawWireCube();   --
// Gizmos.DrawWireSphere(); --
// Handles.DrawSolidRectangleWithOutline(); --
// Gizmos.DrawCube();
// DrawPolyLine()
// DrawRect(rect, pos, Vector3 orientation), DrawRect(rect, pos, Quaternion orientation)

namespace GamePack.DebugDrawSystem
{
    public interface IDrawing
    {
        internal void Draw(Camera camera);
    }
}

#endif
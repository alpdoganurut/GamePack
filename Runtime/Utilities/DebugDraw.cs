using UnityEngine;
using Color = UnityEngine.Color;

namespace GamePack.UnityUtilities
{
    public static class DebugDraw
    {
        public static void Arrow(Vector3 pos, Color color, float size = .2f)
        {
            Debug.DrawRay(pos, new Vector3(size, size * 2,  size), color);
            Debug.DrawRay(pos, new Vector3(-size,  size * 2, -size), color);
            Debug.DrawRay(pos, new Vector3(size,  size * 2, -size), color);
            Debug.DrawRay(pos, new Vector3(-size, size * 2,  size), color);
            
            Debug.DrawRay(pos, new Vector3(0,  size * 2, -size), color);
            Debug.DrawRay(pos, new Vector3(0,  size * 2, size), color);
            
            Debug.DrawRay(pos, new Vector3(size, size * 2,  0), color);
            Debug.DrawRay(pos, new Vector3(-size, size * 2,  0), color);
        }
        
        public static void Pointer(Vector3 pos, Color color, float size = .2f, float time = 0)
        {
            var halfSize = size / 2;
            Debug.DrawLine(pos - new Vector3(halfSize, 0, 0), pos + new Vector3(halfSize, 0, 0), color, time);
            Debug.DrawLine(pos - new Vector3(0, halfSize, 0), pos + new Vector3(0, halfSize, 0), color, time);
            Debug.DrawLine(pos - new Vector3(0, 0, halfSize), pos + new Vector3(0, 0, halfSize), color, time);
        }

        public static void LineArrow(Vector3 start, Vector3 end, Color color, float duration = 0f)
        {
            Debug.DrawLine(start, end, color);
            var directionRotation = Quaternion.LookRotation(end - start);
            var rotatedMatrix = Matrix4x4.Rotate(directionRotation);
            var arrowLine = new Vector3(0, 1, 0);

            var rotatedArrowLine = rotatedMatrix.MultiplyVector(arrowLine);
            
            Debug.DrawRay(end, rotatedArrowLine, color, duration);
        }

        public static void GizmosLineArrow(Vector3 start, Vector3 end, Color color, float size = .1f)
        {
            Gizmos.color = color;
            
            Gizmos.DrawLine(start, end);
            var directionRotation = Quaternion.LookRotation(end - start);
            var rotatedMatrix = Matrix4x4.Rotate(directionRotation);

            const float reverseArrowWidth = -1.6f;
            var arrowLine1 = new Vector3(0, 1, reverseArrowWidth) * size;
            var arrowLine2 = new Vector3(0, -1, reverseArrowWidth) * size;
            var arrowLine3 = new Vector3(1, 0, reverseArrowWidth) * size;
            var arrowLine4 = new Vector3(-1, 0, reverseArrowWidth) * size;

            var rotated1 = rotatedMatrix.MultiplyVector(arrowLine1);
            var rotated2 = rotatedMatrix.MultiplyVector(arrowLine2);
            var rotated3 = rotatedMatrix.MultiplyVector(arrowLine3);
            var rotated4 = rotatedMatrix.MultiplyVector(arrowLine4);
            
            Gizmos.DrawRay(end, rotated1);
            Gizmos.DrawRay(end, rotated2);
            Gizmos.DrawRay(end, rotated3);
            Gizmos.DrawRay(end, rotated4);
            
            Gizmos.DrawLine(end + rotated1, end + rotated3);
            Gizmos.DrawLine(end + rotated2, end + rotated3);
            Gizmos.DrawLine(end + rotated2, end + rotated4);
            Gizmos.DrawLine(end + rotated4, end + rotated1);
        }
        
        public static void GizmosRayArrow(Vector3 start, Vector3 direction, Color color, float size = .1f)
        {
            Gizmos.color = color;
            
            Gizmos.DrawRay(start, direction);
            
            var directionRotation = Quaternion.LookRotation(direction);
            var rotatedMatrix = Matrix4x4.Rotate(directionRotation);

            const float reverseArrowWidth = -1.6f;
            var arrowLine1 = new Vector3(0, 1, reverseArrowWidth) * size;
            var arrowLine2 = new Vector3(0, -1, reverseArrowWidth) * size;
            var arrowLine3 = new Vector3(1, 0, reverseArrowWidth) * size;
            var arrowLine4 = new Vector3(-1, 0, reverseArrowWidth) * size;

            var rotated1 = rotatedMatrix.MultiplyVector(arrowLine1);
            var rotated2 = rotatedMatrix.MultiplyVector(arrowLine2);
            var rotated3 = rotatedMatrix.MultiplyVector(arrowLine3);
            var rotated4 = rotatedMatrix.MultiplyVector(arrowLine4);

            var end = start + direction;
            Gizmos.DrawRay(end, rotated1);
            Gizmos.DrawRay(end, rotated2);
            Gizmos.DrawRay(end, rotated3);
            Gizmos.DrawRay(end, rotated4);
            
            Gizmos.DrawLine(end + rotated1, end + rotated3);
            Gizmos.DrawLine(end + rotated2, end + rotated3);
            Gizmos.DrawLine(end + rotated2, end + rotated4);
            Gizmos.DrawLine(end + rotated4, end + rotated1);
        }
        
        
    }
}
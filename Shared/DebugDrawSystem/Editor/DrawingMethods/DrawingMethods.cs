#if USING_SHAPES
using Shapes;
#endif

using System.Runtime.CompilerServices;
using GamePack.Logging;
using UnityEditor;
using UnityEngine;

namespace GamePack.DebugDrawSystem.DrawingMethods
{
    public static class Draw
    {
        public static void Line(Vector3 start, Vector3 end,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new LineDrawing(start, end, color, localTransform), duration, owner);
#elif UNITY_EDITOR
            Debug.DrawLine(start, end, color ?? Color.white, duration);
#endif
        }
        
        public static void PolyLine(Vector3[] positions,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new PolyLineDrawing(positions, color, localTransform), duration, owner);
#elif UNITY_EDITOR
            LogNotImplemented();
#endif
        }
        
        public static void Ray(Vector3 start, Vector3 ray,
            Color? color = null,
            float duration = -1,
            Object owner = null, 
            Transform localTransform = null)
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new LineDrawing(start, start + ray, color, localTransform), duration, owner);
#elif UNITY_EDITOR
            Debug.DrawLine(start, start + ray, color ?? Color.white, duration);
#endif
        }

        public static void Arrow(Vector3 start, Vector3 end,
            Color? color = null,
            float duration = -1, 
            Object owner = null, 
            Transform localTransform = null)
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new LineDrawing(start, end, color, true, localTransform), duration, owner);
#elif UNITY_EDITOR
            Debug.DrawLine(start, end, color ?? Color.white, duration);
#endif
        }

        public static void ArrowRay(
            Vector3 start, Vector3 ray,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new LineDrawing(start, start + ray, color, true, localTransform), duration, owner);
#elif UNITY_EDITOR
            Debug.DrawLine(start, start + ray, color ?? Color.white, duration);
#endif
        }

#if USING_SHAPES
        public static void Text(
            Vector3 pos, string text,
            Color? color = null,
            float fontSize = DrawInstructionDefaults.DefaultTextSize, 
            TextAlign textAlign = TextAlign.Center, 
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new TextDrawing(pos, text, color , fontSize, textAlign, localTransform), duration, owner);
        }
#endif

        public static void Point(
            Vector3 pos,
            float size = DrawInstructionDefaults.DefaultPointSize,
            float thickness = DrawInstructionDefaults.DefaultLineThickness,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null,
            [CallerMemberName]string memberName = "")
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new PointDrawing(pos, size, thickness, color, localTransform), duration, owner);
#elif UNITY_EDITOR
            var right = pos + new Vector3(size, 0, 0);
            var top = pos + new Vector3(0, size, 0); 
            var front = pos + new Vector3(0, 0, size);
            
            var left = pos + new Vector3(-size, 0, 0);
            var bottom = pos + new Vector3(0, -size, 0); 
            var back = pos + new Vector3(0, 0, -size);

            if (localTransform)
            {
                right = localTransform.TransformPoint(right);
                top = localTransform.TransformPoint(top);
                front = localTransform.TransformPoint(front);
                
                left = localTransform.TransformPoint(left);
                bottom = localTransform.TransformPoint(bottom);
                back = localTransform.TransformPoint(back);
            }
            
            color ??= DrawInstructionDefaults.DefaultColor;
            if(CheckMemberNameIsGizmos(memberName))
            {
                Gizmos.color = color.Value;
                Gizmos.DrawLine(top, bottom);
                Gizmos.DrawLine(left, right);
                Gizmos.DrawLine(front, back);
            }
            else
            {
                Debug.DrawLine(top, bottom, color.Value, duration);
                Debug.DrawLine(left, right, color.Value, duration);
                Debug.DrawLine(front, back, color.Value, duration);
            }
#endif
        }

        public static void Circle(
            Vector3 pos, float radius, Vector3 normal,
            float thickness = DrawInstructionDefaults.DefaultLineThickness,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new CircleDrawing(pos, radius, normal, thickness , color, localTransform), duration, owner);
#elif UNITY_EDITOR
            LogNotImplemented();
#endif
            
        }

#if USING_SHAPES
        public static void Rectangle(
            Vector3 pos, Vector2 size, Vector3 normal,
            RectPivot pivot = RectPivot.Corner,
            float thickness = DrawInstructionDefaults.DefaultLineThickness,
            float radius = 0, 
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new RectangleDrawing(
                    pos,
                    size,
                    normal, 
                    pivot,
                    thickness,
                    radius,
                    color,
                    localTransform),
                    duration,
                    owner);
        }
#endif

        public static void Sphere(
            Vector3 pos, 
            float radius = DrawInstructionDefaults.DefaultPointSize,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null,
            [CallerMemberName]string memberName = "")
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new SphereDrawing(pos, radius, color, localTransform), duration, owner);
#elif UNITY_EDITOR
            if(CheckMemberNameIsGizmos(memberName) )
            {
                Gizmos.color = color.Value;
                Gizmos.DrawSphere(pos, radius);
            }
            else
            {
                Handles.color = color.Value;
                Handles.SphereHandleCap(-1, pos, Quaternion.identity, radius, EventType.Ignore);
            }
#endif
        }

        private static bool CheckMemberNameIsGizmos(string memberName)
        {
            return memberName is "OnDrawGizmos" or "OnDrawGizmosSelected";
        }

        public static void Axis(Vector3 pos,
            Transform localTransform,
            float size = DrawInstructionDefaults.DefaultAxisSize)
        {
#if USING_SHAPES
            DebugDraw.NewDrawing(new AxisDrawing(pos, localTransform, size));
#elif UNITY_EDITOR
            LogNotImplemented();
#endif
        }
        
        private static void LogNotImplemented() => ManagedLog.Log($"Not implemented when not using Shapes.", ManagedLog.Type.Verbose);

#if !USING_SHAPES // Replacement Methods
        
        public static void Rectangle(
            Vector3 pos, Vector2 size, Vector3 normal,
            RectPivot pivot = RectPivot.Corner,
            float thickness = DrawInstructionDefaults.DefaultLineThickness,
            float radius = 0,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            LogNotImplemented();
        }
        
        public static void Text(
            Vector3 pos, string text,
            Color? color = null,
            float fontSize = DrawInstructionDefaults.DefaultTextSize,
            TextAlign textAlign = TextAlign.Center,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            LogNotImplemented();
        }
        
        public enum TextAlign {
            TopLeft,
            Top,
            TopRight,
            Left,
            Center,
            Right,
            BottomLeft,
            Bottom,
            BottomRight
        }
        
        public enum RectPivot {
            Corner,
            Center
        }
#endif
    }
}
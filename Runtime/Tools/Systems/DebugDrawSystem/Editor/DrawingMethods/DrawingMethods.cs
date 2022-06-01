#if USING_SHAPES

using GamePack.Utilities.DebugDrawSystem;
using Shapes;
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
            DebugDraw.NewDrawing(new LineDrawing(start, end, color, localTransform), duration, owner);
        }
        
        public static void PolyLine(Vector3[] positions,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new PolyLineDrawing(positions, color, localTransform), duration, owner);
        }
        
        public static void Ray(Vector3 start, Vector3 ray,
            Color? color = null,
            float duration = -1,
            Object owner = null, 
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new LineDrawing(start, start + ray, color, localTransform), duration, owner);
        }

        public static void Arrow(Vector3 start, Vector3 end,
            Color? color = null,
            float duration = -1, 
            Object owner = null, 
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new LineDrawing(start, end, color, true, localTransform), duration, owner);
        }

        public static void ArrowRay(
            Vector3 start, Vector3 ray,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new LineDrawing(start, start + ray, color, true, localTransform), duration, owner);
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
            DebugDraw.NewDrawing(new TextDrawing(pos, text, color , fontSize, textAlign, localTransform), duration, owner);
        }

        public static void Point(
            Vector3 pos,
            float size = DrawInstructionDefaults.DefaultPointSize,
            float thickness = DrawInstructionDefaults.DefaultLineThickness,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new PointDrawing(pos, size, thickness, color, localTransform), duration, owner);
        }

        public static void Circle(
            Vector3 pos, float radius, Vector3 normal,
            float thickness = DrawInstructionDefaults.DefaultLineThickness,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new CircleDrawing(pos, radius, normal, thickness , color, localTransform), duration, owner);
        }

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

        public static void Sphere(
            Vector3 pos, 
            float radius = DrawInstructionDefaults.DefaultPointSize,
            Color? color = null,
            float duration = -1,
            Object owner = null,
            Transform localTransform = null)
        {
            DebugDraw.NewDrawing(new SphereDrawing(pos, radius, color, localTransform), duration, owner);
        }

        public static void Axis(Vector3 pos,
            Transform localTransform,
            float size = DrawInstructionDefaults.DefaultAxisSize)
        {
            DebugDraw.NewDrawing(new AxisDrawing(pos, localTransform, size));
        }
    }
}

#endif
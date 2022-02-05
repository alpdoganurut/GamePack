using Shapes;
using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem.DrawingMethods
{
    public static class Draw
    {
        public static void Line(Vector3 start, Vector3 end, Color? color = null, float duration = -1)
        {
            DebugDraw.NewDrawing(new LineDrawing(start, end, color), duration);
        }

        public static void Arrow(Vector3 start, Vector3 end, Color? color = null,
            float size = DrawInstructionDefaults.DefaultArrowSize, float duration = -1)
        {
            DebugDraw.NewDrawing(new ArrowDrawing(start, end, color, size), duration);
        }

        public static void Text(Vector3 pos, string text, Color? color = null,
            float fontSize = DrawInstructionDefaults.DefaultTextSize, TextAlign textAlign = TextAlign.Center, float duration = -1)
        {
            DebugDraw.NewDrawing(new TextDrawing(pos, text, color , fontSize, textAlign), duration);
        }

        public static void Point(Vector3 pos, float size = DrawInstructionDefaults.DefaultPointSize,
            float thickness = DrawInstructionDefaults.DefaultLineThickness, Color? color = null, float duration = -1)
        {
            DebugDraw.NewDrawing(new PointDrawing(pos, size, thickness, color), duration);
        }

        public static void Circle(Vector3 pos, float radius, Vector3 normal, float thickness = DrawInstructionDefaults.DefaultLineThickness, Color? color = null, float duration = -1)
        {
            DebugDraw.NewDrawing(new CircleDrawing(pos, radius, normal, thickness , color), duration);
        }

        public static void Sphere(Vector3 pos, float radius = DrawInstructionDefaults.DefaultPointSize, Color? color = null, float duration = -1) 
        {
            DebugDraw.NewDrawing(new SphereDrawing(pos, radius, color), duration);
        }
    }
}
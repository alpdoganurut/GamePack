using Shapes;
using UnityEngine;

// Debug.DrawLine();    *
// Debug.DrawRay();     /
// Gizmos.DrawSphere(); *
// DrawArrow()  *
// DrawPoint()  *
// DrawText()   *
// DrawCircle() *

// Gizmos.DrawWireCube();   --
// Gizmos.DrawWireSphere(); --
// Handles.DrawSolidRectangleWithOutline(); --
// Gizmos.DrawCube();
// DrawPolyLine()
// DrawRect(rect, pos, Vector3 orientation), DrawRect(rect, pos, Quaternion orientation)

namespace GamePack.Utilities.DebugDrawSystem
{
    public interface IDrawing
    {
        internal void Draw();
    }
    
    internal readonly struct LineDrawing: IDrawing
    {
        private readonly Vector3 _start;
        private readonly Vector3 _end;
        private readonly Color _color;

        public LineDrawing(Vector3 start, Vector3 end, Color? color = null)
        {
            _start = start;
            _end = end;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
        }

        void IDrawing.Draw()
        {
            Draw.Line(_start, _end, DrawInstructionDefaults.DefaultLineThickness, _color);
        }
    }
    
    internal readonly struct ArrowDrawing: IDrawing
    {
        private readonly Vector3 _start;
        private readonly Vector3 _end;
        private readonly float _size;
        private readonly Color _color;

        public ArrowDrawing(Vector3 start, Vector3 end, Color? color = null, float size = DrawInstructionDefaults.DefaultArrowSize)
        {
            _start = start;
            _end = end;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
            _size = size;
        }

        void IDrawing.Draw()
        {
            Draw.Line(_start, _end, DrawInstructionDefaults.DefaultLineThickness, _color);
            Draw.Cone(_end, _end-_start, _size, _size * DrawInstructionDefaults.ArrowRadiusToLength, _color);
        }
    }

    internal readonly struct TextDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly string _text;
        private readonly Color _color;
        private readonly float _fontSize;
        private readonly TextAlign _textAlign;

        public TextDrawing(Vector3 pos, string text, Color? color = null, float fontSize = DrawInstructionDefaults.DefaultTextSize, TextAlign textAlign = TextAlign.Center)
        {
            _pos = pos;
            _text = text;
            _color = color ?? Color.white;
            _fontSize = fontSize;
            _textAlign = textAlign;
        }

        void IDrawing.Draw()
        {
            Draw.Text(_pos, _text, _textAlign, _fontSize, _color);
        }

    }

    internal readonly struct PointDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly Color _color;
        private readonly float _thickness;
        private readonly float _size;

        public PointDrawing(Vector3 pos, float size = DrawInstructionDefaults.DefaultPointSize, float thickness = DrawInstructionDefaults.DefaultLineThickness, Color? color = null)
        {
            _pos = pos;
            _size = size;
            _thickness = thickness;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
        }

        void IDrawing.Draw()
        {
            // const float offset = 1;
            var right = _pos + new Vector3(_size, 0, 0);
            var top = _pos + new Vector3(0, _size, 0); 
            var front = _pos + new Vector3(0, 0, _size);
            
            var left = _pos + new Vector3(-_size, 0, 0);
            var bottom = _pos + new Vector3(0, -_size, 0); 
            var back = _pos + new Vector3(0, 0, -_size);
            
            Draw.Line(top, bottom, _thickness, _color);
            Draw.Line(left, right, _thickness, _color);
            Draw.Line(front, back, _thickness, _color);
        }

    }

    internal readonly struct CircleDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly float _radius;
        private readonly Vector3 _normal;
        private readonly Color _color;
        private readonly float _thickness;

        public CircleDrawing(Vector3 pos, float radius, Vector3 normal, float thickness = DrawInstructionDefaults.DefaultLineThickness, Color? color = null)
        {
            _pos = pos;
            _radius = radius;
            _normal = normal;
            _thickness = thickness;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
        }

        void IDrawing.Draw()
        {
            Draw.Ring(_pos, Quaternion.LookRotation(_normal), radius: _radius, thickness: _thickness, color: _color);
        }
    }
    
    internal readonly struct SphereDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly float _radius;
        private readonly Color _color;

        public SphereDrawing(Vector3 pos, float radius = DrawInstructionDefaults.DefaultPointSize, Color? color = null)
        {
            _pos = pos;
            _radius = radius;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
        }

        void IDrawing.Draw()
        {
            Draw.Sphere(_pos, _radius, _color);
        }

    }
}
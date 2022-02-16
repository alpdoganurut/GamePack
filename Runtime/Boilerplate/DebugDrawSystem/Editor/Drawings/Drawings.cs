#if USING_SHAPES

using Shapes;
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

namespace GamePack.Utilities.DebugDrawSystem
{
    public interface IDrawing
    {
        internal void Draw(Camera camera);
    }
    
    internal readonly struct LineDrawing: IDrawing
    {
        private readonly Vector3 _start;
        private readonly Vector3 _end;
        private readonly Color _color;
        private readonly bool _drawArrow;
        private readonly Transform _localTransform;

        public LineDrawing(Vector3 start, Vector3 end, Color? color = null, bool drawArrow = false, Transform localTransform = null)
        {
            _start = start;
            _end = end;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
            _drawArrow = drawArrow;
            _localTransform = localTransform;
        }

        void IDrawing.Draw(Camera camera)
        {
            var start = _localTransform ? _localTransform.TransformPoint(_start) : _start;
            var end = _localTransform ? _localTransform.TransformPoint(_end) : _end;
            
            Draw.Line(start, end, DrawInstructionDefaults.DefaultLineThickness, _color);
            if(_drawArrow)
                Draw.Cone(_end, _end-_start, DrawInstructionDefaults.DefaultLineThickness, 
                    DrawInstructionDefaults.DefaultLineThickness * DrawInstructionDefaults.ArrowRadiusToLength, _color);
        }
    }

    internal readonly struct TextDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly string _text;
        
        private readonly Color _color;
        private readonly float _fontSize;
        private readonly TextAlign _textAlign;
        private readonly Transform _localTransform;
        private readonly bool _lookAtCamera;

        public TextDrawing(Vector3 pos, string text, Color? color = null,
            float fontSize = DrawInstructionDefaults.DefaultTextSize, TextAlign textAlign = TextAlign.Center,
            Transform localTransform = null, bool lookAtCamera = true)
        {
            _pos = pos;
            _text = text;
            
            _color = color ?? Color.white;
            _fontSize = fontSize;
            _textAlign = textAlign;
            
            _localTransform = localTransform;
            _lookAtCamera = lookAtCamera;
        }

        void IDrawing.Draw(Camera camera)
        {
            if(!_lookAtCamera)
            {
                Draw.Text(_pos, _text, _textAlign, _fontSize, _color);
                return;
            }
            
            // Look At Camera Logic
            var translationColumn = _localTransform
                ? _localTransform.localToWorldMatrix.GetColumn(3)
                : new Vector4(0, 0, 0, 1);
            
            var rotationMatrix = camera.transform.localToWorldMatrix;
            rotationMatrix.SetColumn(3, translationColumn);

            var pos = rotationMatrix.inverse.MultiplyPoint(_localTransform
                ? _localTransform.TransformPoint(_pos)
                : _pos);
            Draw.Matrix = rotationMatrix;
            
            Draw.Text(pos, _text, _textAlign, _fontSize, _color);
            Draw.ResetMatrix();
        }

    }

    internal readonly struct PointDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly Color _color;
        private readonly float _thickness;
        private readonly float _size;
        private readonly Transform _localTransform;

        public PointDrawing(Vector3 pos, float size = DrawInstructionDefaults.DefaultPointSize,
            float thickness = DrawInstructionDefaults.DefaultLineThickness, Color? color = null,
            Transform localTransform = null)
        {
            _pos = pos;
            _size = size;
            _thickness = thickness;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
            _localTransform = localTransform;
        }

        void IDrawing.Draw(Camera camera)
        {
            // const float offset = 1;
            var right = _pos + new Vector3(_size, 0, 0);
            var top = _pos + new Vector3(0, _size, 0); 
            var front = _pos + new Vector3(0, 0, _size);
            
            var left = _pos + new Vector3(-_size, 0, 0);
            var bottom = _pos + new Vector3(0, -_size, 0); 
            var back = _pos + new Vector3(0, 0, -_size);

            if (_localTransform)
            {
                right = _localTransform.TransformPoint(right);
                top = _localTransform.TransformPoint(top);
                front = _localTransform.TransformPoint(front);
                
                left = _localTransform.TransformPoint(left);
                bottom = _localTransform.TransformPoint(bottom);
                back = _localTransform.TransformPoint(back);
            }
            
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
        private readonly Transform _localTransform;

        public CircleDrawing(Vector3 pos, float radius, Vector3 normal,
            float thickness = DrawInstructionDefaults.DefaultLineThickness, Color? color = null,
            Transform localTransform = null)
        {
            _pos = pos;
            _radius = radius;
            _normal = normal;
            _thickness = thickness;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
            _localTransform = localTransform;
        }

        void IDrawing.Draw(Camera camera)
        {
            var pos = _localTransform ? _localTransform.TransformPoint(_pos) : _pos;
            Draw.Ring(pos, Quaternion.LookRotation(_normal), radius: _radius, thickness: _thickness, colors: _color);
        }
    }
    
    internal readonly struct SphereDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly float _radius;
        private readonly Color _color;
        private readonly Transform _localTransform;

        public SphereDrawing(Vector3 pos, float radius = DrawInstructionDefaults.DefaultPointSize, Color? color = null,
            Transform localTransform = null)
        {
            _pos = pos;
            _radius = radius;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
            _localTransform = localTransform;
        }

        void IDrawing.Draw(Camera camera)
        {
            var pos = _localTransform ? _localTransform.TransformPoint(_pos) : _pos;
            Draw.Sphere(pos, _radius, _color);
        }
    }
    
    internal readonly struct AxisDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly float _size;
        private readonly Transform _localTransform;

        public AxisDrawing(Vector3 pos,
            Transform localTransform, float size = DrawInstructionDefaults.DefaultAxisSize)
        {
            _pos = pos;
            _size = size;
            _localTransform = localTransform;
        }

        void IDrawing.Draw(Camera camera)
        {
            var right = _pos + new Vector3(_size, 0, 0);
            var top = _pos + new Vector3(0, _size, 0); 
            var front = _pos + new Vector3(0, 0, _size);

            var center = _pos;

            if (_localTransform)
            {
                right = _localTransform.TransformPoint(right);
                top = _localTransform.TransformPoint(top);
                front = _localTransform.TransformPoint(front);
                
                center = _localTransform.TransformPoint(center);
            }
            
            Draw.Line(center, top, DrawInstructionDefaults.DefaultLineThickness, Colors.Green);
            Draw.Line(center, right, DrawInstructionDefaults.DefaultLineThickness, Colors.Red);
            Draw.Line(center, front, DrawInstructionDefaults.DefaultLineThickness, Colors.Blue);
        }
    }
    
    
}

#endif
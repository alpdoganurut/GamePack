#if USING_SHAPES
using Shapes;
using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem
{
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
}
#endif
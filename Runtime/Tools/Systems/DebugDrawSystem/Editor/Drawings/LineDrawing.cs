using Shapes;
using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem
{
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
                Draw.Cone(_end, _end-_start, DrawInstructionDefaults.DefaultArrowRadius, 
                    DrawInstructionDefaults.DefaultArrowRadius * DrawInstructionDefaults.ArrowRadiusToLength, _color);
        }
    }
}
using Shapes;
using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem
{
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
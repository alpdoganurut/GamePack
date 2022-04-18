using Shapes;
using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem
{
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
}
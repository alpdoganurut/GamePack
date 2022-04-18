using Shapes;
using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem
{
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
}
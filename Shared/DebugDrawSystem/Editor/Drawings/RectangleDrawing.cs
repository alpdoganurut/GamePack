#if USING_SHAPES
using Shapes;
using UnityEngine;

namespace GamePack.DebugDrawSystem
{
    internal readonly struct RectangleDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly Vector3 _normal;
        private readonly Color _color;
        private readonly float _thickness;
        private readonly Transform _localTransform;
        private readonly Vector2 _size;
        private readonly RectPivot _pivot;
        private readonly float _cornerRadius;

        public RectangleDrawing(
            Vector3 pos, Vector2 size, Vector3 normal,
            RectPivot pivot = RectPivot.Corner, 
            float thickness = DrawInstructionDefaults.DefaultLineThickness,
            float cornerRadius = 0,
            Color? color = null,
            Transform localTransform = null)
        {
            _pos = pos;
            _size = size;
            _normal = normal;
            _thickness = thickness;
            _cornerRadius = cornerRadius;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
            _localTransform = localTransform;
            _pivot = pivot;
        }

        void IDrawing.Draw(Camera camera)
        {
            var pos = _localTransform ? _localTransform.TransformPoint(_pos) : _pos;
            Draw.RectangleBorder(pos, Quaternion.LookRotation(_normal), _size, _pivot, _thickness, _cornerRadius, _color);
        }
    }
}
#endif
#if USING_SHAPES
using Shapes;
using UnityEngine;

namespace GamePack.DebugDrawSystem
{
    internal readonly struct PolyLineDrawing: IDrawing
    {
        private readonly Vector3[] _positions;
        private readonly Color _color;
        private readonly Transform _localTransform;

        public PolyLineDrawing(Vector3[] positions, Color? color = null, Transform localTransform = null)
        {
            _positions = positions;
            _color = color ?? DrawInstructionDefaults.DefaultColor;
            _localTransform = localTransform;
        }

        void IDrawing.Draw(Camera camera)
        {
            if(_positions == null || _positions.Length == 0) return;
            
            for (var i = 0; i < _positions.Length - 1; i++)
            {
                var pos = _positions[i];
                var nextPos = _positions[i + 1];
                
                var start = _localTransform ? _localTransform.TransformPoint(pos) : pos;
                var end = _localTransform ? _localTransform.TransformPoint(nextPos) : nextPos;
            
                Draw.Line(start, end, DrawInstructionDefaults.DefaultLineThickness, _color);
            }
        }
    }
}
#endif
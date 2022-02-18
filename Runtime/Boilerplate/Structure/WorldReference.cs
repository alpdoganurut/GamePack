using System;
#if USING_SHAPES
using GamePack.Utilities.DebugDrawSystem.DrawingMethods;
#endif
using UnityEngine;

namespace GamePack.Boilerplate.Structure
{
    public class WorldReference: StructureMonoBehaviourBase
    {
        private Transform _transform;

        public Vector3 Position
        {
            get
            {
                if(!_transform) return Vector3.zero;
                return _transform.position;
            }
        }

        private void Awake()
        {
            _transform = transform;
        }

#if USING_SHAPES
        private void OnDrawGizmos()
        {
            Draw.Axis(Vector3.zero, transform);
            Draw.Text(Position, $"{name}", fontSize: 3, color: Colors.Yellow);
        }
#endif

    }
}
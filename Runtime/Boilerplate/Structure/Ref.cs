using System;
// #if USING_SHAPES
using GamePack.DebugDrawSystem.DrawingMethods;
// #endif
using UnityEngine;

namespace GamePack.Boilerplate.Structure
{
    public class Ref: StructureMonoBehaviourBase
    {
        private const string Prefix = "[Ref]";

        private Transform _transform;

        // ReSharper disable once MemberCanBePrivate.Global
        public Vector3 Position
        {
            get
            {
                if(!_transform) return Internal_Transform.position;
                return _transform.position;
            }
        }

        private void Awake()
        {
            _transform = Internal_Transform;
        }

        private void OnValidate()
        {
            _transform = Internal_Transform;
            
            // Add prefix
            if (!name.Contains(Prefix)) name = $"{Prefix} {name}";
        }

// #if USING_SHAPES
        private void OnDrawGizmos()
        {
            Draw.Axis(Vector3.zero, Internal_Transform);
            Draw.Text(Position - new Vector3(0, 0.2f, 0), $"{name}", color: Colors.Yellow);
        }
// #endif

    }
}
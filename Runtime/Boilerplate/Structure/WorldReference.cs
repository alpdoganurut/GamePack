using System;
using GamePack.Utilities.DebugDrawSystem.DrawingMethods;
using UnityEngine;

namespace GamePack.Boilerplate.Structure
{
    public class WorldReference: StructureMonoBehaviourBase
    {
        private Transform _transform;

        public Vector3 Position => _transform.position;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnDrawGizmos()
        {
            Draw.Axis(Vector3.zero, transform);
        }

    }
}
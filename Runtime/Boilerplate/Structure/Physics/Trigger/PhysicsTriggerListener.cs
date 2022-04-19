using System;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    public abstract class PhysicsTriggerListenerBase
    {
        public Type Type { get; protected set; }
        public PhysicsEventPhase? Phase { get; protected set; }

        public abstract void Invoke(Component component, Collider other);
    }

    public class PhysicsTriggerListener<T> : PhysicsTriggerListenerBase where T: Component
    {
        public delegate void PhysicsTriggerEvent(T component, Collider other);

        private readonly PhysicsTriggerEvent _event;

        public PhysicsTriggerListener(Type type, PhysicsEventPhase? phase, PhysicsTriggerEvent physicsTriggerEvent)
        {
            Type = type;
            Phase = phase;
            _event = physicsTriggerEvent;
        }

        public override void Invoke(Component component, Collider other) => _event?.Invoke(component as T, other);
    }
}
using System;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    public delegate void PhysicsTriggerEvent<T>(T component, Collider other);

    internal abstract class PhysicsTriggerListenerBase
    {
        internal  Type Type { get; private protected set; }
        internal  PhysicsEventPhase? Phase { get; private protected set; }

        internal  abstract void Invoke(Component component, Collider other);
    }

    internal sealed class PhysicsTriggerListener<T> : PhysicsTriggerListenerBase where T: Component
    {

        private readonly PhysicsTriggerEvent<T> _event;

        internal PhysicsTriggerListener(Type type, PhysicsEventPhase? phase, PhysicsTriggerEvent<T> physicsTriggerEvent)
        {
            Type = type;
            Phase = phase;
            _event = physicsTriggerEvent;
        }

        internal  override void Invoke(Component component, Collider other) => _event?.Invoke(component as T, other);
    }
}
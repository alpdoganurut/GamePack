using System;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    public abstract class PhysicsTriggerListenerBase
    {
        public Type Type { get; set; }
        public PhysicsEventPhase? Phase { get; set; }

        public abstract void Invoke(Component component, Collider other);
    }

    public class PhysicsTriggerListener<T> : PhysicsTriggerListenerBase where T: Component
    {
        public delegate void PhysicsTriggerEvent(T component, Collider other);

        public event PhysicsTriggerEvent Event;

        public override void Invoke(Component component, Collider other) => Event?.Invoke(component as T, other);
    }
}
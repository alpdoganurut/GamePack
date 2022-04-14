using System;
using UnityEngine;
using Component = UnityEngine.Component;

namespace GamePack.Boilerplate.Structure.Physics.Trigger
{
    [RequireComponent(typeof(PhysicsObject))]
    public abstract class PhysicsTrigger: PhysicsTriggerBase    // TODO: Rename
    {
        public delegate void EventDelegate(Component component, Collider other);
        public event EventDelegate DidEnter;
        
        protected abstract Type[] Types { get;}

        protected override void PhysicsObjectOnDidTrigger(PhysicsObject.PhysicsEventPhase phase, Collider other)
        {
            if (phase != PhysicsObject.PhysicsEventPhase.Enter) return;
            
            foreach (var type in Types)
            {
                var validComponents = other.gameObject.GetComponents(type);
                if (validComponents.Length <= 0) continue;

                foreach (var component in validComponents)
                {
                    Log($"TriggerDidEnter, other:{other}, component: {component}");
                    DidEnter?.Invoke(component, other);
                }
            }
        }
    }
}
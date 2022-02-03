using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics.Trigger
{
    public class GenericPhysicsTrigger<T> : PhysicsTriggerBase where T: Component
    {
        public delegate void EventDelegate(T component);
        public event EventDelegate DidEnter;

        protected override void PhysicsObjectOnDidTrigger(PhysicsObject.PhysicsEventPhase phase, Collider other)
        {
            if (phase != PhysicsObject.PhysicsEventPhase.Enter) return;
            
            var component = other.gameObject.GetComponent<T>();
                
            if(component)
            {
                Log($"TriggerDidEnter, other:{other}, component: {component}");
                DidEnter?.Invoke(component);
            }
        }
    }
}
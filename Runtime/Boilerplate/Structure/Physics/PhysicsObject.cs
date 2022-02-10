using GamePack.Logging;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    public class PhysicsObject: MonoBehaviour
    {
        private Rigidbody _rigidbody;

        public enum PhysicsEventPhase
        {
            Enter, Stay, Exit
        }

        internal delegate void PhysicsCollisionEvent(PhysicsEventPhase eventPhase, Collision collision);
        internal delegate void PhysicsTriggerEvent(PhysicsEventPhase eventPhase, Collider collider);

        internal event PhysicsCollisionEvent DidCollide;
        internal event PhysicsTriggerEvent DidTrigger;
        
        private void OnCollisionEnter(Collision collision)
        {
            DidCollide?.Invoke(PhysicsEventPhase.Enter, collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            DidCollide?.Invoke(PhysicsEventPhase.Stay, collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            DidCollide?.Invoke(PhysicsEventPhase.Exit, collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            DidTrigger?.Invoke(PhysicsEventPhase.Enter, other);
        }

        private void OnTriggerStay(Collider other)
        {
            DidTrigger?.Invoke(PhysicsEventPhase.Stay, other);
        }

        private void OnTriggerExit(Collider other)
        {
            DidTrigger?.Invoke(PhysicsEventPhase.Exit, other);
        }

        private void OnValidate()
        {
            if (!_rigidbody) _rigidbody = GetComponent<Rigidbody>();
            if (!_rigidbody)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
                _rigidbody.isKinematic = true;
                ManagedLog.Log($"Added Rigidbody to {name}.", ManagedLog.Type.Info);
            }
        }
    }
}
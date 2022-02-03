using System;
using GamePack.Logging;
using Sirenix.OdinInspector;
using UnityEngine;
using Component = UnityEngine.Component;

namespace GamePack.Boilerplate.Structure.Physics
{
    [RequireComponent(typeof(PhysicsObject))]
    public abstract class PhysicsTrigger: PhysicsTriggerBase
    {
        public delegate void EventDelegate(Component component);
        public event EventDelegate DidEnter;

        // [SerializeField, ReadOnly] private PhysicsObject _PhysicsObject;
        
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
                    
                    DidEnter?.Invoke(component);
                }
            }
        }
        
        /*private void Awake()
        {
            _PhysicsObject.DidTrigger += PhysicsObjectOnDidTrigger;
        }*/

        /*private void PhysicsObjectOnDidTrigger(PhysicsObject.PhysicsEventPhase phase, Collider other)
        {
            if (phase != PhysicsObject.PhysicsEventPhase.Enter) return;
            
            foreach (var type in Types)
            {
                var validComponents = other.gameObject.GetComponents(type);
                if (validComponents.Length <= 0) continue;

                foreach (var component in validComponents)
                {
                    Log($"TriggerDidEnter, other:{other}, component: {component}");
                    
                    DidEnter?.Invoke(component);
                }
            }
        }*/
        
        /*
        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (!col)
            {
                ManagedLog.LogError($"{InternalGameObject.name} doesn't have any Collider component!");
            }
            if (col && !col.isTrigger
                    #region Editor
#if UNITY_EDITOR
                    && UnityEditor.EditorUtility.DisplayDialog("TriggerEvent","TriggerEvent collider is not set as trigger, set it now?", "OK", "Cancel") 
#endif
                #endregion
               )
            {
                col.isTrigger = true;
            }

            if (!_PhysicsObject)
                _PhysicsObject = GetComponent<PhysicsObject>();
        }*/
    }
}
using System;
using System.Collections.Generic;
using GamePack.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    public class PhysicsTrigger: StructureMonoBehaviourBase
    {
        [SerializeField, ReadOnly] private PhysicsObject _PhysicsObject;

        private readonly List<PhysicsTriggerListenerBase> _listeners = new();
        
        private void Awake() => _PhysicsObject.DidTrigger += PhysicsObjectOnDidTrigger;

        public void ListenFor<T>(PhysicsTriggerListener<T>.PhysicsTriggerEvent action, PhysicsEventPhase? phase = null) where T : Component
        {
            var listener = new PhysicsTriggerListener<T>();
            listener.Event += action;
            listener.Type = typeof(T);
            listener.Phase = phase;
            _listeners.Add(listener);
        }
        
        private void PhysicsObjectOnDidTrigger(PhysicsEventPhase phase, Collider other)
        {
            foreach (var listener in _listeners)
            {
                if(listener.Phase.HasValue && listener.Phase != phase) continue;
                
                var component = other.gameObject.GetComponent(listener.Type);
                if (component) listener.Invoke(component, other);
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (!col)
            {
                ManagedLog.LogError($"{Internal_GameObject.name} doesn't have any Collider component!", gameObject);
            }
            if (col && !col.isTrigger
                    #region Editor
                    && UnityEditor.EditorUtility.DisplayDialog("TriggerEvent","TriggerEvent collider is not set as trigger, set it now?", "OK", "Cancel") 
                #endregion
               )
            {
                col.isTrigger = true;
            }

            if (!_PhysicsObject)
                _PhysicsObject = GetComponent<PhysicsObject>();
            if (!_PhysicsObject)
                _PhysicsObject = Internal_GameObject.AddComponent<PhysicsObject>();
        }
    }
#endif

}
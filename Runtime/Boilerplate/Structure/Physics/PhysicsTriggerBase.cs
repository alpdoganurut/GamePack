using GamePack.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    public abstract class PhysicsTriggerBase: StructureMonoBehaviourBase
    {
        [SerializeField, ReadOnly] private PhysicsObject _PhysicsObject;

        
        private void Awake()
        {
            _PhysicsObject.DidTrigger += PhysicsObjectOnDidTrigger;
        }

        protected abstract void PhysicsObjectOnDidTrigger(PhysicsObject.PhysicsEventPhase phase, Collider other);
        
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
        }
    }
}
using GamePack.Logging;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamePack.Boilerplate.Structure
{
    public abstract class StructureMonoBehaviourBase: MonoBehaviour
    {
        #region Access Restriction

        private protected GameObject Internal_GameObject => base.gameObject;
        
        // ReSharper disable once InconsistentNaming
        public new GameObject gameObject
        {
            get
            {
                Debug.LogError($"Shouldn't access 'gameObject' of {nameof(ControllerBase)}");
                return base.gameObject;
            }
        }

        // ReSharper disable once InconsistentNaming
        public new bool enabled
        {
            get
            {
                Debug.LogError($"Shouldn't access 'gameObject' of {nameof(ControllerBase)}");
                return base.enabled;
            }
            set
            {
                Debug.LogError($"Shouldn't access 'gameObject' of {nameof(ControllerBase)}");
                base.enabled = value;
            }
        } 

        #endregion

        protected static void Log(object obj, ManagedLog.Type type = ManagedLog.Type.Default, Object context = null)
        {
            ManagedLog.Log(obj, type, context);
        }

        public new static T Instantiate<T>(T original) where T: Object
        {
           var n = MonoBehaviour.Instantiate(original);
           StructureManager.RegisterViewOrController(n);
           return n;
        }
        
        public new static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T: Object
        {
            var n = MonoBehaviour.Instantiate(original, position, rotation);
            StructureManager.RegisterViewOrController(n);

            return n;
        }
        
        // TODO: Add other Instantiation methods 
    }
}
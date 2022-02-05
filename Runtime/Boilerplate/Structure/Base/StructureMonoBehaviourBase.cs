using GamePack.Logging;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamePack.Boilerplate.Structure
{
    public abstract class StructureMonoBehaviourBase: MonoBehaviour
    {
        #region Access Restriction

        private protected GameObject InternalGameObject => base.gameObject;
        
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

        protected static void Log(object obj, [CanBeNull] ManagedLog.Type? type = null, Object context = null)
        {
            ManagedLog.Log(obj, type, context);
        }
        
    }
}
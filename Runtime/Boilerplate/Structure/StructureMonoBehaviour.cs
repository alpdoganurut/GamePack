using UnityEngine;
using UnityEngine.Assertions;

namespace Boilerplate.Structure
{
    public class StructureMonoBehaviour: MonoBehaviour
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
    }
}
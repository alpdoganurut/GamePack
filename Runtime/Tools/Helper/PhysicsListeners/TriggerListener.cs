#region Development
#if UNITY_EDITOR
using UnityEditor; 
#endif
#endregion

using System;
using UnityEngine;

namespace GamePack.Tools.Helper.PhysicsListeners
{
    [Obsolete]
    public class TriggerListener: MonoBehaviour
    {
        public event Action<Collider> Enter;
        public event Action<Collider> Stay;
        public event Action<Collider> Exit;
        
        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col && !col.isTrigger
                    #region Development
#if UNITY_EDITOR
                    && EditorUtility.DisplayDialog("TriggerEvent","TriggerEvent collider is not set as trigger, set it now?", "OK", "Cancel") 
#endif
                #endregion
                )
            {
                col.isTrigger = true;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Enter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            Stay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Exit?.Invoke(other);
        }
    }
}
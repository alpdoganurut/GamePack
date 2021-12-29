#region Development
#if UNITY_EDITOR
using UnityEditor; 
#endif
#endregion

using System;
using UnityEngine;

namespace GamePack.Tools.Helper.CollisionDetection
{
    public class EventCollider: MonoBehaviour
    {
        public event Action<Collision> Enter;
        public event Action<Collision> Stay;
        public event Action<Collision> Exit;

        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            
            if (col && col.isTrigger
                    #region Development
#if UNITY_EDITOR
                    && EditorUtility.DisplayDialog("ColliderEvent","ColliderEvent collider is set as trigger, unset it now?", "OK", "Cancel") 
#endif
                #endregion
            )
            {
                col.isTrigger = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Enter?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            Stay?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            Exit?.Invoke(collision);
        }
    }
}
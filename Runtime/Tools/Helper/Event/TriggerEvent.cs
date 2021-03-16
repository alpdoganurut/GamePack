using System;
using UnityEngine;

namespace GamePack
{
    
    public class TriggerEvent: MonoBehaviour
    {
        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerExit;
        
        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(other);
        }
    }
}
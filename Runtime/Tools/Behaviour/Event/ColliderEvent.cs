using System;
using UnityEngine;

namespace GamePack
{
    public class ColliderEvent: MonoBehaviour
    {
        public event Action<Collision> Enter;
        public event Action<Collision> Stay;
        public event Action<Collision> Exit;

        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col) col.isTrigger = false;
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
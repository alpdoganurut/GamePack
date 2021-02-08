using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    public class FixedRotation: MonoBehaviour
    {
        [SerializeField, ReadOnly] private Quaternion _WorldRotation;

        private void OnValidate()
        {
            _WorldRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            transform.rotation = _WorldRotation;
        }
    }
}
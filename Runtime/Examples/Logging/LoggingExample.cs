using System;
using GamePack.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Examples.Logging
{
    public class LoggingExample: MonoBehaviour
    {
        [SerializeField] private float _Speed = 1;
        [SerializeField] private float _LimitZ = 5;

        [Button]
        private void LogInWorld(string msg = "Hello", Color? color = null)
        {
            WorldLog.Log($"{msg} At Pos", pos: Vector3.zero, color: color);
            WorldLog.Log($"{msg} At Transform", transform: transform, color: color);
        }

        private void Update()
        {
            transform.position += Vector3.forward * _Speed * Time.deltaTime;
            if (transform.position.z > _LimitZ)
            {
                transform.position = Vector3.zero;
            }
        }
    }
}
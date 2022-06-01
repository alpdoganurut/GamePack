#if USING_SHAPES
using GamePack.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples.Logging
{
    public class LoggingExample: MonoBehaviour
    {
        [SerializeField] private float _Speed = 1;
        [SerializeField] private float _LimitZ = 5;

        [Button]
        private void LogInWorld(string msg = "Hello", Color? color = null)
        {
            WorldLog.Log($"{msg} At Pos", pos: Vector3.zero, color: color);
            WorldLog.Log($"{msg} At Transform", localTransform: transform, color: color);
        }

        [Button]
        private void LogOnScreen(string msg = "On Screen Log")
        {
            WorldLog.OnScreen(msg);
        }

        private void Update()
        {
            transform.position += Vector3.forward * _Speed * Time.deltaTime;
            if (transform.position.z > _LimitZ)
            {
                transform.position = Vector3.zero;
            }
        }

        [Button]
        private void LogMethodTest()
        {
            ManagedLog.LogMethod();
        }
    }
}
#endif
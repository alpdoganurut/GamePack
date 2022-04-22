using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Behaviour
{
    /// <summary>
    /// Basic script to make something look at something constantly.
    /// </summary>
    public class LookAt: MonoBehaviour
    {
        [SerializeField, Required] private Transform _Target;

        private void Update()
        {
            transform.LookAt(_Target);
        }
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Behaviour
{
    /// <summary>
    /// Very basic script to make something look something constantly.
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
using System;
using UnityEngine;

namespace Editor.Utilities
{
    public class EditorUtilitiesHelper:MonoBehaviour
    {
        public event Action<bool> OnUpdate;
        
        private void Update()
        {
            OnUpdate?.Invoke(Application.isPlaying);
        }
    }
}
using System;
using UnityEngine;

namespace GamePack.Utilities
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
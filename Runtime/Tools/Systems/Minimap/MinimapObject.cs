using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Minimap
{
    public class MinimapObject: MonoBehaviour
    {
        [SerializeField, Required] private int _MinimapId;
        // [SerializeField] public int _LayerIndex;
        private bool _isQuitting;

        public void OnEnable()
        {
            MinimapBase.GetById(_MinimapId)?.AddMapObject(this);
        }

        private void OnDisable()
        {
            if(_isQuitting) return;
            MinimapBase.GetById(_MinimapId)?.RemoveMapObject(this);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}
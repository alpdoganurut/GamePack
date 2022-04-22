using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Minimap
{
    public class MinimapObject: MonoBehaviour
    {
        [SerializeField, Required] private int _MinimapId;
        private bool _isQuitting;
        private bool _addedToMinimap;

        private void OnEnable()
        {
            AddToMinimap();
        }

        public void AddToMinimap()
        {
            if(_addedToMinimap) return;
            
            var minimapBase = MinimapBase.GetById(_MinimapId);
            if(minimapBase)
            {
                _addedToMinimap = true;
                minimapBase.AddMapObject(this);
            }
            
        }

        private void OnDisable()
        {
            if(_isQuitting) return;
            MinimapBase.GetById(_MinimapId)?.RemoveMapObject(this);
            _addedToMinimap = false;
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}
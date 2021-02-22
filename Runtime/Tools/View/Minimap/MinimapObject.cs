using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Minimap
{
    public class MinimapObject: MonoBehaviour
    {
        [SerializeField, Required] private int _MinimapId;
        [SerializeField, Required] private MinimapObjectType _ObjectType;
        
        public MinimapObjectType Type => _ObjectType;
        
        private void OnEnable()
        {
            MinimapBase.GetById(_MinimapId).AddMapObject(this);
        }

        private void OnDisable()
        {
            MinimapBase.GetById(_MinimapId).RemoveMapObject(this);
        }
    }
}
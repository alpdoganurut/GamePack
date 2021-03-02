using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Minimap
{
    public class MinimapObject: MonoBehaviour
    {
        [SerializeField, Required] private int _MinimapId;
        [SerializeField] private bool _PreLoaded;

        // [SerializeField, Required] private MinimapObjectType _ObjectType;
        
        // public MinimapObjectType Type => _ObjectType;


        private void Start()
        {
            if(_PreLoaded) OnEnable();
        }

        private void OnEnable()
        {
                MinimapBase.GetById(_MinimapId)?.AddMapObject(this);
            // LeanTween.delayedCall(.1f, () =>
            // {

            // });
        }

        private void OnDisable()
        {
            MinimapBase.GetById(_MinimapId)?.RemoveMapObject(this);
        }
    }
}
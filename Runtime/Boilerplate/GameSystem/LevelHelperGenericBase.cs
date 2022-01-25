using Boilerplate.GameSystem;
using Boilerplate.Structure;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HexGames
{
    public abstract class LevelHelperGenericBase<TLevelInitData>: LevelHelperBase where TLevelInitData: LevelInitDataBase
    {
        [ShowInInspector, ReadOnly] private ControllerGenericBase<TLevelInitData>[] _controllers;

        internal ControllerGenericBase<TLevelInitData>[] Controllers => _controllers;

        protected virtual void EditorAutoFill() {}

        private void OnValidate()
        {
            if(Application.isPlaying) return;
            
            EditorAutoFill();
            _controllers = FindAllObjects.InScene<ControllerGenericBase<TLevelInitData>>().ToArray();
        }
    }
}
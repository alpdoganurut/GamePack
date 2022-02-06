using GamePack.Boilerplate.Structure;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Boilerplate.Main
{
    public abstract class LevelHelperGenericBase<TLevelInitData>: LevelHelperBase where TLevelInitData: LevelInitDataBase
    {
        // [ShowInInspector, ReadOnly] private ControllerGenericBase<TLevelInitData>[] _controllers;

        // internal ControllerGenericBase<TLevelInitData>[] Controllers => _controllers;
    }
}
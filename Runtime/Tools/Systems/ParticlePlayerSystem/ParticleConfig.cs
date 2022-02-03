using System;
using GamePack.Poolable;
using Sirenix.OdinInspector;

// ReSharper disable InconsistentNaming
namespace GamePack.Tools.Systems.ParticlePlayerSystem
{
    [Serializable]
    public struct ParticleConfig
    {
        [AssetsOnly] public PoolableParticle Prefab;
        [AssetsOnly] public int PrefillCount;
        public string Id;
    }
}
using System;
using GamePack.PoolingSystem;
using Sirenix.OdinInspector;

// ReSharper disable InconsistentNaming
namespace GamePack.ParticlePlayerSystem
{
    [Serializable]
    public struct ParticleConfig
    {
        [AssetsOnly] public PoolableParticle Prefab;
        [AssetsOnly] public int PrefillCount;
        public string Id;
    }
}
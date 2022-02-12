using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.ParticlePlayerSystem
{
    // [CreateAssetMenu(fileName = "Particle Player Config", menuName = "GamePack/ParticlePlayerConfig", order = 0)]
    public class ParticlePlayerConfig: MonoBehaviour
    {
        [SerializeField, Required] private ParticleConfig[] _Configs;

        public ParticleConfig[] Configs => _Configs;
    }
}
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.ParticlePlayerSystem
{
    public class ParticlePlayerSceneConfig: MonoBehaviour
    {
        [FormerlySerializedAs("_Configs")] [SerializeField, Required] private ParticleConfig[] _ParticleConfigs;

        public ParticleConfig[] ParticleConfigs => _ParticleConfigs;
    }
}
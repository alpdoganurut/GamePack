using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples.ParticlePlayer
{
    public class ParticlePlayerExample : MonoBehaviour
    {
        [SerializeField, Required] private string[] _Ids;

        private readonly int _hashedFirst = global::GamePack.Tools.Systems.ParticlePlayerSystem.ParticlePlayer.HashId("first");
        
        [Button, HideInEditorMode]
        private void SpawnParticle()
        {
            global::GamePack.Tools.Systems.ParticlePlayerSystem.ParticlePlayer.Play(_Ids.GetRandom(), Random.insideUnitSphere * 5);
            global::GamePack.Tools.Systems.ParticlePlayerSystem.ParticlePlayer.Play(_hashedFirst, Random.insideUnitSphere * 5);
        }
    }
}
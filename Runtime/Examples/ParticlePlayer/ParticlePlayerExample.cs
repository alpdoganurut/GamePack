using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples.ParticlePlayer
{
    public class ParticlePlayerExample : MonoBehaviour
    {
        [SerializeField, Required] private string[] _Ids;

        private readonly int _hashedFirst = global::GamePack.ParticlePlayerSystem.ParticlePlayer.HashId("first");
        
        [Button, HideInEditorMode]
        private void SpawnParticle()
        {
            global::GamePack.ParticlePlayerSystem.ParticlePlayer.Play(_Ids.GetRandom(), Random.insideUnitSphere * 5);
            global::GamePack.ParticlePlayerSystem.ParticlePlayer.Play(_hashedFirst, Random.insideUnitSphere * 5);
        }
    }
}
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Examples.ParticlePlayer
{
    public class ParticlePlayerExample : MonoBehaviour
    {
        [SerializeField, Required] private string[] _Ids;

        private readonly int _hashedFirst = GamePack.Tools.Systems.ParticlePlayerSystem.ParticlePlayer.HashId("first");
        
        [Button, HideInEditorMode]
        private void SpawnParticle()
        {
            GamePack.Tools.Systems.ParticlePlayerSystem.ParticlePlayer.Play(_Ids.GetRandom(), Random.insideUnitSphere * 5);
            GamePack.Tools.Systems.ParticlePlayerSystem.ParticlePlayer.Play(_hashedFirst, Random.insideUnitSphere * 5);
        }
    }
}
using GamePack.ParticlePlayerSystem;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples
{
    public class ParticlePlayerExample : MonoBehaviour
    {
        [SerializeField, Required] private string[] _Ids;

        private readonly int _hashedFirst = ParticlePlayer.HashId("first");
        
        [Button, HideInEditorMode]
        private void SpawnParticle()
        {
            ParticlePlayer.Play(_Ids.GetRandom(), Random.insideUnitSphere * 5);
            ParticlePlayer.Play(_hashedFirst, Random.insideUnitSphere * 5);
        }
    }
}
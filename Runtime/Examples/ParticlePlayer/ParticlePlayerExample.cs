using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples.ParticlePlayer
{
    public class ParticlePlayerExample : MonoBehaviour
    {
        [SerializeField, Required] private string[] _Ids;
        
        [Button, HideInEditorMode]
        private void SpawnParticle()
        {
            global::GamePack.Tools.Systems.ParticlePlayerSystem.ParticlePlayer.Play(_Ids.GetRandom(), Random.insideUnitSphere * 5);
        }
    }
}
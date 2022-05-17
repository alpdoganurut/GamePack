using System;
using GamePack.ParticlePlayerSystem;
using GamePack.TimerSystem;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePack.Examples
{
    public class ParticlePlayerExample : MonoBehaviour
    {
        [SerializeField, Required] private string[] _Ids;
        
        [SerializeField, Required] private Transform _Mover;

        private readonly int _hashedFirst = ParticlePlayer.HashId("first");

        private void Start()
        {
            if(_Mover)
            {
                var initialPos = _Mover.transform.position;
                var finalPos = _Mover.transform.position + new Vector3(0, 0, 5);
                new Operation(duration: 1, updateAction: tVal =>
                {
                    _Mover.transform.position = Vector3.Lerp(initialPos, finalPos, tVal);
                }).Start().Repeat();
            }
            
        }

        [Button, HideInEditorMode]
        private void SpawnParticle()
        {
            ParticlePlayer.Play(_Ids.GetRandom(), Random.insideUnitSphere * 5);
            ParticlePlayer.Play(_hashedFirst, Random.insideUnitSphere * 5);

        }

        [Button, HideInEditorMode]
        private void SpawnFollowing()
        {
            ParticlePlayer.Play(_Ids.GetRandom(), follow: _Mover);
        }
    }
}
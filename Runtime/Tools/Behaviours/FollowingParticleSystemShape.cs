using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FollowingParticleSystemShape: MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        
        [SerializeField] private GameObject _ObjectToFollow;
        [SerializeField] private Vector3 _Offset;


        private void LateUpdate()
        {
            var particleSystemShape = _particleSystem.shape;
            particleSystemShape.position = transform.InverseTransformPoint(_ObjectToFollow.transform.position + _Offset);
        }
        
        
#if UNITY_EDITOR
        [Button]
        private void SaveCurrentOffset()
        {
            _Offset = transform.position - _ObjectToFollow.transform.position;
        }

        private void OnValidate()
        {
            if (!_particleSystem) _particleSystem = GetComponent<ParticleSystem>();
        }
#endif
    }
}
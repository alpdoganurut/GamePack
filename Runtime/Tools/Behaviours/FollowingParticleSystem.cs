using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FollowingParticleSystem: MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        public ParticleSystem ParticleSystem
        {
            get
            {
                if (!_particleSystem) _particleSystem = GetComponent<ParticleSystem>();
                return _particleSystem;
            }
        }
        
        [SerializeField] private GameObject _ObjectToFollow;
        [SerializeField] private Vector3 _Offset;


        private void LateUpdate()
        {
            var particleSystemShape = ParticleSystem.shape;
            particleSystemShape.position = transform.InverseTransformPoint(_ObjectToFollow.transform.position + _Offset);
        }
        
        
#if UNITY_EDITOR
        [Button]
        private void GetCurrentOffset()
        {
            _Offset = transform.position - _ObjectToFollow.transform.position;
        }
#endif
    }
}
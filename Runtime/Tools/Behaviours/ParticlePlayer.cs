using System;
using System.Collections.Generic;
using GamePack.Poolable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack
{
    public class ParticlePlayer: MonoBehaviour
    {
        [Serializable]
        private struct ParticleConfig
        {
            [AssetsOnly] public PoolableParticle Prefab;
            [AssetsOnly] public int PrefillCount;
            public string Id;
        }

        private static ParticlePlayer _instance;
        
        [SerializeField, Required] private ParticleConfig[] _Configs;
        
        private readonly Dictionary<int, PoolController> _poolControllers = new Dictionary<int, PoolController>();
        
        private void Awake()
        {
            Assert.IsNull(_instance);
            _instance = this; 
            
            for (var index = 0; index < _Configs.Length; index++)
            {
                var particleConfig = _Configs[index];
                var newPoolController = gameObject.AddComponent<PoolController>();
                newPoolController.Init(particleConfig.Prefab, particleConfig.PrefillCount);
                _poolControllers[index] = newPoolController;
                var hashedId = HashId(particleConfig.Id);
                _poolControllers[hashedId] = newPoolController;
            }
        }

        public static void Play(int hashedId, Vector3 position)
        {
            if (!_instance)
            {
                Debug.LogError($"Create {nameof(ParticlePlayer)} instance in scene before using!");
                return;
            }
            
            if(!_instance._poolControllers.ContainsKey(hashedId)) return;
            
            var poolable = _instance._poolControllers[hashedId].Get();
            poolable.transform.position = position;
        }

        public static void Play(string id, Vector3 position)
        {
            Play(HashId(id), position);
        }
        
        public static int HashId(string id)
        {
            return Animator.StringToHash(id);
        }
    }
}
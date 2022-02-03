using System;
using System.Collections.Generic;
using System.Linq;
using GamePack.Logging;
using GamePack.Poolable;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace GamePack.Tools.Systems.ParticlePlayerSystem
{
    public static class ParticlePlayer
    {
        private static ParticlePlayerConfig _instance;
        
        // [SerializeField, Required] private ParticleConfig[] _Configs;
        
        private static readonly Dictionary<int, PoolController> PoolControllers = new Dictionary<int, PoolController>();
        private static GameObject _managedGameObject;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            InitializeOnEnterPlayMode();
        }

        // [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode()
        {
            ManagedLog.Log($"{nameof(ParticlePlayerConfig)}.{nameof(InitializeOnLoad)}", ManagedLog.Type.Structure);
            
            // Assert.IsNull(_instance);
            _instance = FindAllObjects.InScene<ParticlePlayerConfig>().FirstOrDefault();
            _managedGameObject = new GameObject($"{nameof(ParticlePlayer)} Helper");

            if (!_instance)
            {
                ManagedLog.LogError("Can't find ParticlePlayerConfig in scene, ParticlePlayer is inactive.");
            }
            else
            {
                ManagedLog.Log($"Found {nameof(ParticlePlayerConfig)} object in scene. {_instance.GetScenePath()}");
            }
            
            if(_instance)
                InitiateConfigs();
        }

        private static void InitiateConfigs()
        {
            for (var index = 0; index < _instance.Configs.Length; index++)
            {
                var particleConfig = _instance.Configs[index];
                var newPoolController = _managedGameObject.AddComponent<PoolController>();
                newPoolController.Init(particleConfig.Prefab, particleConfig.PrefillCount);
                PoolControllers[index] = newPoolController;

                var hashedId = HashId(particleConfig.Id);
                PoolControllers[hashedId] = newPoolController;
            }
        }

        public static void Play(int hashedId, Vector3 position)
        {
            if (!_instance)
            {
                Debug.LogError($"Create {nameof(ParticlePlayer)} instance in scene before using!");
                return;
            }
            
            if(!PoolControllers.ContainsKey(hashedId)) return;
            
            var poolable = PoolControllers[hashedId].Get();
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
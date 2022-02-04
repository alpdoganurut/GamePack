using System.Collections.Generic;
using System.Linq;
using GamePack.Logging;
using GamePack.Poolable;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Tools.Systems.ParticlePlayerSystem
{
    public static class ParticlePlayer
    {
        private static readonly Color LogColor = Colors.Aqua;
        
        private static ParticlePlayerConfig _config;
        
        private static readonly Dictionary<int, PoolController> PoolControllers = new Dictionary<int, PoolController>();
        private static GameObject _managedGameObject;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(arg1 != LoadSceneMode.Additive)
                Initialize();
        }

        private static void Initialize()
        {
            ManagedLog.Log($"{nameof(ParticlePlayerConfig)}.{nameof(InitializeOnLoad)}",
                ManagedLog.Type.Structure, color: LogColor);
            
            _config = FindAllObjects.InScene<ParticlePlayerConfig>().FirstOrDefault();
            _managedGameObject = new GameObject($"{nameof(ParticlePlayer)} Helper");

            if (!_config)
            {
                ManagedLog.Log("Can't find ParticlePlayerConfig in scene, ParticlePlayer is inactive.",
                    ManagedLog.Type.Verbose, color:LogColor);
            }
            else
            {
                ManagedLog.Log($"Found {nameof(ParticlePlayerConfig)} object in scene. {_config.GetScenePath()}",
                    color:LogColor);
            }
            
            if(_config)
                InitiateConfigs();
        }

        private static void InitiateConfigs()
        {
            for (var index = 0; index < _config.Configs.Length; index++)
            {
                var particleConfig = _config.Configs[index];
                var newPoolController = _managedGameObject.AddComponent<PoolController>();
                newPoolController.Init(particleConfig.Prefab, particleConfig.PrefillCount);
                PoolControllers[index] = newPoolController;

                var hashedId = HashId(particleConfig.Id);
                PoolControllers[hashedId] = newPoolController;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void Play(int hashedId, Vector3 position)
        {
            if (!_config)
            {
                Debug.LogError($"Create {nameof(ParticlePlayerConfig)} instance in scene before using!");
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
        
        // ReSharper disable once MemberCanBePrivate.Global
        public static int HashId(string id)
        {
            return Animator.StringToHash(id);
        }
    }
}
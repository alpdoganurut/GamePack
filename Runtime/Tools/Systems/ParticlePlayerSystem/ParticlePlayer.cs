#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System.Linq;
using GamePack.Logging;
using GamePack.PoolingSystem;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.ParticlePlayerSystem
{
    public static class ParticlePlayer
    {
        private static readonly Color LogColor = Colors.Aqua;
        
        private static ParticlePlayerSceneConfig _config;
        
        private static readonly Dictionary<int, PoolController> PoolControllers = new Dictionary<int, PoolController>();
        private static GameObject _managedGameObject;

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoadMethod()
        {
            InitializeOnLoadMethod();
        }
#endif

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.Log($"{nameof(ParticlePlayer)}.{nameof(InitializeOnLoadMethod)}",
                ManagedLog.Type.Structure, color: LogColor);

            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            ManagedLog.Log($"{nameof(ParticlePlayer)}.{nameof(SceneManagerOnSceneLoaded)}",
                ManagedLog.Type.Structure, color: LogColor);
            if(arg1 != LoadSceneMode.Additive)
                Initialize();
        }

        private static void Initialize()
        {
            ManagedLog.Log($"{nameof(ParticlePlayerSceneConfig)}.{nameof(InitializeOnLoadMethod)}",
                ManagedLog.Type.Structure, color: LogColor);
            
            _config = FindAllObjects.InScene<ParticlePlayerSceneConfig>().FirstOrDefault();
            _managedGameObject = new GameObject($"{nameof(ParticlePlayer)} Helper");

            if (!_config)
            {
                ManagedLog.Log("Can't find ParticlePlayerConfig in scene, ParticlePlayer is inactive.",
                    ManagedLog.Type.Verbose, color:LogColor);
            }
            else
            {
                ManagedLog.Log($"Found {nameof(ParticlePlayerSceneConfig)} object in scene. {_config.GetScenePath()}",
                    color:LogColor);
            }
            
            if(_config)
                InitiateConfigs();
        }

        private static void InitiateConfigs()
        {
            for (var index = 0; index < _config.ParticleConfigs.Length; index++)
            {
                var particleConfig = _config.ParticleConfigs[index];
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
                Debug.LogError($"Create {nameof(ParticlePlayerSceneConfig)} instance in scene before using!");
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
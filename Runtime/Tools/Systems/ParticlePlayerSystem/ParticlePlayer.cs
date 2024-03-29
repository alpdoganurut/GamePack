#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System.Linq;
using GamePack.Logging;
using GamePack.Modules.ObjectPool;
using GamePack.Utilities;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

namespace GamePack.ParticlePlayerSystem
{
    public static class ParticlePlayer
    {
        private static readonly Color LogColor = Colors.Aqua;
        
        private static ParticlePlayerSceneConfig _config;
        
        private static readonly Dictionary<int, ParticlePlayerPoolController> PoolControllers = new();
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
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure, color: LogColor);

            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure, color: LogColor);
            if(arg1 != LoadSceneMode.Additive)
                Initialize();
        }

        private static void Initialize()
        {
            ManagedLog.LogMethod(type:ManagedLog.Type.Structure, color: LogColor);
            
            _config = FindAllObjects.InScene<ParticlePlayerSceneConfig>().FirstOrDefault();

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

            if (!_config) return;
            
            _managedGameObject = new GameObject($"{nameof(ParticlePlayer)}Helper");
            InitiateConfigs();
        }

        private static void InitiateConfigs()
        {
            for (var index = 0; index < _config.ParticleConfigs.Length; index++)
            {
                var particleConfig = _config.ParticleConfigs[index];
                var newPoolController = _managedGameObject.AddComponent<ParticlePlayerPoolController>();
                newPoolController.Init(particleConfig.Prefab, particleConfig.PrefillCount);
                PoolControllers[index] = newPoolController;

                var hashedId = HashId(particleConfig.Id);
                PoolControllers[hashedId] = newPoolController;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static PoolableParticle Play(int hashedId, Vector3 position = new(), Transform follow = null)
        {
            if (!_config)
            {
                Debug.LogError($"Create {nameof(ParticlePlayerSceneConfig)} instance in scene before using!");
                return null;
            }
            
            if(!PoolControllers.ContainsKey(hashedId))
            {
                Debug.LogWarning($"Can't find particle. Hashed ID: {hashedId}");
                return null;
            }
            
            var poolable = PoolControllers[hashedId].Get();

            if (follow)
            {
                var positionConstraint = poolable.gameObject.AddComponent<PositionConstraint>();
                positionConstraint.AddSource(new ConstraintSource {weight = 1, sourceTransform = follow});
                positionConstraint.constraintActive = true;
                
                var rotationConstraint = poolable.gameObject.AddComponent<RotationConstraint>();
                rotationConstraint.AddSource(new ConstraintSource {weight = 1, sourceTransform = follow});
                rotationConstraint.constraintActive = true;
                
                poolable.LifeDidEnd += FollowingParticleOnLifeDidEnd;
            }
            else poolable.transform.position = position;

            return poolable;
        }

        private static void FollowingParticleOnLifeDidEnd(PoolableBase obj)
        {
            var positionConstraint = obj.gameObject.GetComponent<PositionConstraint>();
            Object.Destroy(positionConstraint);
            var rotConstraint = obj.gameObject.GetComponent<RotationConstraint>();
            Object.Destroy(rotConstraint);
            obj.LifeDidEnd -= FollowingParticleOnLifeDidEnd;
        }

        public static PoolableParticle Play(string id, Vector3 position = new(), Transform follow = null) => Play(HashId(id), position, follow);

        // ReSharper disable once MemberCanBePrivate.Global
        public static int HashId(string id) => Animator.StringToHash(id);
    }
}
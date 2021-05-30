using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace GameStructure
{
    public static class GameStructureSystem
    {
        private const bool ShowHelper = false;
        
        private static readonly List<TimeResponder> Responders = new List<TimeResponder>();
        private static GameStructureSceneHelper _helper;

        static GameStructureSystem()
        {
            Debug.Log("GameStructureSystem: Constructed.");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log($"GameStructureSystem: OnSceneLoaded {arg1}");

            if (arg1 == LoadSceneMode.Additive) return; // Not triggered when additive.
            
            Assert.IsTrue(_helper == null);
            
            Debug.Log("GameStructureSystem: Creating helper.");
            _helper = new GameObject(nameof(GameStructureSceneHelper)).AddComponent<GameStructureSceneHelper>();
            _helper.StartEvent += OnStart;
            _helper.UpdateEvent += OnUpdate;
            _helper.FixedUpdateEvent += OnFixedUpdate;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!ShowHelper) _helper.hideFlags = HideFlags.HideInHierarchy;
        }

        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            Debug.Log("GameStructureSystem: InitializeOnEnterPlayMode");
            Responders.Clear();
        }

        private static void OnUpdate()
        {
            foreach (var timeResponder in Responders)
            {
                timeResponder.Update();
            }
        }

        private static void OnFixedUpdate()
        {
            foreach (var timeResponder in Responders)
            {
                timeResponder.FixedUpdate();
            }
        }

        private static void OnStart()
        {
            foreach (var timeResponder in Responders)
            {
                timeResponder.Start();
            }
        }

        public static void AddResponder(TimeResponder responder)
        {
            Responders.Add(responder);
        }

        public static void RemoveResponder(TimeResponder responder)
        {
            Responders.Remove(responder);
        }
    }
}
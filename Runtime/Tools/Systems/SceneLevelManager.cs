#if UNITY_EDITOR
using UnityEditor; 
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace GamePack
{
    public class SceneLevelManager: MonoBehaviour
    {
        [SerializeField, Required, FoldoutGroup("Setup")]
        private string _LevelKey;
        
        [SerializeField, FoldoutGroup("Setup")]
        private bool _IsLoop = true;
        
        [SerializeField, FoldoutGroup("Setup"), ShowIf("_IsLoop"), Min(0), MaxValue("@_LevelSceneNames.Length - 1")]
        private int _LoopIndex;
        
        [SerializeField, ReadOnly, FoldoutGroup("Info")]
        private string[] _LevelSceneNames;
        
        [ShowInInspector, ReadOnly, FoldoutGroup("Info")]
        private Scene? _loadedScene;
        
        private AsyncOperation _asyncOperation;
        
        [ShowInInspector, FoldoutGroup("Info"), InlineButton("IterateLevel", "+")]
        public int CurrentLevelIndex
        {
            get => PlayerPrefs.GetInt(_LevelKey, 0);
            set => PlayerPrefs.SetInt(_LevelKey, value);
        }

        [ShowInInspector, FoldoutGroup("Info")]
        private int ClampedLevelIndex
        {
            get
            {
                if (_LevelSceneNames.Length - _LoopIndex <= 0)
                {
                    Debug.LogError("_LoopIndex should be smaller than scene count.");
                    return 0;
                }

                if (!_IsLoop) return Mathf.Clamp(CurrentLevelIndex, 0, _LevelSceneNames.Length - 1);

                if (_LevelSceneNames.Length == 0) return 0;

                return _LoopIndex +
                       ((CurrentLevelIndex - _LoopIndex) % (_LevelSceneNames.Length - _LoopIndex));
            }
        }

        /// Async. Unloads currently loaded level first and calls callback.
        public void LoadCurrentLevelScene(Action callback)
        {
            Assert.IsTrue(_asyncOperation == null || _asyncOperation.isDone);
            
#if UNITY_EDITOR
            var levelSceneName = _TestLevel ? _TestLevel.name : _LevelSceneNames[ClampedLevelIndex];
#else
            var levelSceneName = _LevelSceneNames[ClampedLevelIndex];
#endif

            if (_loadedScene.HasValue)
            {
                UnloadCurrentLevel(UnLoadComplete);
            }
            else
            {
                UnLoadComplete();
            }
            
            void UnLoadComplete()
            {
                Debug.Log("Scene unload complete");
                Assert.IsTrue(_asyncOperation == null || _asyncOperation.isDone);
                _asyncOperation = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Additive);
                _asyncOperation.completed += LoadOnComplete;
            }
            
            void LoadOnComplete(AsyncOperation obj)
            {
                _loadedScene = SceneManager.GetSceneByName(levelSceneName);
                callback?.Invoke();
            }
        }

        public void IterateLevel()
        {
            CurrentLevelIndex++;
        }

        public void UnloadCurrentLevel(Action didUnload)
        {
            if(!_loadedScene.HasValue) return;
            
            _asyncOperation = SceneManager.UnloadSceneAsync(_loadedScene.Value);
            _asyncOperation.completed += DidUnload;

            void DidUnload(AsyncOperation asyncOperation)
            {
                _loadedScene = null;
                _asyncOperation.completed -= DidUnload;
                didUnload?.Invoke();
            }
        }
        
        #region Development
#if UNITY_EDITOR
        
        [SerializeField, Required, FoldoutGroup("Setup")] 
        private SceneAsset[] _SceneAssets; 
        
        public SceneAsset[] SceneAssets
        {
            get => _SceneAssets;
            set => _SceneAssets = value;
        }
        
        private void OnValidate()
        {
            // Refresh build setting scenes
            var refreshBuildSettingScenes = false;
            foreach (var sceneAsset in SceneAssets)
            {
                if(EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == AssetDatabase.GetAssetPath(sceneAsset)) == null)
                {
                    refreshBuildSettingScenes = true;
                    Debug.Log($"{sceneAsset.name} is missing from build settings, adding it automatically!");
                }
            }
            if(refreshBuildSettingScenes)  RefreshBuildSettings();
            // Add test level
            if (EditorBuildSettings.scenes.FirstOrDefault(scene =>
                scene.path == AssetDatabase.GetAssetPath(_TestLevel)) == null)
            {
                Debug.Log("Adding Test Level to build settings.");
                var allScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes)
                {
                    new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(_TestLevel), true)
                };
                EditorBuildSettings.scenes = allScenes.ToArray();
            }
            // Convert scenes to names
            _LevelSceneNames = SceneAssets.Select(asset => asset.name).ToArray();
        }

        [Button(ButtonSizes.Large)]
        public void RefreshBuildSettings()
        {
            var allScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (var sceneAsset in SceneAssets)
            {
                var assetPath = AssetDatabase.GetAssetPath(sceneAsset);
                
                if(EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == assetPath) == null)
                {
                    allScenes.Add(new EditorBuildSettingsScene(assetPath, true)); 
                }
                    
            }

            EditorBuildSettings.scenes = allScenes.ToArray();
        }
        
        [SerializeField,  InlineButton("@_TestLevel = null", "Clear")] 
        public SceneAsset _TestLevel;
#endif
        #endregion
    }
}
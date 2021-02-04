using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace GamePack
{
    public class SceneLevelManager: MonoBehaviour
    {
        [SerializeField, InfoBox("Change this to a unique string!")]
        private string _LevelKey = "com.alpdoganurut.levelindex";
        
        #region Development
#if UNITY_EDITOR
        [SerializeField, Required] private SceneAsset[] _SceneAssets; 
        
        private void OnValidate()
        {
            _LevelSceneNames = _SceneAssets.Select(asset => asset.name).ToArray();

            foreach (var sceneAsset in _SceneAssets)
            {
                if(EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == AssetDatabase.GetAssetPath(sceneAsset)) == null)
                {
                    Debug.LogError($"{sceneAsset.name} is missing from build settings!");
                }
                    
            }
        }
        
        [SerializeField, InfoBox("Set this to test a specific level. Unset to return to normal.")] private SceneAsset _TestLevel;

        [Button]
        private void AddLevelsToBuildSettings()
        {
            var allScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (var sceneAsset in _SceneAssets)
            {
                var assetPath = AssetDatabase.GetAssetPath(sceneAsset);
                
                if(EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == assetPath) == null)
                {
                    allScenes.Add(new EditorBuildSettingsScene(assetPath, true)); 
                }
                    
            }

            EditorBuildSettings.scenes = allScenes.ToArray();
        }
#endif
        #endregion
        
        [SerializeField, Required, InfoBox("Set -1 to disable looping.")] private int _LoopIndex = -1;
        [SerializeField, ReadOnly] private string[] _LevelSceneNames;
        [ShowInInspector, ReadOnly] private Scene? _loadedScene;
        private AsyncOperation _asyncOperation;
        
        [ShowInInspector]
        public int CurrentLevelIndex
        {
            get
            {
                var currentLevelIndex = PlayerPrefs.GetInt(_LevelKey, 0);
                // currentLevelIndex = Mathf.Clamp(currentLevelIndex, 0, _LevelSceneNames.Length - 1);
                
                return currentLevelIndex;
            }
             set
            {
                // if (value > _LevelSceneNames.Length - 1) value = _LevelSceneNames.Length - 1;
                PlayerPrefs.SetInt(_LevelKey, value);
            }
        }

        [ShowInInspector]
        private int ClampedLevelIndex
        {
            get
            {
                if (_LoopIndex < 0) return Mathf.Clamp(CurrentLevelIndex, 0, _LevelSceneNames.Length - 1);

                if (_LevelSceneNames.Length == 0) return 0;
                
                return _LoopIndex +
                       ((CurrentLevelIndex - _LoopIndex) % (_LevelSceneNames.Length - _LoopIndex));
            }
        }

        /// <summary>
        /// Async. Unloads currently loaded level first and calls callback.
        /// </summary>
        /// <param name="callback"></param>
        public void LoadCurrentLevelScene(Action callback)
        {
            Assert.IsTrue(_asyncOperation == null || _asyncOperation.isDone);
            
            var levelSceneName = _LevelSceneNames[ClampedLevelIndex];
            
            #region Development - Test Level
#if UNITY_EDITOR
            if (_TestLevel)
                levelSceneName = _TestLevel.name; 
#endif
            #endregion

            if (_loadedScene.HasValue)
            {
                UnloadCurrentLevel(UnLoadComplete);
                // _asyncOperation = SceneManager.UnloadSceneAsync(_loadedScene.Value);
                // _asyncOperation.completed += LoadScene;
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
                /*_loadedScene = SceneManager.GetSceneByName(levelSceneName);
                callback?.Invoke();*/
            }
            
            void LoadOnComplete(AsyncOperation obj)
            {
                _loadedScene = SceneManager.GetSceneByName(levelSceneName);
                callback?.Invoke();
            }
        }

        [Button]
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
    }
}
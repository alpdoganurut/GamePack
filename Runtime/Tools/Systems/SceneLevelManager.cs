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
        [SerializeField, InfoBox("Change this to a unique string!"), FoldoutGroup("Setup")]
        private string _LevelKey = "com.alpdoganurut.levelindex";
        
        [SerializeField, Required, InfoBox("Set -1 to disable looping."), FoldoutGroup("Setup")]
        private int _LoopIndex = -1;
        
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
                if (_LoopIndex < 0) return Mathf.Clamp(CurrentLevelIndex, 0, _LevelSceneNames.Length - 1);

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
            _LevelSceneNames = SceneAssets.Select(asset => asset.name).ToArray();

            var addAll = false;
            foreach (var sceneAsset in SceneAssets)
            {
                if(EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == AssetDatabase.GetAssetPath(sceneAsset)) == null)
                {
                    addAll = true;
                    Debug.Log($"{sceneAsset.name} is missing from build settings, adding it automatically!");
                }
            }
            
            if(addAll)  AddLevelsToBuildSettings();
        }

        [Button(ButtonSizes.Large)]
        private void AddLevelsToBuildSettings()
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
        
        [SerializeField, InfoBox("Set this to test a specific level. Unset to return to normal.")] 
        private SceneAsset _TestLevel;
#endif
        #endregion
    }
}
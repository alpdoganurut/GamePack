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
    [CreateAssetMenu(fileName = "Level Manager", menuName = "Hex/Scene Level Manager", order = 0)]
    public class SceneLevelManager: ScriptableObject
    {
        private const string ActivateSceneNamesInfo =
            "Enable to use level scenes lighting settings. Activating scenes also cause new objects to be Instantiated in that scene.";
     
        // Static access to Loaded Scene
        public static Scene? LoadedScene => _loadedScene;

        [ShowInInspector, ReadOnly, TabGroup("Info")]
        private static Scene? _loadedScene;
        
        [SerializeField, TabGroup("Setup")]
        private bool _IsLoop = true;
        
        [SerializeField, TabGroup("Setup"), ShowIf("_IsLoop"), Min(0), MaxValue("@Mathf.Max(_LevelSceneNames != null ? (_LevelSceneNames.Length - 1) : 0, 0)")]
        private int _LoopIndex;

        [SerializeField, TabGroup("Setup"), InfoBox(ActivateSceneNamesInfo)]
        private bool _ActivateAfterLoad;

        [SerializeField, ReadOnly, TabGroup("Info")]
        private string[] _LevelSceneNames;
        
        private AsyncOperation _asyncOperation;
        
        [SerializeField, ReadOnly, TabGroup("Info")]
        private string _LevelKey;

        [ShowInInspector, TabGroup("Info"), InlineButton("IterateLevel", "+")]
        public int CurrentLevelIndex
        {
            get => PlayerPrefs.GetInt(_LevelKey, 0);
            set => PlayerPrefs.SetInt(_LevelKey, value);
        }

        [ShowInInspector, TabGroup("Info")]
        private int ClampedLevelIndex
        {
            get
            {
                if (_LevelSceneNames == null || _LevelSceneNames.Length == 0) return 0;
                if (!_IsLoop) return Mathf.Clamp(CurrentLevelIndex, 0, _LevelSceneNames.Length - 1);
                if (_LevelSceneNames.Length - _LoopIndex <= 0)
                {
                    Debug.LogError("_LoopIndex should be smaller than scene count.");
                    return 0;
                }


                if (_LevelSceneNames.Length == 0) return 0;

                return _LoopIndex +
                       ((CurrentLevelIndex - _LoopIndex) % (_LevelSceneNames.Length - _LoopIndex));
            }
        }

        #region InitializeOnEnterPlayMode
#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            _loadedScene = null;
        } 
#endif
        #endregion
        
        /// Async. Unloads currently loaded level first and calls callback.
        public void LoadCurrentLevelScene(Action callback)
        {
            Assert.IsTrue(_asyncOperation == null || _asyncOperation.isDone);
            Assert.IsTrue(_LevelSceneNames.Length > 0, "No levels set in Level Manager!");
            
            // Not loading a scene is a test feature.
#if UNITY_EDITOR
            if(!_LoadLevelScene)
            {
                if(SceneManager.sceneCount > 2 || SceneManager.sceneCount <= 1) Debug.LogError("Invalid scene count! There should be 2 scenes on level load.");
                _loadedScene = SceneManager.GetSceneAt(1);
                callback?.Invoke();
                return;
            }
#endif
            // Set Scene Name
            var levelSceneName = _LevelSceneNames[ClampedLevelIndex];
#if UNITY_EDITOR
            if (_TestLevel)
                levelSceneName = _TestLevel.name;
#endif

            // Unload if necessary
            if (_loadedScene.HasValue && _loadedScene.Value.IsValid()) UnloadCurrentLevel(LoadScene);
            else LoadScene();
            
            void LoadScene()
            {
                Debug.Log("Scene unload complete");
                Assert.IsTrue(_asyncOperation == null || _asyncOperation.isDone);
                
                // Load TestLevel if it is set and in Editor environment 
#if UNITY_EDITOR
                if (_TestLevel)
                {
                    var guid = AssetDatabase.FindAssets($"{_TestLevel.name} t:scene")[0];
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    _asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path,
                        new LoadSceneParameters(LoadSceneMode.Additive));
                    _asyncOperation.completed += OnloadComplete;
                    return;
                }
#endif          
                _asyncOperation = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Additive);
                _asyncOperation.completed += OnloadComplete;
            }

            void OnloadComplete(AsyncOperation asyncOperation)
            {
                _loadedScene = SceneManager.GetSceneByName(levelSceneName);
                if(_ActivateAfterLoad)
                    SceneManager.SetActiveScene(_loadedScene.Value);
                callback?.Invoke();
            }
        }

        public void IterateLevel()
        {
            CurrentLevelIndex++;
        }

        public void UnloadCurrentLevel(Action didUnload)
        {
            
#if UNITY_EDITOR
            if(!_LoadLevelScene)
            {
                didUnload?.Invoke();
                return;
            }
#endif
            
            if(!_loadedScene.HasValue || !_loadedScene.Value.IsValid())
            {
                Debug.LogError("!_loadedScene.HasValue failed when unloading level. Returning but not sure if should all just callback immediately.");
                didUnload?.Invoke();
                return;
            }
            
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
        
        [SerializeField, Required, TabGroup("Setup")] 
        private SceneAsset[] _SceneAssets; 
        
        public SceneAsset[] SceneAssets
        {
            get => _SceneAssets;
            set => _SceneAssets = value;
        }

        private void OnValidate()
        {
            // Refresh build setting scenes
            if(_SceneAssets != null)
            {
                var refreshBuildSettingScenes = false;
                foreach (var sceneAsset in SceneAssets)
                {
                    if(!sceneAsset) continue;
                    
                    if (EditorBuildSettings.scenes.FirstOrDefault(scene =>
                        scene.path == AssetDatabase.GetAssetPath(sceneAsset)) == null)
                    {
                        
                        refreshBuildSettingScenes = true;
                        Debug.Log($"{sceneAsset.name} is missing from build settings, adding it automatically!");
                    }
                }

                if (refreshBuildSettingScenes) RefreshBuildSettings();
            }
            
            // Convert scenes to names
            _LevelSceneNames = SceneAssets.Where(asset => asset).Select(asset => asset.name).ToArray();
            
            // LevelKey
            _LevelKey = PlayerSettings.applicationIdentifier + ".levelindex";
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
        
        [SerializeField, 
         HideInInspector,
         InlineButton("@_TestLevel = null", "Clear"),
         TabGroup("Setup")] 
        public SceneAsset _TestLevel;

        [SerializeField, InfoBox("Disable to cancel scene loading for testing."), TabGroup("Setup")] 
        private bool _LoadLevelScene = true;
#endif

        #endregion
    }
}
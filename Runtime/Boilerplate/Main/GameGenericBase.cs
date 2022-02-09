
using System.Linq;
using GamePack.Boilerplate.Structure;
using GamePack.Boilerplate.Tutorial;
using GamePack.Logging;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_ANALYTICS
using ElephantSDK;
using GameAnalyticsSDK;
#endif

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedParameter.Global

namespace GamePack.Boilerplate.Main
{
    public abstract class GameGenericBase<TConfig, TLevelHelper, TLevelInitData> : GameBase where TConfig: ConfigBase where TLevelHelper: LevelHelperBase where TLevelInitData: LevelInitDataBase
    {
        private const string LevelTextPrefix = "Level ";
        
        private static TConfig _staticConfig;

        public static TConfig Config
        {
            get
            {
                // Try to find Game in editor mode
                if (!_staticConfig && !Application.isPlaying)
                {
                    var sceneGame = FindObjectOfType<GameGenericBase<TConfig, TLevelHelper, TLevelInitData>>();
                    
                    if(_staticConfig) return _staticConfig = sceneGame._Config;
                }
                #region Development - Find config in editor
    #if UNITY_EDITOR
                // Try to find first Config for test purposes
                if (!_staticConfig)
                {
                    _staticConfig = FindAllObjects.InEditor<TConfig>().FirstOrDefault();
                    if(_staticConfig) Debug.Log($"No Game present. Found a config file in {AssetDatabase.GetAssetPath(_staticConfig)}.");
                    else Debug.Log("No config file found.");
                } 
    #endif
                #endregion
                return _staticConfig;
            }
        }

        [SerializeField, Required, FoldoutGroup("Default")]
        private SceneLevelManager _SceneLevelManager;
        
        [SerializeField, FoldoutGroup("Default"),
         Required, InlineButton("@UnityEditor.Selection.activeObject = _Config", "Select")]
        private TConfig _Config;

        [SerializeField, FoldoutGroup("Default"),
         InlineButton("SelectOrCreateGameEvents", "@_GameEvents ? \"Select\" : \"Create\"")]
        private GameEvents _GameEvents;

        [SerializeField, FoldoutGroup("Default"),
         InlineButton("SelectOrCreateTutorialManager", "@_TutorialManager ? \"Select\" : \"Create\"")]
        private TutorialManager _TutorialManager;

        [SerializeField, FoldoutGroup("Default")]
        private GameObject _FakeScene;
        
        [SerializeField, Required, FoldoutGroup("Default")]
        private Button _StartGameButton;
        
        [SerializeField, Required, FoldoutGroup("Default")]
        private TMP_Text _LevelNumberText;

        [Space]
        
        [SerializeField, Required, FoldoutGroup("Default")]
        private bool _UnloadSceneAfterStop = true;
        private TLevelHelper _levelHelper;
        
        
        [ShowInInspector, ReadOnly, PropertyOrder(-1)] private bool _isPlaying;

        [SerializeField, Required] private TLevelInitData _LevelInitData;
        
        // [ShowInInspector, ReadOnly] private ControllerGenericBase<TLevelInitData>[] _controllers;

        #region Development - InitializeOnEnterPlayMode
    #if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            if (options == EnterPlayModeOptions.DisableDomainReload)
            {
                _staticConfig = null;
            }
        } 
    #endif
        #endregion
        
        protected virtual void Awake()
        {
            #if ENABLE_ANALYTICS
            GameAnalytics.Initialize();
            #endif
            
            _staticConfig = _Config;
            
            if(_StartGameButton) _StartGameButton.onClick.AddListener(StartGame);
            RefreshLevelNumberText();
            if(_FakeScene) _FakeScene.SetActive(true);

            Application.targetFrameRate = 60;
            Time.timeScale = _Config.DefaultTimeScale;
        }


        private protected override void InternalStartGame()
        {
            if (_isPlaying)
            {
                Debug.LogError("Game is already started!");
                return;
            }
        
#if ENABLE_ANALYTICS
            Elephant.LevelStarted(_SceneLevelManager.CurrentLevelIndex);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, _SceneLevelManager.CurrentLevelIndex.ToString());
#endif
            _isPlaying = true;

            WillStartLevel();
            _SceneLevelManager.LoadCurrentLevelScene(() =>
            {
                ManagedLog.Log($"Level Scene is loaded.", ManagedLog.Type.Verbose);
                
                if(_GameEvents) _GameEvents.Trigger(true);
                
                if(_FakeScene) _FakeScene.SetActive(false);
                
                _levelHelper = FindObjectOfType<TLevelHelper>();
                
                if (!LevelHelper)
                    Debug.LogError($"No LevelHelper found in the scene: {SceneLevelManager.LoadedScene}!");

                if (_TutorialManager)
                    _TutorialManager.ShowTutorial(_SceneLevelManager.CurrentLevelIndex);

                // Invoke controller methods
                if(StructureManager.Controllers != null)
                    foreach (var controller in StructureManager.Controllers)
                    {
                        (controller as ControllerGenericBase<TLevelInitData>)?.InternalOnLevelStart(_LevelInitData);
                    }
                
                /*if(_levelHelper.Controllers != null)
                    foreach (var controller in _levelHelper.Controllers)
                        controller.InternalOnLevelStart(_LevelInitData);*/
                
                DidStartLevel(LevelHelper);
            });
        }

        private protected override void InternalStopGame(bool isSuccess)
        {
            if (!_isPlaying)
            {
                Debug.LogError("Game is not started yet!");
                return;
            }

#if ENABLE_ANALYTICS
            if(isSuccess)
            {
                Elephant.LevelCompleted(_SceneLevelManager.CurrentLevelIndex);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, _SceneLevelManager.CurrentLevelIndex.ToString());
            }
            else
            {
                Elephant.LevelFailed(_SceneLevelManager.CurrentLevelIndex);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, _SceneLevelManager.CurrentLevelIndex.ToString());
            }
#endif
            
            if(isSuccess) _SceneLevelManager.IterateLevel();
            
            _isPlaying = false;

            WillStopLevel(LevelHelper, isSuccess);
            
            if(_GameEvents) _GameEvents.Trigger(false, isSuccess);
            if(_TutorialManager) _TutorialManager.Cancel();
            if(_FakeScene) _FakeScene.SetActive(true);
            RefreshLevelNumberText();
            if (_UnloadSceneAfterStop)
            {
                _SceneLevelManager.UnloadCurrentLevel(() =>
                {
                    DidStopLevel(isSuccess);
                });
            }
            
            if(StructureManager.Controllers != null)
                foreach (var controller in StructureManager.Controllers)
                {
                    (controller as ControllerGenericBase<TLevelInitData>)?.InternalOnLevelStop();
                }
        }
        
        private void RefreshLevelNumberText()
        {
            if (_LevelNumberText) _LevelNumberText.text = $"{LevelTextPrefix} {_SceneLevelManager.CurrentLevelIndex}";
        }
        
        #region Public API

        public override bool IsPlaying => _isPlaying;

        private TLevelHelper LevelHelper => _levelHelper;

        // ReSharper disable once UnusedMember.Global
        public SceneLevelManager LevelManager => _SceneLevelManager;

        #endregion

        #region Virtual Game State Callbacks

        protected virtual void WillStartLevel(){}

        protected virtual void DidStartLevel(TLevelHelper levelHelper){}

        protected virtual void WillStopLevel(TLevelHelper levelHelper, bool isSuccess){}

        protected virtual void DidStopLevel(bool isSuccess){}

        #endregion

        #region Development

    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_Config)
            {
                Debug.Log("Config is empty. Trying to find it in Assets.");
                _Config = FindAllObjects.InEditor<TConfig>().FirstOrDefault();

                Debug.Log(!_Config
                    ? "Can't find Config for game. Please create one."
                    : $"Found a Config at {UnityEditor.AssetDatabase.GetAssetPath(_Config)}");
            }
            
            if (!_SceneLevelManager)
            {
                Debug.Log("SceneLevelManager is empty. Trying to find it in Assets.");
                _SceneLevelManager = FindAllObjects.InEditor<SceneLevelManager>().FirstOrDefault();

                Debug.LogError(!_SceneLevelManager
                    ? "Can't find SceneLevelManager for game. Create one."
                    : $"Found a SceneLevelManager at {UnityEditor.AssetDatabase.GetAssetPath(_SceneLevelManager)}");
            }

            // _controllers = FindAllObjects.InScene<ControllerGenericBase<TLevelInitData>>().ToArray();
        }
        
        // ReSharper disable once UnusedMember.Local
        private void SelectOrCreateGameEvents()
        {
            if (_GameEvents)
            {
                UnityEditor.Selection.activeGameObject = _GameEvents.gameObject;
                return;
            }

            if (!_GameEvents)
            {
                _GameEvents = GetComponentInChildren<GameEvents>();
                if(_GameEvents)
                    Debug.Log("Fetched already existing GameEvents.");
            }

            if (!_GameEvents)
            {
                if (!EditorUtility.DisplayDialog("GameEvents", "No GameEvents have been found, create one?", "Create",
                    "Cancel")) return;
                
                Debug.Log($"Creating new GameEvents object under ");
                _GameEvents = new GameObject("GameEvents").AddComponent<GameEvents>();
                _GameEvents.transform.SetParent(transform);
                Selection.activeGameObject = _GameEvents.gameObject;

            }
        }
        
        // ReSharper disable once UnusedMember.Local
        private void SelectOrCreateTutorialManager()
        {
            if (_TutorialManager)
            {
                Selection.activeGameObject = _TutorialManager.gameObject;
                return;
            }

            if (!_TutorialManager)
            {
                _TutorialManager = GetComponentInChildren<TutorialManager>();
                
                if(_TutorialManager)
                    Debug.Log("Fetched already existing TutorialManager.");
            }

            if (!_TutorialManager)
            {
                
                if (!EditorUtility.DisplayDialog("TutorialManager", "No TutorialManager have been found, create one?", "Create",
                    "Cancel")) return;
                
                _TutorialManager = new GameObject("TutorialManager").AddComponent<TutorialManager>();
                _TutorialManager.transform.SetParent(transform);
                
                Selection.activeGameObject = _TutorialManager.gameObject;
            }
        }
    #endif

        #endregion
        
        
    }
}

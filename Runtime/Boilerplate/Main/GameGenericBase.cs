using System;
using System.Linq;
using GamePack.Boilerplate.Structure;
using GamePack.Boilerplate.Tutorial;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedParameter.Global

namespace GamePack.Boilerplate.Main
{
    public abstract class GameGenericBase<TConfig, TLevelSceneRefBase, TMainSceneRefBase> : GameBase where TConfig: ConfigBase where TLevelSceneRefBase: LevelSceneRefBase where TMainSceneRefBase: MainSceneRefBase
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
                    var sceneGame = FindObjectOfType<GameGenericBase<TConfig, TLevelSceneRefBase, TMainSceneRefBase>>();
                    
                    if(_staticConfig) return _staticConfig = sceneGame._Config;
                }
                #region Development - Find config in editor
    #if UNITY_EDITOR
                // Try to find first Config for test purposes
                if (!_staticConfig)
                {
                    _staticConfig = FindAllObjects.InEditor<TConfig>().FirstOrDefault();
                    Debug.Log(_staticConfig
                        ? $"No Game present. Found a config file in {AssetDatabase.GetAssetPath(_staticConfig)}."
                        : "No config file found.");
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
        private TLevelSceneRefBase _levelSceneRef;
        
        [ShowInInspector, ReadOnly, PropertyOrder(-1)] private bool _isPlaying;

        [FormerlySerializedAs("_LevelInitData")] 
        [PropertyOrder(1)]
        [SerializeField, Required] private TMainSceneRefBase _MainSceneRef;
        
        // ReSharper disable once StaticMemberInGenericType
        private static GameAnalyticsDelegateBase _analyticsDelegate;
        private GameSessionDelegateBase<TLevelSceneRefBase, TMainSceneRefBase> _gameSessionDelegate;


        /*#region Development - InitializeOnEnterPlayMode
    #if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            if (options == EnterPlayModeOptions.DisableDomainReload) _staticConfig = null;
        } 
    #endif
        #endregion*/
        
        protected virtual void Awake()
        {
            _analyticsDelegate?.Initialize();

            _staticConfig = _Config;
            
            if(_StartGameButton) _StartGameButton.onClick.AddListener(StartGame);
            RefreshLevelNumberText();
            if(_FakeScene) _FakeScene.SetActive(true);

            Time.timeScale = _Config.DefaultTimeScale;
        }

        private protected override void InternalStartGame()
        {
            if (_isPlaying)
            {
                Debug.LogError("Game is already started!");
                return;
            }
      
            _analyticsDelegate?.GameDidStart(_SceneLevelManager.CurrentLevelIndex);
            _isPlaying = true;

            _gameSessionDelegate?.WillStartLevel(_MainSceneRef);
#pragma warning disable CS0618
            WillStartLevel();
#pragma warning restore CS0618
            _SceneLevelManager.LoadCurrentLevelScene(() =>
            {
                if(_GameEvents) _GameEvents.Trigger(true);
                
                if(_FakeScene) _FakeScene.SetActive(false);
                
                _levelSceneRef = FindObjectOfType<TLevelSceneRefBase>();
                
                if (!LevelSceneRef)
                    Debug.LogError($"No {nameof(LevelSceneRefBase)} found in the scene: {SceneLevelManager.LoadedScene}!");

                if (_TutorialManager)
                    _TutorialManager.ShowTutorial(_SceneLevelManager.CurrentLevelIndex);

                // Invoke controller methods
                if(StructureManager.Controllers != null)
                    foreach (var controller in StructureManager.Controllers.ToArray())  // TODO: Optimize
                    {
                        (controller as ControllerGenericBase<TMainSceneRefBase, TLevelSceneRefBase>)?.InternalOnLevelStart(_MainSceneRef, _levelSceneRef);
                    }
                
                _gameSessionDelegate?.DidStartLevel(_MainSceneRef, LevelSceneRef);
#pragma warning disable CS0618
                DidStartLevel(LevelSceneRef);
#pragma warning restore CS0618
            });
        }

        private protected override void InternalStopGame(bool isSuccess)
        {
            if (!_isPlaying)
            {
                Debug.LogError("Game is not started yet!");
                return;
            }

            _analyticsDelegate?.GameDidStop(isSuccess, _SceneLevelManager.CurrentLevelIndex);

            if(isSuccess) _SceneLevelManager.IterateLevel();
            
            _isPlaying = false;

            _gameSessionDelegate?.WillStopLevel(_MainSceneRef, LevelSceneRef, isSuccess);
#pragma warning disable CS0618
            WillStopLevel(LevelSceneRef, isSuccess);
#pragma warning restore CS0618
            
            if(_GameEvents) _GameEvents.Trigger(false, isSuccess);
            if(_TutorialManager) _TutorialManager.Cancel();
            if(_FakeScene) _FakeScene.SetActive(true);
            RefreshLevelNumberText();
            if (_UnloadSceneAfterStop)
            {
                _SceneLevelManager.UnloadCurrentLevel(() =>
                {
                    _gameSessionDelegate?.DidStopLevel(_MainSceneRef, isSuccess);
#pragma warning disable CS0618
                    DidStopLevel(isSuccess);
#pragma warning restore CS0618
                });
            }
            
            if(StructureManager.Controllers != null)
                foreach (var controller in StructureManager.Controllers)
                {
                    (controller as ControllerGenericBase<TMainSceneRefBase, TLevelSceneRefBase>)?.InternalOnLevelStop();
                }
        }
        
        private void RefreshLevelNumberText()
        {
            if (_LevelNumberText) _LevelNumberText.text = $"{LevelTextPrefix} {_SceneLevelManager.CurrentLevelIndex}";
        }
        
        #region Public API

        public override bool IsPlaying => _isPlaying;

        private TLevelSceneRefBase LevelSceneRef => _levelSceneRef;

        // ReSharper disable once UnusedMember.Global
        public SceneLevelManager LevelManager => _SceneLevelManager;

        // protected TMainSceneRefBase MainSceneRef => _MainSceneRef;

        #endregion

        #region Virtual Game State Callbacks

        [Obsolete("Use GameSessionDelegate for handling level event.")]
        protected virtual void WillStartLevel(){}

        [Obsolete("Use GameSessionDelegate for handling level event.")]
        protected virtual void DidStartLevel(TLevelSceneRefBase sceneRef){}

        [Obsolete("Use GameSessionDelegate for handling level event.")]
        protected virtual void WillStopLevel(TLevelSceneRefBase sceneRef, bool isSuccess){}

        [Obsolete("Use GameSessionDelegate for handling level event.")]
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
                    : $"Found a Config at {AssetDatabase.GetAssetPath(_Config)}");
            }
            
            if (!_SceneLevelManager)
            {
                Debug.Log("SceneLevelManager is empty. Trying to find it in Assets.");
                _SceneLevelManager = FindAllObjects.InEditor<SceneLevelManager>().FirstOrDefault();

                Debug.LogError(!_SceneLevelManager
                    ? "Can't find SceneLevelManager for game. Create one."
                    : $"Found a SceneLevelManager at {AssetDatabase.GetAssetPath(_SceneLevelManager)}");
            }
        }
        
        // ReSharper disable once UnusedMember.Local - Used in UI
        private void SelectOrCreateGameEvents()
        {
            if (_GameEvents)
            {
                Selection.activeGameObject = _GameEvents.gameObject;
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
        
        // ReSharper disable once UnusedMember.Local - Used in UI
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
        
        // ReSharper disable once UnusedMember.Global - Used when Analytics is enabled
        public static void SetAnalyticsDelegate(GameAnalyticsDelegateBase analyticsDelegate) => 
            _analyticsDelegate = analyticsDelegate;

        protected void SetGameSessionDelegate(GameSessionDelegateBase<TLevelSceneRefBase, TMainSceneRefBase> gameSessionDelegate)
        {
            _gameSessionDelegate.Game = this;
            _gameSessionDelegate = gameSessionDelegate;
            _gameSessionDelegate?.InitiateMainScene(_MainSceneRef);
        }
    }
}

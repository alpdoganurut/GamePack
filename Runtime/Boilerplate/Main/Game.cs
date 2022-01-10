#if UNITY_EDITOR
using UnityEditor; 
#endif

#if ENABLE_ANALYTICS
using ElephantSDK;
using GameAnalyticsSDK;
#endif

using System.Linq;
using GamePack;
using GamePack.UnityUtilities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;

namespace HexGames
{
    public abstract class Game<TConfig, TLevelHelper> : GameBase where TConfig: ConfigBase where TLevelHelper: LevelHelperBase
    {

    private static TConfig _staticConfig;

    public static TConfig Config
    {
        get
        {
            // Try to find Game in editor mode
            if (!_staticConfig && !Application.isPlaying)
            {
                var sceneGame = FindObjectOfType<Game<TConfig, TLevelHelper>>();
                
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
    } // Access config when not in play mode

    public static Scene? LoadedScene => SceneLevelManager.LoadedScene;
    
    [SerializeField, Required, FoldoutGroup("Default")]
    private SceneLevelManager _SceneLevelManager;
    
    
    [SerializeField, Required, FoldoutGroup("Default")]
    private bool _UnloadSceneAfterStop = true;

    [SerializeField, 
     Required, InlineButton("@UnityEditor.Selection.activeObject = _Config", "Select"), 
     FoldoutGroup("Default")]
    private TConfig _Config;

    [SerializeField,
     InlineButton("SelectOrCreateGameEvents", "@_GameEvents ? \"Select\" : \"Create\""), 
     FoldoutGroup("Default")]
    private GameEvents _GameEvents;

    
    [SerializeField,
     InlineButton("SelectOrCreateTutorialManager", "@_TutorialManager ? \"Select\" : \"Create\""), 
     FoldoutGroup("Default")]
    private TutorialManager _TutorialManager;
    
    [SerializeField, HideInInlineEditors, Required, FoldoutGroup("Default")]
    private Button _StartGameButton;
    
    private TLevelHelper _levelHelper;
    
    [ShowInInspector, ReadOnly] private bool _isPlaying;

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

        Application.targetFrameRate = 60;

/*#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif*/
    }

    #region Public API

    public override bool IsPlaying => _isPlaying;

    protected TLevelHelper LevelHelper => _levelHelper;

    public SceneLevelManager LevelManager => _SceneLevelManager;

    public override void StartGame()
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

        WillStartGame();
        _SceneLevelManager.LoadCurrentLevelScene(() =>
        {
            if(_GameEvents)
                _GameEvents.Trigger(true);
            
            _levelHelper = FindObjectOfType<TLevelHelper>();
            if (!LevelHelper)
            {
                Debug.Log("No LevelHelper found in the scene.");
            }

            if (_TutorialManager)
            {
                _TutorialManager.ShowTutorial(_SceneLevelManager.CurrentLevelIndex);
            }
            
            DidStartGame(LevelHelper);
        });
    }

    public override void StopGame(bool isSuccess)
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

        WillStopGame(LevelHelper, isSuccess);
        if(_GameEvents) _GameEvents.Trigger(false, isSuccess);
        if (_UnloadSceneAfterStop)
        {
            _SceneLevelManager.UnloadCurrentLevel(() =>
            {
                DidStopGame(isSuccess);
            });
        }
    }

    #endregion

    #region Virtual Game State Callbacks

    protected virtual void WillStartGame(){}

    protected  virtual void DidStartGame(TLevelHelper levelHelper){}

    protected  virtual void WillStopGame(TLevelHelper levelHelper, bool isSuccess){}

    protected  virtual void DidStopGame(bool isSuccess){}

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
                ? "Can't find SceneLevelManager for game. Please create one."
                : $"Found a SceneLevelManager at {UnityEditor.AssetDatabase.GetAssetPath(_SceneLevelManager)}");
        }

        /*if (!_SceneLevelManager)
        {
            Debug.Log("Missing LevelManager, creating one.");
            _SceneLevelManager = new GameObject("LevelManager").AddComponent<SceneLevelManager>();
            _SceneLevelManager.transform.SetParent(transform);
        }*/
    }

    // [ResponsiveButtonGroup]
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

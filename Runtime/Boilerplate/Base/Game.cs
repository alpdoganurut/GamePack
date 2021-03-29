#if ENABLE_ANALYTICS
using ElephantSDK;
using GameAnalyticsSDK;
#endif
using System.Linq;
using GamePack;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HexGames
{
    public abstract class Game<TConfig, TLevelHelper> : GameBase where TConfig: ConfigBase where TLevelHelper: LevelHelperBase
    {
    private static TConfig _staticConfig;

    public static TConfig Config
    {
        get
        {
            if (!_staticConfig && !Application.isPlaying) return _staticConfig = FindObjectOfType<Game<TConfig, TLevelHelper>>()._Config;
            return _staticConfig;
        }
    } // Access config when not in play mode

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

    private TLevelHelper _levelHelper;
    
    [ShowInInspector, ReadOnly] private bool _isPlaying;
    
    [SerializeField, HideInInlineEditors] private Button _StartGameButton;

    protected void Awake()
    {
        #if ENABLE_ANALYTICS
        GameAnalytics.Initialize();
        #endif
        
        _staticConfig = _Config;
        _StartGameButton.onClick.AddListener(StartGame);
    }

    #region Public API

    public override bool IsPlaying => _isPlaying;

    public override void StartGame()
    {
        if (_isPlaying)
        {
            Debug.LogError("Game is already started!");
            return;
        }
    
#if ENABLE_ANALYTICS
        Elephant.LevelStarted(_LevelManager.CurrentLevelIndex);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, _LevelManager.CurrentLevelIndex.ToString());
#endif
        
        _isPlaying = true;

        WillStartGame();
        _SceneLevelManager.LoadCurrentLevelScene(() =>
        {
            _GameEvents.Trigger(true);
            _levelHelper = FindObjectOfType<TLevelHelper>();
            if (!_levelHelper)
            {
                Debug.Log("No LevelHelper found in the scene.");
            }

            DidStartGame(_levelHelper);
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
            Elephant.LevelCompleted(_LevelManager.CurrentLevelIndex);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, _LevelManager.CurrentLevelIndex.ToString());
        }
        else
        {
            Elephant.LevelFailed(_LevelManager.CurrentLevelIndex);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, _LevelManager.CurrentLevelIndex.ToString());
        }
#endif
        
        _isPlaying = false;

        WillStopGame(_levelHelper);
        _GameEvents.Trigger(false, isSuccess);
        if (_UnloadSceneAfterStop)
        {
            _SceneLevelManager.UnloadCurrentLevel(() =>
            {
                DidStopGame(_levelHelper);
            });
        }
    }

    #endregion

    #region Abstract Game State Callbacks

    public abstract void WillStartGame();

    public virtual void DidStartGame(TLevelHelper levelHelper)
    {
    }

    public abstract void WillStopGame(TLevelHelper levelHelper);

    public abstract void DidStopGame(TLevelHelper levelHelper);

    #endregion

    #region Development

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!_Config)
        {
            Debug.Log("Config is empty. Trying to find it in the editor.");
            _Config = FindAllObjects.InEditor<TConfig>().FirstOrDefault();

            Debug.Log(!_Config
                ? "Can't find Config for game. Please create one."
                : $"Found a Config at {UnityEditor.AssetDatabase.GetAssetPath(_Config)}");
        }

        if (!_SceneLevelManager)
        {
            Debug.Log("Missing LevelManager, creating one.");
            _SceneLevelManager = new GameObject("LevelManager").AddComponent<SceneLevelManager>();
            _SceneLevelManager.transform.SetParent(transform);
        }
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
            Debug.Log("Fetched already existing GameEvents.");
        }

        if (!_GameEvents)
        {
            _GameEvents = new GameObject("GameEvents").AddComponent<GameEvents>();
            _GameEvents.transform.SetParent(transform);
        }
    }
#endif

    #endregion

    }
}

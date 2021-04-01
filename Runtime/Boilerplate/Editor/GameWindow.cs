using System;
using System.Diagnostics;
using System.Linq;
using GamePack;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace HexGames
{
    [Title("@Title", horizontalLine: false)]
    // [TypeInfoBox("@IsValidGameScene ? (\"Scene: \" + _scene.name) : \"Not a valid scene\"")]
    public class GameWindow: OdinEditorWindow
    {
        // ReSharper disable once UnusedMember.Local
        private string Title => IsValidGameScene ? ("Scene: " + _scene.name + (EditorApplication.isCompiling ? " | Compiling..." : "")) : "Not Valid";
        
        [PropertyOrderAttribute(-1)]
        [ShowInInspector, HideInPlayMode]
        private string GameName
        {
            get => PlayerSettings.productName;
            set
            {
                PlayerSettings.productName = value;
                PlayerSettings.applicationIdentifier = "com.hex." + value.ToLower().Replace(" ", string.Empty);
                EditorSettings.projectGenerationRootNamespace = value.Replace(" ", string.Empty);
            }
        }
        
        #region Initilization

        private static GameWindow _instance;
        private static bool _isInit;
        
        private static bool _isListening;

        private bool IsValidGameScene => _game;
        private Scene _scene;

        private event Action EnterPlayCallback;
        
        [MenuItem("Hex/Game")]
        public static void ShowWindow()
        {
            GetWindow<GameWindow>();
        }

        private void Awake()
        {
            Init();
            _instance = this;
        }

        public void Init()
        {
            Log("Initializing Game Window.");
            if(!_isListening)
            {
                ListenSceneChange();
            }
            else
                Log("Skipped event subscribing.");
            
            _isInit = true;

            InitScene(SceneManager.GetActiveScene());
            if(Application.isPlaying)
                EnterPlayCallback?.Invoke();
        }

        private void InitScene(Scene scene)
        {
            Log($"Initializing scene {scene.name}");

            _scene = scene;
            
            _game = FindObjectOfType<GameBase>();
            _levelHelper = FindObjectOfType<LevelHelperBase>();

            // Check if valid Scene
            if (IsValidGameScene)
            {
                _config = ReflectionHelper.GetPropOrField(_game, "_Config") as ConfigBase;
                _levelManager = ReflectionHelper.GetPropOrField(_game, "_SceneLevelManager") as SceneLevelManager;
                _gameEvents = ReflectionHelper.GetPropOrField(_game, "_GameEvents") as GameEvents;
                Log($"{scene.name} is valid Game scene.");
            }
            else if (_levelHelper)
            {
                Log($"{scene.name} Is valid Level scene.");
            }

        }

        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            Log($"OnEnterPlaymodeInEditor: {options}");

            _isInit = false;
            
            
            if (!_instance || !options.HasFlag(EnterPlayModeOptions.DisableDomainReload)) return;
            
            _instance.StopListeningSceneChange();
        }

        private void EditorSceneManagerOnSceneLoaded(Scene arg1, Scene scene)
        {
            Log($"Scene opened: {scene.name}");
            InitScene(scene);
        }
        
        protected override void OnGUI()
        {
            if(_isInit) base.OnGUI();
            
            if (!_isInit) Init();
        }

        protected override void OnDestroy()
        {
            StopListeningSceneChange();
            Log("Window closed.");
        }

        private void ListenSceneChange()
        {
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerOnSceneLoaded;
            _isListening = true;
        }
        
        public void StopListeningSceneChange()
        {
            Log("Stopping listening.");
            EditorSceneManager.activeSceneChangedInEditMode -= EditorSceneManagerOnSceneLoaded;
            _isListening = false;
        }
        
        #endregion

        #region Game

        // [FoldoutGroup("Main", order:1)]
        
        
        [FoldoutGroup("Game")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private GameBase _game;

        #endregion

        #region Events & UI

        [FoldoutGroup("Game/Events & UI")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("@IsValidGameScene && _gameEvents")]
        private GameEvents _gameEvents;

        [FoldoutGroup("Game/Events & UI", order: -1)]
        [ShowInInspector, ShowIf("@IsValidGameScene")]
        private Button StartGameButton
        {
            get => _game ? ReflectionHelper.GetFieldAsUnityObject(_game, "_StartGameButton") as Button : null;
            set { if(_game) ReflectionHelper.SetFieldWithReflection(_game, "_StartGameButton", value); }
        }

        #endregion
        
        #region Config

        
        
        [FoldoutGroup("Config")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private ConfigBase _config;

        #endregion

        #region Levels

        [FoldoutGroup("Levels")]
        [ShowInInspector, ShowIf("IsValidGameScene")]
        private SceneAsset[] Levels
        {
            get => _levelManager ? _levelManager.SceneAssets : null;
            set { if(_levelManager) _levelManager.SceneAssets = value; }
        }

        // [FoldoutGroup("Levels")]
        // [ShowInInspector, ShowIf("IsValidGameScene")]
        private SceneLevelManager _levelManager;

        [FoldoutGroup("Levels")]
        [ShowInInspector, ShowIf("IsValidGameScene")]
        private void SelectLevelManager()
        {
            Selection.activeObject = _levelManager;
        }

        
        #endregion
        
        #region Game Actions

        [Button(ButtonSizes.Large),
         ShowIf("@EditorApplication.isPlaying && IsValidGameScene && !_game.IsPlaying")]
        private void StartGame()
        {
            _game.StartGame();
        }
        
        [HorizontalGroup(GroupID = "0"), Button(ButtonSizes.Large),
         ShowIf("@EditorApplication.isPlaying && IsValidGameScene && _game.IsPlaying")]
        private void StopGameSuccess()
        {
            _game.StopGame(true);
        }
        
        [HorizontalGroup(GroupID = "0"), Button(ButtonSizes.Large),
         ShowIf("@EditorApplication.isPlaying && IsValidGameScene && _game.IsPlaying")]
        private void StopGameFail()
        {
            _game.StopGame(false);
        }

        #endregion

        #region Settings

        private const string EnableAnalyticsDefineSymbol = "ENABLE_ANALYTICS";
        private const string LoggingDefineSymbol = "GAME_WINDOW_LOGGING";

        [FoldoutGroup("Settings")]
        [Button]
        private void Refresh()
        {
            
            Init();
        }
        
        [FoldoutGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool DisableReloadDomain
        {
            get =>
                EditorSettings.enterPlayModeOptionsEnabled &&
                EditorSettings.enterPlayModeOptions == EnterPlayModeOptions.DisableDomainReload;
            set
            {
                EditorSettings.enterPlayModeOptionsEnabled = value;
                EditorSettings.enterPlayModeOptions =  value ? EnterPlayModeOptions.DisableDomainReload : EnterPlayModeOptions.None;
            }
        }
        
        [FoldoutGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool Analytics
        {
            get =>
                IsDefineSymbolEnabled(EnableAnalyticsDefineSymbol);
            set => SetDefineSymbol(value, EnableAnalyticsDefineSymbol);
        }
        
        [FoldoutGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool Logging
        {
            get =>
                IsDefineSymbolEnabled(LoggingDefineSymbol);
            set => SetDefineSymbol(value, LoggingDefineSymbol);
        }
        
        [FoldoutGroup("Settings"), ShowInInspector, ShowIf("@_game != null")]
        private bool GameVisible
        {
            get =>
                _game && _game.hideFlags == HideFlags.None;
            set
            {
                if(value)
                {
                    _game.gameObject.hideFlags = HideFlags.None;
                    EditorApplication.RepaintHierarchyWindow();
                    EditorApplication.DirtyHierarchyWindowSorting();
                }
                else
                {
                    _game.gameObject.hideFlags = HideFlags.HideInHierarchy;
                    EditorApplication.RepaintHierarchyWindow();
                    EditorApplication.DirtyHierarchyWindowSorting();
                }

                if(!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(_scene);
            }
        }

        private static bool IsDefineSymbolEnabled(string symbol)
        {
            return PlayerSettings
                .GetScriptingDefineSymbolsForGroup(
                    BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget)).Contains(symbol);
        }

        private static void SetDefineSymbol(bool value, string symbol)
        {
            var settings =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(
                    BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

            var containsSymbol = settings.Contains(symbol);
            if (!value && containsSymbol)
            {
                var splitSettings = settings.Split(';').ToList();
                splitSettings.Remove(symbol);

                settings = splitSettings.Aggregate((s, s1) => s + ";" + s1);
            }

            if (value && !containsSymbol)
            {
                settings += ";" + symbol;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), settings);
        }

        #endregion

        #region Test Level

        
        [Button("Test"),
         HorizontalGroup("Test Level", width: 50, order: 100)/* ResponsiveButtonGroup("Editor Actions", AnimateVisibility = false)*/,
         ShowIf("@DisableReloadDomain && !EditorApplication.isPlaying && IsValidGameScene")]
        private void RunTestLevel()
        {
            EditorApplication.isPlaying = true;

            void Callback()
            {
                _game.StartGame();
                EnterPlayCallback -= Callback;
            }

            EnterPlayCallback += Callback;

        }
        
        // [FoldoutGroup("Levels")]
        [ShowInInspector, HorizontalGroup("Test Level"),
         ShowIf("@IsValidGameScene && !EditorApplication.isPlaying"),
         // InlineButton("@_levelManager._TestLevel = null", "Clear"),
            // InlineButton("RunTestLevel", "Test")
        ]
        private SceneAsset TestLevel
        {
            get => _levelManager ? _levelManager._TestLevel : null;
            set { if(_levelManager) _levelManager._TestLevel = value; }
        }

        #endregion
        
        #region Level Helper

        [Title("Level Helper")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("@_levelHelper")]
        private LevelHelperBase _levelHelper;

        #endregion

        [Conditional("GAME_WINDOW_LOGGING")]
        private static void Log(object msg)
        {
            Debug.Log(msg);
        }
    }
}
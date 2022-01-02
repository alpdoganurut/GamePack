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
    [Title("@PlayerSettings.productName")]
    [Title("@Title", horizontalLine: false)]
    // [TypeInfoBox("@IsValidGameScene ? (\"Scene: \" + _scene.name) : \"Not a valid scene\"")]
    public class GameWindow: OdinEditorWindow
    {
        // ReSharper disable once UnusedMember.Local
        private string Title
        {
            get
            {
                if (_levelHelper != null)
                    return $"{_scene.name}";
                
                return IsValidGameScene
                    ? _scene.name + (EditorApplication.isCompiling ? " | Compiling..." : "")
                    : "Not Valid";
            }
        }

        // [InfoBox("@\"Product Name: \" + PlayerSettings.productName", InfoMessageType.None)]
        [PropertyOrderAttribute(-1)]
        [ShowInInspector, HideInPlayMode, ShowIf("@_config != null")]
        private string GameIdentifier
        {
            get => _game ? _game.WorkingTitle : null;
            set
            {
                _game.WorkingTitle = value;
                if(string.IsNullOrWhiteSpace(PlayerSettings.productName))
                    PlayerSettings.productName = value;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.hex." + value.ToLower().Replace(" ", string.Empty));
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
        
        [MenuItem("Window/Game Window")]
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
                _config = ReflectionHelper.GetPropertyOrField(_game, "_Config") as ConfigBase;
                _levelManager = ReflectionHelper.GetPropertyOrField(_game, "_SceneLevelManager") as SceneLevelManager;
                _gameEvents = ReflectionHelper.GetPropertyOrField(_game, "_GameEvents") as GameEvents;
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

        private void OnInspectorUpdate()
        {
            // if(_isInit) base.OnGUI();
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
        
        #region Test Level

        [PropertyOrderAttribute(-1)]
        [ShowInInspector, HorizontalGroup("Test Level"),
         ShowIf("@IsValidGameScene && !EditorApplication.isPlaying")]
        private SceneAsset TestLevel
        {
            get => _levelManager ? _levelManager._TestLevel : null;
            set { if(_levelManager) _levelManager._TestLevel = value; }
        }
        
        
        [Button("Test"),
         HorizontalGroup("Test Level", width: 50)/* ResponsiveButtonGroup("Editor Actions", AnimateVisibility = false)*/,
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
        
        #endregion

        #region Game

        [PropertyOrderAttribute(-2)]
        [Button(size: ButtonSizes.Large), HideIf("IsValidGameScene")]
        private void OpenMainScene()
        {
            EditorSceneManager.OpenScene("Assets/01_Scenes/main.unity");
        }
        
        [TabGroup("Game")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private GameBase _game;

        #endregion

        #region Events & UI

        [Title("Events & UI")]
        [TabGroup("Game")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("@IsValidGameScene && _gameEvents")]
        private GameEvents _gameEvents;

        [TabGroup("Game", order: -1)]
        [ShowInInspector, ShowIf("@IsValidGameScene")]
        private Button StartGameButton
        {
            get => _game ? ReflectionHelper.GetFieldAsUnityObject(_game, "_StartGameButton") as Button : null;
            set { if(_game) ReflectionHelper.SetFieldWithReflection(_game, "_StartGameButton", value); }
        }

        #endregion
        
        #region Config

        
        
        [TabGroup("Config")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private ConfigBase _config;

        #endregion

        #region Levels

        [TabGroup("Levels")]

        /*[TabGroup("Levels")]
        [ShowInInspector, ShowIf("IsValidGameScene")]
        private SceneAsset[] Levels
        {
            get => _levelManager ? _levelManager.SceneAssets : null;
            set { if(_levelManager) _levelManager.SceneAssets = value; }
        }*/

        [ShowInInspector,
         ShowIf("IsValidGameScene"),
         InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private SceneLevelManager _levelManager;

        /*[TabGroup("Levels")]
        [ShowInInspector, ShowIf("IsValidGameScene")]
        private void SelectLevelManager()
        {
            Selection.activeObject = _levelManager;
        }*/

        
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

        [TabGroup("Settings")]
        [Button]
        private void Refresh()
        {
            
            Init();
        }
        
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
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
        
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool Analytics
        {
            get =>
                IsDefineSymbolEnabled(EnableAnalyticsDefineSymbol);
            set => SetDefineSymbol(value, EnableAnalyticsDefineSymbol);
        }
        
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool Logging
        {
            get =>
                IsDefineSymbolEnabled(LoggingDefineSymbol);
            set => SetDefineSymbol(value, LoggingDefineSymbol);
        }
        
        [TabGroup("Settings"), ShowInInspector, ShowIf("@_game != null")]
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
using GamePack.Boilerplate;
using GamePack.Boilerplate.Main;
using GamePack.Logging;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        #region Initilization

        private static GameWindow _instance;
        
        [PropertyOrder(OrderTabsMid)]
        [TabGroup("Config")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static ConfigBase _staticConfig;
        
        [TabGroup("Levels")]
        [ShowInInspector, PropertyOrder(GameWindow.OrderTabsMid),
         InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static SceneLevelManager _staticLevelManager;
        
        // ReSharper disable once UnusedMember.Local
        private bool IsValidGameSceneAndMain => _game && SceneManager.GetActiveScene().path == MainSceneAssetPath;
        private static bool IsValidGameScene => _game;

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

        private void Init()
        {
            GameWindow.Log("Initializing Game Window.");
            /*
            if(!_isListening)
            {
                ListenSceneChange();
            }
            else
                GameWindow.Log("Skipped event subscribing.");
                */
            

            InitScene(SceneManager.GetActiveScene());
            if(Application.isPlaying)
                EnterPlayCallback?.Invoke();
        }


        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.Log($"{nameof(GameWindow)}.{nameof(InitializeOnLoadMethod)}", ManagedLog.Type.Structure);
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerOnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneManagerOnSceneLoaded;
        }


        /*
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            GameWindow.Log($"OnEnterPlaymodeInEditor: {options}");
            
            // if (!_instance || !options.HasFlag(EnterPlayModeOptions.DisableDomainReload)) return;
            
            // _instance.StopListeningSceneChange();
            // CheckTestLevelPlayModeEnter();
            
        }
        */

        private static void OnSceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode mode)
        {
            CheckLevelEnterPlayModeForLoadingMainScene();
        }
        
        private static void EditorSceneManagerOnSceneLoaded(Scene arg1, Scene scene)
        {
            GameWindow.Log($"Scene opened: {scene.name}");
            InitScene(scene);
        }
        
        private static void InitScene(Scene scene)
        {
            GameWindow.Log($"Initializing scene {scene.name}");

            _scene = scene;
            
            _game = FindObjectOfType<GameBase>();
            _levelHelper = FindObjectOfType<LevelHelperBase>();

            // Check if valid Scene
            if (IsValidGameScene)
            {
                _config = ReflectionHelper.GetPropertyOrField(_game, "_Config") as ConfigBase;
                _levelManager = ReflectionHelper.GetPropertyOrField(_game, "_SceneLevelManager") as SceneLevelManager;
                _gameEvents = ReflectionHelper.GetPropertyOrField(_game, "_GameEvents") as GameEvents;
                
                _staticConfig = _config;
                _staticLevelManager = _levelManager;
                
                GameWindow.Log($"{scene.name} is valid Game scene.");
            }
            else if (_levelHelper)
            {
                GameWindow.Log($"{scene.name} Is valid Level scene.");
            }

        }

        /*
        private void OnInspectorUpdate()
        {
            // if(_isInit) base.OnGUI();
            if (!_isInit) Init();
        }
        */

        /*
        protected override void OnDestroy()
        {
            StopListeningSceneChange();
            GameWindow.Log("Window closed.");
        }
        */

        /*
        public void StopListeningSceneChange()
        {
            GameWindow.Log("Stopping listening.");
            EditorSceneManager.activeSceneChangedInEditMode -= EditorSceneManagerOnSceneLoaded;
            _isListening = false;
        }
        */
        
        #endregion
    }
}
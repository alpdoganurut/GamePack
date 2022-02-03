using System;
using GamePack.Boilerplate;
using GamePack.Boilerplate.GameSystem;
using GamePack.UnityUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        #region Initilization

        private static global::GamePack.Editor.Boilerplate.GameWindow _instance;
        private static bool _isInit;
        
        private static bool _isListening;

        private bool IsValidGameScene => _game && SceneManager.GetActiveScene().path == GameWindow.MainSceneAssetPath;
        private Scene _scene;

        private event Action EnterPlayCallback;
        
        [MenuItem("Window/Game Window")]
        public static void ShowWindow()
        {
            GetWindow<global::GamePack.Editor.Boilerplate.GameWindow>();
        }

        private void Awake()
        {
            Init();
            _instance = this;
        }

        private void Init()
        {
            GameWindow.Log("Initializing Game Window.");
            if(!_isListening)
            {
                ListenSceneChange();
            }
            else
                GameWindow.Log("Skipped event subscribing.");
            
            _isInit = true;

            InitScene(SceneManager.GetActiveScene());
            if(Application.isPlaying)
                EnterPlayCallback?.Invoke();
        }

        private void InitScene(Scene scene)
        {
            GameWindow.Log($"Initializing scene {scene.name}");

            _scene = scene;
            
            _game = Object.FindObjectOfType<GameBase>();
            _levelHelper = Object.FindObjectOfType<LevelHelperBase>();

            // Check if valid Scene
            if (IsValidGameScene)
            {
                _config = ReflectionHelper.GetPropertyOrField(_game, "_Config") as ConfigBase;
                _levelManager = ReflectionHelper.GetPropertyOrField(_game, "_SceneLevelManager") as SceneLevelManager;
                _gameEvents = ReflectionHelper.GetPropertyOrField(_game, "_GameEvents") as GameEvents;
                GameWindow.Log($"{scene.name} is valid Game scene.");
            }
            else if (_levelHelper)
            {
                GameWindow.Log($"{scene.name} Is valid Level scene.");
            }

        }

        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            GameWindow.Log($"OnEnterPlaymodeInEditor: {options}");

            _isInit = false;
            
            if (!_instance || !options.HasFlag(EnterPlayModeOptions.DisableDomainReload)) return;
            
            _instance.StopListeningSceneChange();
        }

        private void EditorSceneManagerOnSceneLoaded(Scene arg1, Scene scene)
        {
            GameWindow.Log($"Scene opened: {scene.name}");
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
            GameWindow.Log("Window closed.");
        }

        private void ListenSceneChange()
        {
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerOnSceneLoaded;
            _isListening = true;
        }
        
        public void StopListeningSceneChange()
        {
            GameWindow.Log("Stopping listening.");
            EditorSceneManager.activeSceneChangedInEditMode -= EditorSceneManagerOnSceneLoaded;
            _isListening = false;
        }
        
        #endregion
    }
}
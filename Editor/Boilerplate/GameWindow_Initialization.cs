using System;
using Boilerplate.GameSystem;
using GamePack;
using GamePack.UnityUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexGames
{
    public partial class GameWindow
    {
        #region Initilization

        private static GameWindow _instance;
        private static bool _isInit;
        
        private static bool _isListening;

        private bool IsValidGameScene => _game && SceneManager.GetActiveScene().path == MainSceneAssetPath;
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

        private void Init()
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
    }
}
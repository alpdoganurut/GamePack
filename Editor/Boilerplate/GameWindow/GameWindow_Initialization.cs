using GamePack.Boilerplate;
using GamePack.Boilerplate.Main;
using GamePack.Logging;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {

        [TabGroup("Config")]
        [PropertyOrder(OrderTabsMid)]
        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static ConfigBase _staticConfig;

        [TabGroup("Levels")]
        [PropertyOrder(OrderTabsMid)]
        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static SceneLevelManager _staticLevelManager;
        
        // ReSharper disable once UnusedMember.Local
        private bool IsValidGameSceneAndMain => _game && SceneManager.GetActiveScene().path == MainSceneAssetPath;
        private static bool IsValidGameScene => _game;

        [MenuItem("Window/Game Window")]
        public static void ShowWindow() => GetWindow<GameWindow>();

        private void Awake() => Init();

        private void Init()
        {
            GameWindow.Log("Initializing Game Window.");
            InitScene(SceneManager.GetActiveScene());
            
            if(Application.isPlaying)
                EnterPlayCallback?.Invoke();
        }


        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure);
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChangedInEditMode;
            InitScene(SceneManager.GetActiveScene());
        }

        private static void OnActiveSceneChangedInEditMode(Scene arg1, Scene scene)
        {
            GameWindow.Log($"Scene opened: {scene.name}");
            if(Application.isPlaying) return;
            InitScene(scene);
        }
        
        private static void InitScene(Scene scene)
        {
            GameWindow.Log($"Initializing scene {scene.name}");
            
            _scene = scene;
            
            _game = FindObjectOfType<GameBase>();
            _levelSceneRef = FindObjectOfType<LevelSceneRefBase>();

            // Check if valid Scene
            if (IsValidGameScene)
            {
                _staticConfig = ReflectionHelper.GetPropertyOrField(_game, "_Config") as ConfigBase;

                _staticLevelManager = ReflectionHelper.GetPropertyOrField(_game, "_SceneLevelManager") as SceneLevelManager;
                _gameEvents = ReflectionHelper.GetPropertyOrField(_game, "_GameEvents") as GameEvents;
                
                GameWindow.Log($"{scene.name} is valid Game scene.");
            }
            else if (_levelSceneRef)
            {
                GameWindow.Log($"{scene.name} Is valid Level scene.");
            }

            // Find Config and SceneLevelManager in project if not a valid game scene
            if (!IsValidGameScene)
            {
                _staticConfig = FindInProject.AssetByType<ConfigBase>();
                _staticLevelManager = FindInProject.AssetByType<SceneLevelManager>();
            }
        }
    }
}
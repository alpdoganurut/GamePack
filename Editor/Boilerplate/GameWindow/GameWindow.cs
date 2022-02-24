using System;
using System.Diagnostics;
using GamePack.Boilerplate;
using GamePack.Boilerplate.Main;
using GamePack.Logging;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace GamePack.Editor.Boilerplate
{
    // [Title("@\"Game Name: \" + PlayerSettings.productName")]
    public partial class GameWindow: OdinEditorWindow
    {
        // Shared among class parts
        private static event Action EnterPlayCallback;
        private static Scene _scene;
        
        private const int OrderTop = -20;
        private const int OrderTopMid = -10;
        private const int OrderTabsTop = -3;
        private const int OrderTabsMid = -2;
        private const int OrderTabsBottom = -1;
        private const int OrderDefault = 0;
        private const int OrderBottomMid = 10;
        private const int OrderBottom = 20;
        
        public const string MainSceneAssetPath = "Assets/01_Scenes/main.unity";
        private const string NotSetProductName = "GAME_NAME_NOT_SET";
        private const string BuildIdentifierPrefix = "com.hex.";
        private const string NotSetGameIdentifier = "IDENTIFIERNOTSET"; // Don't use _ or " ". Unity BuildSettings removes whitespaces.

        #region Game
        
        [PropertyOrder(OrderTabsTop)]
        [TabGroup("Game")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private static GameBase _game;

        #endregion

        #region Events & UI

        // ReSharper disable once NotAccessedField.Local
        private static GameEvents _gameEvents;

        #endregion
        
        #region Config

        /*[PropertyOrder(OrderTabsMid)]
        [TabGroup("Config")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]*/
        private static ConfigBase _config;

        #endregion

        #region Levels

        /*[TabGroup("Levels")]
        [ShowInInspector, PropertyOrder(GameWindow.OrderTabsMid),
         ShowIf("IsValidGameScene"),
         InlineEditor(InlineEditorObjectFieldModes.Hidden)]*/
        private static SceneLevelManager _levelManager;

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

        [Conditional("GAME_WINDOW_LOGGING")]
        private static void Log(object msg)
        {
            ManagedLog.Log(msg, color: Colors.PowderBlue, type: ManagedLog.Type.Structure);
        }
    }
}
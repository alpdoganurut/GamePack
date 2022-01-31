using System.Diagnostics;
using Boilerplate.GameSystem;
using HexGames;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace GamePack.Editor.Boilerplate
{
    [Title("@\"Game Name: \" + PlayerSettings.productName")]
    public partial class GameWindow: OdinEditorWindow
    {
        private const int OrderTop = -20;
        private const int OrderTopMid = -10;
        private const int OrderTabsTop = -3;
        private const int OrderTabsMid = -2;
        private const int OrderTabsBottom = -1;
        private const int OrderDefault = 0;
        private const int OrderBottomMid = 10;
        private const int OrderBottom = 20;
        
        private const string MainSceneAssetPath = "Assets/01_Scenes/main.unity";
        private const string NotSetProductName = "GAME_NAME_NOT_SET";
        private const string BuildIdentifierPrefix = "com.hex.";
        private const string NotSetGameIdentifier = "IDENTIFIERNOTSET"; // Don't use _ or " ". Unity BuildSettings removes whitespaces.

        #region Test Level

        [PropertySpace]
        [PropertyOrder(OrderBottom)]
        [ShowInInspector, HorizontalGroup("Test Level"),
         ShowIf("@IsValidGameScene && !EditorApplication.isPlaying")]
        private SceneAsset TestLevel
        {
            get => _levelManager ? _levelManager._TestLevel : null;
            set { if(_levelManager) _levelManager._TestLevel = value; }
        }
        
        [PropertySpace]
        [Button("Test"),
         HorizontalGroup("Test Level", width: 70)/* ResponsiveButtonGroup("Editor Actions", AnimateVisibility = false)*/,
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
        
        [PropertyOrder(OrderTabsTop)]
        [TabGroup("Game")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private GameBase _game;

        #endregion

        #region Events & UI

        // ReSharper disable once NotAccessedField.Local
        private GameEvents _gameEvents;

        #endregion
        
        #region Config

        [PropertyOrder(OrderTabsMid)]
        [TabGroup("Config")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private ConfigBase _config;

        #endregion

        #region Levels

        [TabGroup("Levels")]
        [ShowInInspector,
         ShowIf("IsValidGameScene"),
         InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private SceneLevelManager _levelManager;

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
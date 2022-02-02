using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Boilerplate.GameSystem;
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
    [Title("@\"Game Name: \" + PlayerSettings.productName")]
    public partial class GameWindow: OdinEditorWindow
    {
        private const int OrderTop = -20;
        private const int OrderTopMid = -10;
        private const int Default = 0;
        private const int OrderBottomMid = 10;
        private const int OrderBottom = 20;
        
        private const string MainSceneAssetPath = "Assets/01_Scenes/main.unity";
        private const string NotSetProductName = "GAME_NAME_NOT_SET";
        private const string BuildIdentifierPrefix = "com.hex.";
        private const string NotSetGameIdentifier = "IDENTIFIERNOTSET"; // Don't use _ or " ". Unity BuildSettings removes whitespaces.

        [InfoBox("GameName is not set.", InfoMessageType.Error, VisibleIf = "@GameName == NotSetProductName || string.IsNullOrEmpty(GameName) ")]
        [VerticalGroup("row1/left")]
        [PropertyOrderAttribute(OrderTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameScene")]
        private string GameName
        {
            get => PlayerSettings.productName == NotSetProductName ? "" : PlayerSettings.productName;
            set => PlayerSettings.productName = string.IsNullOrEmpty(value) ? NotSetProductName : value;
        }
        
        [InfoBox("GameIdentifier is not set.", InfoMessageType.Error, VisibleIf = "@GameIdentifier == null || GameIdentifier == \"\" ")]
        [VerticalGroup("row1/left")]
        [PropertyOrderAttribute(OrderTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameScene")]
        private string GameIdentifier
        {
            get
            {
                var applicationIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
                var gameIdentifier = applicationIdentifier.Contains(BuildIdentifierPrefix) ? applicationIdentifier.Replace(BuildIdentifierPrefix, "") : "";
                
                if (gameIdentifier == NotSetGameIdentifier) return "";
                return gameIdentifier;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) value = NotSetGameIdentifier;
                else value = value.ToLower().Replace(" ", string.Empty);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, BuildIdentifierPrefix + value);
                EditorSettings.projectGenerationRootNamespace = value;
            }
        }

        [InfoBox("Icon is empty", InfoMessageType.Error, VisibleIf = "@GameIcon == null")]
        [HideLabel, HorizontalGroup("row1", 50), VerticalGroup("row1/right")]
        [ShowInInspector, PreviewField(50, ObjectFieldAlignment.Right)]
        private Texture2D GameIcon
        {
            get => PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown)[0];
            set
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new []{value});
            }
        }

        #region Test Level

        [PropertySpace]
        [PropertyOrderAttribute(OrderTop)]
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

        [PropertyOrderAttribute(OrderTop)]
        [Button(size: ButtonSizes.Large), HideIf("IsValidGameScene")]
        private void OpenMainScene()
        {
            EditorSceneManager.OpenScene(MainSceneAssetPath);
        }
        
        [PropertyOrderAttribute(OrderTopMid)]
        [TabGroup("Game")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("IsValidGameScene")]
        private GameBase _game;

        #endregion

        #region Events & UI

        private GameEvents _gameEvents;

        #endregion
        
        #region Config

        [PropertyOrderAttribute(OrderTopMid)]
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
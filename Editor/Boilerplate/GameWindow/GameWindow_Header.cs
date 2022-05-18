using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        [PropertyOrder(OrderGlobalTop)]
        [Button(ButtonSizes.Large), HideIf("IsValidGameSceneAndMain")]
        private void OpenMainScene()
        {
            EditorSceneManager.OpenScene(MainSceneAssetPath);
        }
        
        [PropertyOrder(OrderGlobalTop)]
        [Button(ButtonSizes.Large), HideIf("IsValidGameScene"), ShowIf("@_levelSceneRef")]
        private void TestThisLevel()
        {
            var sceneAssetPath = SceneManager.GetActiveScene().path;
            TestLevel = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            
            // Open main scene
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneLoad;
            EditorSceneManager.OpenScene(MainSceneAssetPath);
            void OnSceneLoad(Scene arg0, Scene scene)
            {
                EditorSceneManager.activeSceneChangedInEditMode -= OnSceneLoad;
                // Enter Play Mode
                EditorApplication.isPlaying = true;
                EnterPlayCallback += OnEnterPlayMode;
                void OnEnterPlayMode()
                {
                    // Start Game
                    EnterPlayCallback -= OnEnterPlayMode;
                    _game.StartGame();
                    
                    
                }
                
            }
            
            // Return to Level Scene
            EditorApplication.playModeStateChanged += OnExitPlayMode;
            void OnExitPlayMode(PlayModeStateChange playModeStateChange)
            {
                if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
                {
                    EditorApplication.playModeStateChanged -= OnExitPlayMode;
                    EditorSceneManager.OpenScene(sceneAssetPath);
                }
            }
        }


        [InfoBox("GameName is not set.", InfoMessageType.Error, VisibleIf = "@GameName == NotSetProductName || string.IsNullOrEmpty(GameName) ")]
        [VerticalGroup("row1/left")]
        [PropertyOrder(OrderGlobalTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameSceneAndMain")]
        private string GameName
        {
            get => PlayerSettings.productName == NotSetProductName ? "" : PlayerSettings.productName;
            set => PlayerSettings.productName = string.IsNullOrEmpty(value) ? NotSetProductName : value;
        }
        
        [InfoBox("GameIdentifier is not set.", InfoMessageType.Error, VisibleIf = "@GameIdentifier == null || GameIdentifier == \"\" ")]
        [VerticalGroup("row1/left")]
        [PropertyOrder(OrderGlobalTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameSceneAndMain")]
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
                var namespaceValue = value.Replace(" ", string.Empty);
                if (string.IsNullOrEmpty(value))
                {
                    value = NotSetGameIdentifier;
                    namespaceValue = NotSetGameIdentifier;
                }
                else value = value.ToLower().Replace(" ", string.Empty);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, BuildIdentifierPrefix + value);
                EditorSettings.projectGenerationRootNamespace = namespaceValue;
            }
        }

        // [InfoBox("Icon is empty", InfoMessageType.Error, VisibleIf = "@GameIcon == null")]
        [HideLabel, HorizontalGroup("row1", 50), VerticalGroup("row1/right")]
        [PropertyOrder(OrderGlobalTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameSceneAndMain"), PreviewField(50, ObjectFieldAlignment.Right)]
        private Texture2D GameIcon
        {
            get
            {
                var iconsForTargetGroup = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
                if (iconsForTargetGroup.Length <= 0) return null;
                return iconsForTargetGroup[0];
            }
            set
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new []{value});
            }
        }
    }
}
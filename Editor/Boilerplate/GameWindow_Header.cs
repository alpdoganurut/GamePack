using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {

        [PropertyOrder(GameWindow.OrderTop)]
        [Button(size: ButtonSizes.Large), HideIf("IsValidGameScene")]
        private void OpenMainScene()
        {
            EditorSceneManager.OpenScene(GameWindow.MainSceneAssetPath);
        }
        
        [InfoBox("GameName is not set.", InfoMessageType.Error, VisibleIf = "@GameName == NotSetProductName || string.IsNullOrEmpty(GameName) ")]
        [VerticalGroup("row1/left")]
        [PropertyOrder(GameWindow.OrderTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameScene")]
        private string GameName
        {
            get => PlayerSettings.productName == GameWindow.NotSetProductName ? "" : PlayerSettings.productName;
            set => PlayerSettings.productName = string.IsNullOrEmpty(value) ? GameWindow.NotSetProductName : value;
        }
        
        [InfoBox("GameIdentifier is not set.", InfoMessageType.Error, VisibleIf = "@GameIdentifier == null || GameIdentifier == \"\" ")]
        [VerticalGroup("row1/left")]
        [PropertyOrder(GameWindow.OrderTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameScene")]
        private string GameIdentifier
        {
            get
            {
                var applicationIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
                var gameIdentifier = applicationIdentifier.Contains(GameWindow.BuildIdentifierPrefix) ? applicationIdentifier.Replace(GameWindow.BuildIdentifierPrefix, "") : "";
                
                if (gameIdentifier == GameWindow.NotSetGameIdentifier) return "";
                return gameIdentifier;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) value = GameWindow.NotSetGameIdentifier;
                else value = value.ToLower().Replace(" ", string.Empty);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, GameWindow.BuildIdentifierPrefix + value);
                EditorSettings.projectGenerationRootNamespace = value;
            }
        }

        [InfoBox("Icon is empty", InfoMessageType.Error, VisibleIf = "@GameIcon == null")]
        [HideLabel, HorizontalGroup("row1", 50), VerticalGroup("row1/right")]
        [PropertyOrder(GameWindow.OrderTop)]
        [ShowInInspector, HideInPlayMode, ShowIf("IsValidGameScene"), PreviewField(50, ObjectFieldAlignment.Right)]
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
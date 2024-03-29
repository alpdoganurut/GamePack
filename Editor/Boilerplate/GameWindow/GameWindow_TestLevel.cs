using GamePack.Boilerplate;
using Sirenix.OdinInspector;
using UnityEditor;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        #region Test Level

        [PropertySpace]
        [PropertyOrder(OrderGlobalBottom)]
        [Title("Testing")]
        [ShowInInspector, HorizontalGroup("Test Level"),
         ShowIf("@IsValidGameScene && !EditorApplication.isPlaying")]
        public static SceneAsset TestLevel
        {
            get => SceneLevelManager.TestLevel;
            set => SceneLevelManager.TestLevel = value;
        }
        
        [PropertySpace]
        [PropertyOrder(OrderGlobalBottom+1)]
        [Title("")]
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
    }
}
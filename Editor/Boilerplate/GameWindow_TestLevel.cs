using Sirenix.OdinInspector;
using UnityEditor;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
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
        [PropertyOrder(OrderBottom+1)]
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
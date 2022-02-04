using GamePack.Boilerplate.Main;
using Sirenix.OdinInspector;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        #region Level Helper

        [Title("Level Helper")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("@_levelHelper")]
        private LevelHelperBase _levelHelper;

        
        #endregion

    }
}
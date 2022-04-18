using GamePack.Boilerplate.Main;
using Sirenix.OdinInspector;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        #region Level Scene Ref

        [Title("Level Helper")]
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden), ShowIf("@_levelSceneRef")]
        private static LevelSceneRefBase _levelSceneRef;
        
        #endregion
    }
}
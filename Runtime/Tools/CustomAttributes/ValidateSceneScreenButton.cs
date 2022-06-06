using Shared.EditorDrawer.Buttons;

namespace GamePack.CustomAttributes
{
    public class ValidateSceneScreenButton: ScreenButtonBase
    {
        public override string Label => "Validate Scene";
        public override void Action() => CustomAttributeSystem.ProcessScene();
    }
}
using Shared.EditorDrawer.Buttons;
using UnityEngine;

namespace GamePack.CustomAttributes
{
    [CreateAssetMenu(menuName = "GamePack/ScreenButton/ValidateSceneScreenButton")]
    public class ValidateSceneScreenButton: ScriptableSceneButtonBase
    {
        public override string Label => "Validate Scene";
        public override void Action() => CustomAttributeSystem.ProcessScene();
    }
}
using System;
using UnityEngine;

namespace Shared.EditorDrawer.Buttons
{
    [CreateAssetMenu(menuName = "GamePack/ScreenButton/ToggleUIButton")]
    public class ToggleUIButton: ScriptableSceneButtonBase
    {
        private const int UILayer = 1 << 5;
        
        public override string Label => IsUILayerVisible ? "Hide UI" : "Show UI";

        private static bool IsUILayerVisible => (UnityEditor.Tools.visibleLayers & UILayer) == UILayer; 
        
        public override void Action()
        {
            if( IsUILayerVisible )
                UnityEditor.Tools.visibleLayers &= ~UILayer;
            else
                UnityEditor.Tools.visibleLayers |= UILayer;
        }
    }
}
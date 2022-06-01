#if UNITY_EDITOR
using GamePack.Tools.Helper;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Tools
{
    public class ReplaceWithWindow : OdinEditorWindow
    {
        [MenuItem("Utilities/Show Replace Tools Window", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<ReplaceWithWindow>();
        }

        
    }
}
#endif
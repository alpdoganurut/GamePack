using System.Collections.Generic;
using System.Linq;
using GamePack.Editor.Boilerplate;
using GamePack.Logging;
using GamePack.Utilities;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorDrawer
{
    public static class EditorDrawerSystem
    {
        private const float Spacing = 6;
        private const float Padding = 6;

        private static ScreenButtonsConfig _config;

        private static IEnumerable<ScreenButtonBase> Buttons
        {
            get
            {
                if(_config)
                {
                    foreach (var button in _config.Buttons)
                        yield return button;
                }

                foreach (var button in SceneButtons)
                    
                    yield return button;
                foreach (var button in DynamicButtons)
                    yield return button;
            }
        }

        private static readonly List<ScreenButtonBase> SceneButtons = new();
        
        private static readonly List<ScreenButtonBase> DynamicButtons = new();
        
        private static readonly List<ScreenInfo> SceneInfos = new();

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure);
            
            _config = FindInProject.ByType<ScreenButtonsConfig>();
            
            SceneView.duringSceneGui += DrawGuiOnScene;
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if(obj == PlayModeStateChange.ExitingPlayMode)
                SceneInfos.Clear();
        }

        private static void DrawGuiOnScene(SceneView view)
        {
            if (!ProjectEditorConfig.Instance.ShowSceneButtons) return;
            
            Handles.BeginGUI();
            
            var totalHeight = Spacing;
            foreach (var screenButtonBase in Buttons)
            {
                if (screenButtonBase == null) continue;

                DrawButton(screenButtonBase, totalHeight);
                totalHeight += screenButtonBase.Size.y + Spacing + (Padding * 2);
            }

            foreach (var screenInfo in SceneInfos)
            {
                var rect = new Rect(Spacing, totalHeight, screenInfo.Size.x + Padding, screenInfo.Size.y + Padding);
                GUI.skin.box.alignment = TextAnchor.MiddleCenter;
                GUI.Box(rect, screenInfo.Message, GUI.skin.box);
                totalHeight += screenInfo.Size.y + Spacing + Padding;
            }
            
            Handles.EndGUI();
        }

        public static void RegisterButtonComponent(EventButtonComponent buttonComponent)
        {
            if(Buttons.Any(button => (button as EventButton)?.Component == buttonComponent)) return;
            
            SceneButtons.Add(new EventButton(buttonComponent, buttonComponent.Label));
        }

        public static void RegisterInfo(ScreenInfo info) => SceneInfos.Add(info);

        public static void RegisterButton(ScreenButtonBase button)
        {
            DynamicButtons.Add(button);
        }

        public static void UnRegisterButtonComponent(EventButtonComponent eventButtonComponent) => 
            SceneButtons.RemoveAll(button => (button as EventButton)?.Component == eventButtonComponent);

        private static void DrawButton(ScreenButtonBase button, float yPos)
        {
            if (GUI.Button(new Rect(Spacing, yPos, button.Size.x + (Padding * 2), button.Size.y + (Padding * 2)), button.Label))
                button.Action();
        }
    }
}
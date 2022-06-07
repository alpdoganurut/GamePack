using System;
using System.Collections.Generic;
using System.Linq;
using GamePack.Editor.Boilerplate;
using GamePack.Logging;
using GamePack.Utilities;
using Shared.EditorDrawer.Buttons;
using Shared.EditorDrawer.Buttons.EventButton;
using UnityEditor;
using UnityEngine;

namespace Shared.EditorDrawer
{
    public static class EditorDrawerSystem
    {
        private const float Spacing = 4;
        private const float SpaceHeight = 6;

        private static EditorDrawerConfig _config;

        private static readonly List<IScreenButton> SceneButtons = new();
        
        private static readonly List<DynamicButton> DynamicButtons = new();
        
        private static readonly List<ScreenInfo> SceneInfos = new();

        #region Initilisation

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure);
            
            _config = FindInProject.AssetByType<EditorDrawerConfig>();
            
            SceneView.duringSceneGui += DrawGuiOnScene;
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if(obj == PlayModeStateChange.ExitingPlayMode)
                SceneInfos.Clear();
        }

        internal static void RegisterScreenButtonsConfig(EditorDrawerConfig config)
        {
            if(!_config)
                _config = config;
        }

        internal static void UnRegisterButtonComponent(ScreenButtonComponent screenButtonComponent) => 
            SceneButtons.RemoveAll(button => (button as EventButton)?.Component == screenButtonComponent);
        
        #endregion

        #region Drawing

        private static void DrawGuiOnScene(SceneView view)
        {
            if (!ProjectEditorConfig.Instance.ShowScreenButtons) return;
            
            Handles.BeginGUI();
            var areaWidth = _config ? _config.ButtonGroupWidth + 50 : Screen.width;
            GUILayout.BeginArea (new Rect (Spacing,Spacing,areaWidth, Screen.height));
            GUILayout.BeginVertical();

            try
            {
                DrawConfigButtons();
                DrawSceneButtons();
                DrawDynamicButtons();
                DrawScreenInfos();
            }
            catch (Exception e)
            {
                // Buttons list can change during a button action.
                Console.WriteLine(e);
                return;
            }
            
            GUILayout.EndArea();
            GUILayout.EndVertical();
            Handles.EndGUI();
        }

        private static void DrawScreenInfos()
        {
            GUILayout.Space(SpaceHeight);
            foreach (var screenInfo in SceneInfos)
            {
                GUI.skin.box.alignment = TextAnchor.MiddleCenter;
                DrawScreenInfo(screenInfo);
            }
        }

        private static void DrawDynamicButtons()
        {
            if (DynamicButtons.Count > 0)
            {
                GUILayout.Space(SpaceHeight);
                foreach (var screenButtonBase in DynamicButtons) DrawMethod(screenButtonBase);
            }
        }

        private static void DrawSceneButtons()
        {
            if (SceneButtons.Count > 0)
            {
                GUILayout.Space(SpaceHeight);
                foreach (var screenButtonBase in SceneButtons) DrawMethod(screenButtonBase);
            }
        }

        private static void DrawConfigButtons()
        {
            if (!_config) return;
            
            var lineWidth = 0f;
            var inGroup = false;
            foreach (var screenButtonBase in _config.Buttons)
            {
                if (screenButtonBase == null) continue;

                // var width = screenButtonBase.Size.x;
                var width = GUI.skin.button.CalcSize(new GUIContent(screenButtonBase.Label)).x;;
                if (lineWidth + width > _config.ButtonGroupWidth
                    || lineWidth == 0)
                {
                    if (inGroup) GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    inGroup = true;
                    lineWidth = 0;
                }

                lineWidth += (width 
                              + _config.ButtonStyle.GUIStyle.margin.left 
                              + _config.ButtonStyle.GUIStyle.margin.right);

                DrawMethod(screenButtonBase);
            }

            if (inGroup) GUILayout.EndHorizontal();
        }

        private static void DrawScreenInfo(ScreenInfo screenInfo)
        {
            if (GUILayout.Button(screenInfo.Message, _config != null ? _config.ScreenInfoStyle.GUIStyle : null))
            {
                if(screenInfo.BoundGameObject) Selection.activeGameObject = screenInfo.BoundGameObject;
            }
        }

        private static void DrawMethod(IScreenButton screenButtonBase)
        {
            if (GUILayout.Button(screenButtonBase.Label, _config != null ? _config.ButtonStyle.GUIStyle : null))
                screenButtonBase.Action();
        }

        #endregion

        #region Registration API

        public static void RegisterButtonComponent(ScreenButtonComponent buttonComponent)
        {
            if(SceneButtons.Any(button => (button as EventButton)?.Component == buttonComponent)) return;
            
            SceneButtons.Add(new EventButton(buttonComponent, buttonComponent.Label));
        }

        public static void RegisterInfo(ScreenInfo info) => SceneInfos.Add(info);

        public static void UnregisterInfo(ScreenInfo screenInfo) => SceneInfos.Remove(screenInfo);

        public static void RegisterDynamicButton(DynamicButton button) => DynamicButtons.Add(button);

        public static void UnregisterDynamicButton(DynamicButton button) => DynamicButtons.Remove(button);

        #endregion

    }
}
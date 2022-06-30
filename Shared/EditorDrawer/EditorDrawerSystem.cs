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
        private const int ScreenVisibilityMargin = 30;
        private const float Spacing = 4;
        private const float SpaceHeight = 6;
        private static EditorDrawerConfig _config;
        
        private static readonly List<IScreenButton> SceneButtons = new();
        private static readonly List<DynamicButton> DynamicButtons = new();
        private static readonly List<ScreenInfo> SceneInfos = new();

        public static EditorDrawerConfig Config
        {
            get
            {
                if(!_config) FindConfigInProject();
                return _config;
            }
        }

        private static float MaxLineWidth
        {
            get
            {
                if (!Config || !SceneView.currentDrawingSceneView) return 0;
                return Mathf.Min(
                    Config.IsWindowVisible,
                    (SceneView.currentDrawingSceneView.position.size.x - _draggedPosOffset.x - ScreenVisibilityMargin));
            }
        }

        #region Initilisation

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure);
            
            FindConfigInProject();
            
            SceneView.duringSceneGui += DrawGuiOnScene;
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        private static void FindConfigInProject() => _config = FindInProject.AssetByType<EditorDrawerConfig>();

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

            DragUpdate();
            
            Handles.BeginGUI();
            // Drag Button
            GUI.Button(new Rect(_draggedPosOffset.x + Spacing, _draggedPosOffset.y + Spacing, DragButtonSize.x, DragButtonSize.y), "", Config.ButtonStyle.GUIStyle);
            
            var areaWidth = MaxLineWidth;
            GUILayout.BeginArea (new Rect (Spacing + _draggedPosOffset.x + Spacing + DragButtonSize.x ,_draggedPosOffset.y + Spacing ,areaWidth, Screen.height));
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
                DrawScreenInfo(screenInfo);
            }
        }

        private static void DrawScreenInfo(ScreenInfo screenInfo)
        {
            if (GUILayout.Button(screenInfo.Message, _config != null ? _config.ScreenInfoStyle.GUIStyle : null))
            {
                if(screenInfo.BoundGameObject) Selection.activeGameObject = screenInfo.BoundGameObject;
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

                var width = GUI.skin.button.CalcSize(new GUIContent(screenButtonBase.Label)).x;;
                if (lineWidth + width > MaxLineWidth
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

        private static void DrawSceneButtons()
        {
            if (SceneButtons.Count <= 0) return;
            GUILayout.Space(SpaceHeight);
            foreach (var screenButtonBase in SceneButtons) DrawMethod(screenButtonBase);
        }

        private static void DrawDynamicButtons()
        {
            if (DynamicButtons.Count <= 0) return;
            GUILayout.Space(SpaceHeight);
            foreach (var screenButtonBase in DynamicButtons) DrawMethod(screenButtonBase);
        }

        private static void DrawMethod(IScreenButton screenButtonBase)
        {
            if (GUILayout.Button(screenButtonBase.Label, _config != null ? _config.ButtonStyle.GUIStyle : null))
                screenButtonBase.Action();
        }

        #endregion

        #region Dragging

        private const int ScreenDragMargin = 100;
        private static readonly Vector2 DragButtonSize = new(20, 20);
        private static Vector2 _draggedPosOffset;
        
        private static bool _isDrag;
        private static Vector2 _dragLastMousePos;
        private static Rect ActiveDragArea => new Rect(_draggedPosOffset.x + Spacing, _draggedPosOffset.y + Spacing, DragButtonSize.x, DragButtonSize.y);

        private static void DragUpdate()
        {
            var evt = Event.current;
            var isDown = evt.type == EventType.MouseDown;
            if (!_isDrag && isDown && ActiveDragArea.Contains(evt.mousePosition))
            {
                _isDrag = true;
                _dragLastMousePos = evt.mousePosition;
            }

            if (_isDrag && evt.isMouse)
            {
                var deltaMousePos = evt.mousePosition - _dragLastMousePos;
                _draggedPosOffset += deltaMousePos;
                _dragLastMousePos = evt.mousePosition;
                ClampToWindow();
            } 

            if (_isDrag && (evt.type is EventType.MouseUp or EventType.MouseLeaveWindow))
            {
                _isDrag = false;
                ClampToWindow();
            }
            
            if(evt.type is EventType.MouseEnterWindow or EventType.MouseLeaveWindow)
                ClampToWindow();

        }

        private static void ClampToWindow()
        {
            var minX = Spacing;
            var maxX = SceneView.currentDrawingSceneView.position.size.x - DragButtonSize.x - Spacing - ScreenDragMargin;

            var minY = Spacing;
            var maxY = SceneView.currentDrawingSceneView.position.size.y - DragButtonSize.y - Spacing - ScreenDragMargin;

            _draggedPosOffset.x = Mathf.Clamp(_draggedPosOffset.x, minX, maxX);
            _draggedPosOffset.y = Mathf.Clamp(_draggedPosOffset.y, minY, maxY);
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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GamePack.CustomAttributes.Attributes;
using Shared.EditorDrawer;
using UnityEditor;
using UnityEngine;

namespace GamePack.CustomAttributes
{
    internal static class HandleDrawing
    {
        private struct ComponentHandleInfo
        {
            public Component Component;
            public List<FieldHandleInfo> HandleInfos;
        }
        
        private class FieldHandleInfo
        {
            public SpaceType SpaceType;
            public FieldInfo FieldInfo;
            public Component Component;
            private bool _isDrawing;

            public bool IsLocal => SpaceType is SpaceType.Local;
            
            public T GetValue<T>()
            {
                if (FieldInfo.GetValue(Component) is T)
                    return (T) FieldInfo.GetValue(Component);
                return default;
            }

            public string ValueString => FieldInfo.GetValue(Component).ToString();
            public bool DrawAlways { get; set; }

            public bool IsDrawing
            {
                get => DrawAlways || _isDrawing;
                set => _isDrawing = value;
            }

            public void SetValue(Vector3 position)
            {
                FieldInfo.SetValue(Component, position);
            }
        }

        private static readonly List<ComponentHandleInfo> ComponentHandleInfos = new();

        private static GUIStyle _infoStyle;

        private static GUIStyle InfoStyle
        {
            get
            {
                if (_infoStyle == null)
                {
                    _infoStyle = new GUIStyle(EditorDrawerSystem.Config.ScreenInfoStyle.GUIStyle);
                    _infoStyle.hover = _infoStyle.normal;
                }
                
                return _infoStyle;
            }
        }
        
        private static GUIStyle _buttonStyle;

        private static GUIStyle ButtonStyle
        {
            get
            {
                if (_buttonStyle == null)
                {
                    _buttonStyle = new GUIStyle(EditorDrawerSystem.Config.ScreenInfoStyle.GUIStyle);
                }

                return _buttonStyle;
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            Selection.selectionChanged += UpdateSelection;
            SceneView.duringSceneGui += DrawGuiOnScene;
            // EditorWindow.GetWindow<SceneView>().size
        }

        private static void DrawGuiOnScene(SceneView obj)
        {
            foreach (var componentHandleInfo in ComponentHandleInfos)
            {
                if(!componentHandleInfo.Component) continue;
                if(!componentHandleInfo.Component.gameObject.activeInHierarchy) continue;   // Don't draw if gameobject is inactive
                
                var componentLocalLabelStringBuilder = new StringBuilder();
                foreach (var drawingFieldInfo in componentHandleInfo.HandleInfos)
                {
                    if(!drawingFieldInfo.IsDrawing) continue;
                    
                    var labelStr = $"<b>{drawingFieldInfo.FieldInfo.Name}</b>\n{drawingFieldInfo.ValueString}";

                    var valuePos = drawingFieldInfo.GetValue<Vector3>();
                    var valuePosExists = valuePos != Vector3.zero;

                    if (!valuePosExists)
                    {
                        if (componentLocalLabelStringBuilder.Length > 0)
                            componentLocalLabelStringBuilder.Append("\n");
                        componentLocalLabelStringBuilder.Append(labelStr);
                    }
                    else
                    {
                        var drawingScope = drawingFieldInfo.IsLocal
                            ? new Handles.DrawingScope(drawingFieldInfo.Component.transform.localToWorldMatrix) 
                            : new Handles.DrawingScope(Matrix4x4.identity);
                    
                        using (drawingScope)
                        {
                            var newPos = Handles.PositionHandle(valuePos, Quaternion.identity);
                            drawingFieldInfo.SetValue(newPos);
                            Handles.DrawWireCube(newPos, new Vector3(.2f, .2f, .2f));
                            
                            var worldPos = drawingFieldInfo.IsLocal
                                ? drawingFieldInfo.Component.transform.TransformPoint(newPos)
                                : newPos;
                            
                            DrawLabel(worldPos, labelStr, componentHandleInfo.Component, isButton: drawingFieldInfo.DrawAlways);
                        }
                    }
                    
                }
                // Draw non position Labels
                if (componentLocalLabelStringBuilder.Length > 0)
                    DrawLabel(componentHandleInfo.Component.transform.position,
                        componentLocalLabelStringBuilder.ToString(), componentHandleInfo.Component, true);
            }
            
        }

        private static void DrawLabel(Vector3 position, string labelStr, Component component, bool isButton)
        {
            Handles.BeginGUI();
            var buttonPos = SceneViewWorldToScreenPoint(SceneView.currentDrawingSceneView, position);
            var size = InfoStyle.CalcSize(new GUIContent(labelStr));

            if (isButton 
                && GUI.Button(
                    new Rect(buttonPos.x, buttonPos.y, size.x, size.y),
                    new GUIContent(labelStr), ButtonStyle))
                Selection.activeGameObject = component.gameObject;
            else GUI.Label(
                    new Rect(buttonPos.x, buttonPos.y, size.x, size.y),
                    new GUIContent(labelStr), InfoStyle);

            Handles.EndGUI();
        }

        public static void UpdateSelection()
        {
            var sel = Selection.gameObjects;

            foreach (var info in ComponentHandleInfos)
            foreach (var handleInfo in info.HandleInfos)
            {
                if (handleInfo.DrawAlways) continue;

                var handleInfoGameObject = handleInfo.Component.gameObject;
                if (!handleInfoGameObject) continue;

                var isSelected = Array.IndexOf(sel, handleInfoGameObject) >= 0;
                handleInfo.IsDrawing = isSelected;
            }
        }

        public static void AddInfo(FieldInfo fieldInfo, Component component, SpaceType spaceType, bool drawAlways = false)
        {
            var handleInfo = new FieldHandleInfo {Component = component, FieldInfo = fieldInfo, SpaceType = spaceType, DrawAlways = drawAlways};
            var existingComponentFieldsIndex = ComponentHandleInfos.FindIndex(infos => infos.Component == component);
            if (existingComponentFieldsIndex >= 0)
                ComponentHandleInfos[existingComponentFieldsIndex].HandleInfos.Add(handleInfo);
            else
                ComponentHandleInfos.Add(new ComponentHandleInfo
                    {Component = component, HandleInfos = new List<FieldHandleInfo> {handleInfo}});
        }

        public static void Clear()
        {
            ComponentHandleInfos.Clear();
        }

        /// https://forum.unity.com/threads/pixel-perfect-conversion-from-world-space-to-sceneview-ui-coordinates.714065/
        private static Vector3 SceneViewWorldToScreenPoint(SceneView sv, Vector3 worldPos)
        {
            var style = (GUIStyle)"GV Gizmo DropDown";
            var ribbon = style.CalcSize(sv.titleContent);
 
            var svCorrectSize = sv.position.size;
            svCorrectSize.y -= ribbon.y; //exclude this nasty ribbon
 
            // gives coordinate inside SceneView context.
            // WorldToViewportPoint() returns 0-to-1 value, where 0 means 0% and 1.0 means 100% of the dimension
            Vector3 pointInView = sv.camera.WorldToViewportPoint(worldPos);
            Vector3 pointInSceneView = pointInView * svCorrectSize;
            var p1 = pointInSceneView;
            p1.y = sv.position.height - p1.y;
 
            return p1;
        }
    }
}
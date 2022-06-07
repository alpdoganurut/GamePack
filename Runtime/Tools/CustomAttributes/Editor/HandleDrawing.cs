using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GamePack.CustomAttributes.Attributes;
using GamePack.DebugDrawSystem.DrawingMethods;
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
                else return default;
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
        // private static readonly List<ComponentHandleInfos> DrawingFieldInfos = new();
        // private static readonly List<ComponentHandleInfos> AlwaysDrawingFieldInfos = new();

        private static GUIStyle _infoStyle;

        private static GUIStyle InfoStyle =>
            _infoStyle ??= new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(2, 2, 2, 2),
                richText = true
            };

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            // PlayerLoopUtilities.AppendToPlayerLoop<Update>(typeof(HandleDrawing), Update);
            Selection.selectionChanged += UpdateSelection;
            SceneView.duringSceneGui += DrawGuiOnScene;
            
            // _infoStyle = new GUIStyle(GUI.skin.box);
            // _infoStyle.alignment = TextAnchor.MiddleCenter;
        }

        private static void DrawGuiOnScene(SceneView obj)
        {
            foreach (var componentFields in ComponentHandleInfos)
            {
                if(!componentFields.Component) continue;
                if(!componentFields.Component.gameObject.activeInHierarchy) continue;   // Don't draw if gameobject is inactive
                
                var componentLocalLabelStringBuilder = new StringBuilder();
                foreach (var drawingFieldInfo in componentFields.HandleInfos)
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
                            // Gizmos.DrawSphere(valuePos, .2f);
                            var newPos = Handles.PositionHandle(valuePos, Quaternion.identity);
                            drawingFieldInfo.SetValue(newPos);
                            Handles.DrawWireCube(newPos, new Vector3(.2f, .2f, .2f));
                            DrawLabel(newPos, labelStr);
                        }
                    }
                    
                }
                // Draw non position Labels
                if (componentLocalLabelStringBuilder.Length > 0)
                    DrawLabel(componentFields.Component.transform.position,
                        componentLocalLabelStringBuilder.ToString());
            }
            
        }

        private static void DrawLabel(Vector3 position, string labelStr)
        {
            Handles.Label(
                position + (Vector3.down * .2f), labelStr, InfoStyle);
        }

        public static void UpdateSelection()
        {
            // DrawingFieldInfos.Clear();
            
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

        public static void Clear()
        {
            ComponentHandleInfos.Clear();
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
            // if (drawAlways)
                // AlwaysDrawingFieldInfos.Add(handleInfo);
            // else
                // HandleInfos.Add(handleInfo);
        }
    }
}
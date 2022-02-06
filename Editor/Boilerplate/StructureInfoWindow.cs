using System.Collections.Generic;
using GamePack.Boilerplate.Structure;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Boilerplate
{
    public class StructureInfoWindow: OdinEditorWindow
    {
        
        [ShowInInspector] private bool ShowViewNames
        {
            get => StructureManager.ShowViewNames;
            set => StructureManager.ShowViewNames = value;
        }
        
        [ShowInInspector] private bool ShowViewAxes
        {
            get => StructureManager.ShowViewAxes;
            set => StructureManager.ShowViewAxes = value;
        }

        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Boxed), HideInEditorMode] 
        private static List<ControllerBase> Controllers => StructureManager.Controllers;
        
        [ShowInInspector, InlineEditor(InlineEditorModes.GUIAndPreview), HideInEditorMode] 
        private static List<View> Views => StructureManager.Views;

        [MenuItem("Window/Controller Info Window")]
        public static void ShowWindow()
        {
            GetWindow<StructureInfoWindow>();
        }
    }
}
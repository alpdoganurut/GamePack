using System.Collections.Generic;
using GamePack.Boilerplate.Structure;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Boilerplate
{
    public class StructureInfoWindow: OdinEditorWindow
    {
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Boxed), HideInEditorMode] 
        private static List<ControllerBase> Controllers => StructureInfo.Controllers;
        
        [ShowInInspector, InlineEditor(InlineEditorModes.GUIAndPreview), HideInEditorMode] 
        private static List<View> Views => StructureInfo.Views;
        
        /*[ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Boxed), HideInEditorMode] 
        private static List<ControllerBase> _controllers;
        [ShowInInspector, InlineEditor(InlineEditorModes.GUIAndPreview), HideInEditorMode] 
        private static List<View> _views;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode()
        {
            _controllers = new List<ControllerBase>();
            _views = new List<View>();
        }
        
        private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _controllers.AddRange(FindAllObjects.InScene<ControllerBase>());
            _views.AddRange(FindAllObjects.InScene<View>());
        }*/

        [MenuItem("Window/Controller Info Window")]
        public static void ShowWindow()
        {
            GetWindow<StructureInfoWindow>();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace GamePack.Boilerplate.Structure
{
    public static class StructureInfo
    {
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Boxed), HideInEditorMode] 
        private static readonly List<ControllerBase> MyControllers = new List<ControllerBase>();
        
        [ShowInInspector, InlineEditor(InlineEditorModes.GUIAndPreview), HideInEditorMode] 
        private static readonly List<View> MyViews = new List<View>();

        public static List<ControllerBase> Controllers => MyControllers;

        public static List<View> Views => MyViews;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
        }

        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode()
        {
            MyControllers.Clear();
            MyViews.Clear();
        }
        
        private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            RefreshLists();
        }

        private static void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            RefreshLists();

        }

        private static void RefreshLists()
        {
            Controllers.Clear();
            Views.Clear();

            Controllers.AddRange(FindAllObjects.InScene<ControllerBase>());
            Views.AddRange(FindAllObjects.InScene<View>());
        }
    }
}
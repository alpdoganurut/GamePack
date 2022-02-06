using System.Collections.Generic;
using System.Linq;
using GamePack.Logging;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace GamePack.Boilerplate.Structure
{
    public static class StructureManager
    {
        public static List<ControllerBase> Controllers { get; } = new List<ControllerBase>();

        public static List<View> Views { get; } = new List<View>();

        public static bool ShowViewNames = false;
        public static bool ShowViewAxes = false;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
            
            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(ManagedLog), Update);
        }

        private static void Update()
        {
            if(!Application.isPlaying) return;
            
            foreach (var view in Views)
            {
                if(view)    // TODO: This is hacky, manage destroyed views and controllers
                    view.InternalUpdate();
            }
        }

        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode()
        {
            Controllers.Clear();
            Views.Clear();
        }
        
        private static void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            AddAllComponentsInScene<View>(scene);
            AddAllComponentsInScene<ControllerBase>(scene);
            // TODO: We need to keep track of newly instantiated views and destroyed ones
        }

        private static void AddAllComponentsInScene<T>(Scene arg0) where T: Component
        {
            var allObjectsInScene = arg0.GetRootGameObjects();
            foreach (var gameObject in allObjectsInScene)
            {
                var components = gameObject.GetComponentsInChildren<T>();

                switch (components)
                {
                    case View[] views:
                        Views.AddRange(views);
                        foreach (var view in views)
                        {
                            view.InternalOnLoad(); // TODO: This must be called for newly instantiated views as well
                        }
                        break;
                    case ControllerBase[] controllers:
                        Controllers.AddRange(controllers);
                        break;
                }
            }
        }

        private static void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            Views.RemoveAll(view => !view);
            Controllers.RemoveAll(controllerBase => !controllerBase);
        }

        /*private static void RefreshLists()
        {
            Controllers.Clear();
            Views.Clear();

            Controllers.AddRange(FindAllObjects.InScene<ControllerBase>());
            Views.AddRange(FindAllObjects.InScene<View>());
        }*/
    }
}
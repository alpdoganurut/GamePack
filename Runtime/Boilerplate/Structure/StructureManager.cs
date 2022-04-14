
#if USING_SHAPES
using Shapes;
using Draw = GamePack.Utilities.DebugDrawSystem.DrawingMethods.Draw;
#endif

using System.Collections.Generic;
using GamePack.Logging;
using GamePack.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
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

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoadMethod()
        {
            InitializeOnLoadMethod();
        }
#endif

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.Log($"{nameof(StructureManager)}.{nameof(InitializeOnLoadMethod)}", ManagedLog.Type.Structure);
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
            
            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(ManagedLog), OnUpdate);
         
            // SceneManagerOnSceneLoaded doesn't get called for initial scene in builds, we need to call it on load to capture Managed Objects
            #if !UNITY_EDITOR
            var activeScene = SceneManager.GetActiveScene();
            AddAllManagedObjectsInScene(activeScene);
            #endif
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode()
        {
            ManagedLog.Log($"{nameof(StructureManager)}.{nameof(InitializeOnEnterPlayMode)}", ManagedLog.Type.Structure);
            Controllers.Clear();
            Views.Clear();
        }
#endif
        
        private static void OnUpdate()
        {
            if(!Application.isPlaying) return;
            
            // Update views, and draw 
            foreach (var view in Views)
            {
                if (!view) continue; // TODO: This is hacky, manage destroyed views and controllers (but this must stay probably)
                view.Internal_OnUpdate();
                DrawView(view);
            }
        }
        
        // This doesn't get called for initial scene in builds.
        private static void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            ManagedLog.Log($"{nameof(StructureManager)}.{nameof(SceneManagerOnSceneLoaded)}", ManagedLog.Type.Structure);
            AddAllManagedObjectsInScene(scene);
            // TODO: We need to keep track of newly instantiated views and destroyed ones
        }

        private static void AddAllManagedObjectsInScene(Scene scene)
        {
            AddAllComponentsInScene<View>(scene);
            AddAllComponentsInScene<ControllerBase>(scene);
        }

        private static void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            ManagedLog.Log($"{nameof(StructureManager)}.{nameof(SceneManagerOnSceneUnloaded)}", ManagedLog.Type.Structure);
            Views.RemoveAll(view => !view);
            Controllers.RemoveAll(controllerBase => !controllerBase);
        }

        internal static void RegisterViewOrController(Object obj)
        {
            switch (obj)
            {
                case View view:
                    Views.Add(view);
                    break;
                case ControllerBase controllerBase:
                    Controllers.Add(controllerBase);
                    break;
                default:
                    return;
            }
            ManagedLog.Log($"Registered new {obj.GetType().Name} ({obj.GetScenePath()})", ManagedLog.Type.Structure);
        }
        
        private static void AddAllComponentsInScene<T>(Scene arg0) where T: Component
        {
            var addedCount = 0;
            var allObjectsInScene = arg0.GetRootGameObjects();
            foreach (var gameObject in allObjectsInScene)
            {
                var components = gameObject.GetComponentsInChildren<T>();
                addedCount += components.Length;
                switch (components)
                {
                    case View[] views:
                        Views.AddRange(views);
                        foreach (var view in views)
                        {
                            view.Internal_OnLoad();
                        }
                        break;
                    case ControllerBase[] controllers:
                        Controllers.AddRange(controllers);
                        break;
                }
            }
            
            if(addedCount > 0)
                ManagedLog.Log($"Added {addedCount} {typeof(T).Name}", ManagedLog.Type.Structure);
        }
        
        private static void DrawView(View view)
        {
#if USING_SHAPES
            if (ShowViewAxes)
                Draw.Axis(Vector3.zero, view.transform);
            if (ShowViewNames)
                Draw.Text(Vector3.zero, $"{view.GetScenePath()} ({view.GetType().Name})",
                    color: view.IsVisible ? Color.white : Colors.DimGray,
                    textAlign: TextAlign.Bottom,
                    fontSize: .5f,
                    localTransform: view.transform);
#endif
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
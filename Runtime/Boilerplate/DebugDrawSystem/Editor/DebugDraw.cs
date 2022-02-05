#define DEBUG_DRAW

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Shapes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GamePack.Utilities.DebugDrawSystem
{
    public static class DebugDraw
    {
        private struct TimedDrawing
        {
            public IDrawing Drawing { get; }
            public float EndTime { get; }

            public TimedDrawing(IDrawing drawing, float endTime)
            {
                Drawing = drawing;
                EndTime = endTime;
            }
        }
        
#pragma warning disable CS0414
        [ReadOnly] private static bool _isInit;
#pragma warning restore CS0414

        private static RenderPassEvent _drawOrder = RenderPassEvent.AfterRendering;
        // private static CameraEvent _drawOrder = CameraEvent.AfterForwardAlpha;
        
        private static bool _showInGameViewWhenNotPlaying = false;
        
        private static bool _logEvents = false;
        private static bool _logDrawUpdates = false;
        
        private const bool SHOW_ORIGIN_DATA = true;
        private static bool _showInfo = false;

        private static readonly List<IDrawing> DefaultDrawings = new List<IDrawing>();
        private static readonly List<IDrawing> WaitingFrameDrawings = new List<IDrawing>();
        private static readonly List<IDrawing> FrameDrawings = new List<IDrawing>();
        private static readonly List<TimedDrawing> TimedDrawings = new List<TimedDrawing>();

        private static DebugDrawSceneHelper _helper;
        private static string _infoText;

        internal static void NewDrawing(IDrawing drawing, float duration = -1)
        {
            if(duration < 0) WaitingFrameDrawings.Add(drawing);
            else TimedDrawings.Add(new TimedDrawing(drawing, duration));
        }
        
        #region Initialization

        
        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            Initialize();
        }

        [Conditional("DEBUG_DRAW")]
        private static void Initialize()
        {
            _isInit = true;
            LogEvent($"{nameof(DebugDraw)}.InitializeOnLoadMethod");
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerOnSceneLoaded;
            ListenCamera();

            PlayerLoopUtilities.AppendToPlayerLoop<PostLateUpdate>(typeof(DebugDraw), CustomLateUpdate);
            EnsureOfHelper();

            // Default Drawings
            if (SHOW_ORIGIN_DATA)
            {
                DefaultDrawings.Add(new TextDrawing(new Vector3(.02f, .01f), nameof(DebugDraw),
                    new Color(1f, 0.84f, 0.2f), fontSize: .5f,
                    textAlign: TextAlign.BottomLeft)); // Not working consistently, maybe just in edit mode
                DefaultDrawings.Add(new PointDrawing(Vector3.zero, .5f, 0.02f, new Color(1f, 0.84f, 0.2f, .5f)));
            }
        }

        private static void ListenCamera()
        {
#if (SHAPES_URP || SHAPES_HDRP)
#if UNITY_2019_1_OR_NEWER
        RenderPipelineManager.beginCameraRendering += DrawShapesSrp;
        void DrawShapesSrp( ScriptableRenderContext ctx, Camera cam ) => OnCameraPreRender( cam );
#else
				public virtual void OnEnable() => Debug.LogWarning( "URP/HDRP immediate mode doesn't really work pre-Unity 2019.1, as there is no OnPreRender or beginCameraRendering callback" );
#endif
#else
		public virtual void OnEnable() => Camera.onPreRender += OnCameraPreRender;
		public virtual void OnDisable() => Camera.onPreRender -= OnCameraPreRender;
#endif   
            // Camera.onPreRender += OnCameraPreRender;
        }

        /*
        private static void AppendToPlayerLoop()
        {
            var defaultSystems = PlayerLoop.GetCurrentPlayerLoop();
            var customLateUpdate = new PlayerLoopSystem()
            {
                updateDelegate = CustomLateUpdate,
                type = typeof(DebugDraw)
            };
            AddToSystem<PostLateUpdate>(ref defaultSystems, customLateUpdate);
            PlayerLoop.SetPlayerLoop(defaultSystems);
        }

        private static bool AddToSystem<TSystemToAdd>(ref PlayerLoopSystem system, PlayerLoopSystem addition)
        {
            if (system.type == typeof(TSystemToAdd))
            {
                var oldList = system.subSystemList;
                var listLength = oldList == null ? 1 : oldList.Length + 1;
                var newList =  new PlayerLoopSystem[listLength];
                oldList?.CopyTo(newList, 0);
                newList[listLength - 1] = addition;
                system.subSystemList = newList;
                return true;
            }
            
            if (system.subSystemList != null)
            {
                for (var i = 0; i < system.subSystemList.Length; i++)
                {
                    if (AddToSystem<TSystemToAdd>(ref system.subSystemList[i], addition))
                    {
                        return true;
                    }
                }
            }
            return false;
        }*/

        private static void EditorSceneManagerOnSceneLoaded(Scene arg0, Scene arg1)
        {
            LogEvent($"{nameof(DebugDraw)}.EditorSceneManagerOnSceneLoaded");

            EnsureOfHelper();
        }

        private static void EnsureOfHelper()
        {
            // Try to find helper in scene
            if (!_helper)
            {
                var helpers = Resources.FindObjectsOfTypeAll<DebugDrawSceneHelper>(); // TODO: This searches also in Assets

                if (helpers.Length > 1)
                {
                    LogEvent("More than one helper is existent for DebugDraw! Destroying excessive instances.");

                    for (var index = 1; index < helpers.Length; index++)
                    {
                        var helper = helpers[index];
                        Object.Destroy(helper);
                    }
                }

                if (helpers.Length == 1)
                {
                    _helper = helpers[0];
                    LogEvent("Found helper!");
                }
            }

            // Create one if not found any
            if (!_helper)
            {
                _helper = new GameObject($"{nameof(DebugDrawSceneHelper)} {Random.Range(0, 10000)}")
                    .AddComponent<DebugDrawSceneHelper>();
                _helper.gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                EditorApplication.RepaintHierarchyWindow();
                EditorApplication.DirtyHierarchyWindowSorting();

                LogEvent($"Created {nameof(DebugDrawSceneHelper)}");
            }
        }


        #endregion

        #region Drawing

        private static void OnCameraPreRender( Camera cam ) {
            
            switch( cam.cameraType) {
                case CameraType.Preview:
                case CameraType.Reflection:
                    return; // Don't render in preview windows or in reflection probes in case we run this script in the editor
                case CameraType.SceneView:
                    if(!SceneView.lastActiveSceneView.drawGizmos) return;
                    break;
                case CameraType.Game:
                    if(!_showInGameViewWhenNotPlaying && !Application.isPlaying) return;
                    break;
                case CameraType.VR:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            DrawShapes(cam);
        }

        private static void DrawShapes(Camera cam)
        {
            LogUpdate($"{nameof(DebugDraw)}.DrawShapes cam: {cam.name} waiting: {WaitingFrameDrawings.Count} frame: {FrameDrawings.Count} timed: {TimedDrawings.Count}");
            
            using (Draw.Command(cam, _drawOrder))
            {
                foreach (var drawing in DefaultDrawings)
                {
                    drawing.Draw();
                }

                foreach (var drawing in FrameDrawings)
                {
                    drawing.Draw();
                }

                foreach (var timedDrawing in TimedDrawings)
                {
                    timedDrawing.Drawing.Draw();
                }
            }
        }

        #endregion

        #region Drawing Management

        private static void CustomLateUpdate()
        {
            LogUpdate($"{nameof(DebugDraw)}.CustomLateUpdate time:{Time.time}");
            
            // Return if not playing
            if(!Application.isPlaying) return;
            
            FrameUpdate();
            TimedDrawings.RemoveAll(timedDrawing => timedDrawing.EndTime < Time.time);
        }

        internal static void OnHelperDrawGizmos()
        {
            LogUpdate($"{nameof(DebugDraw)}.OnDrawGizmos");
            
            // Return if play mode and not paused
            if (Application.isPlaying && !EditorApplication.isPaused) return;
            
            FrameUpdate();
        }

        private static void FrameUpdate()
        {
            if(_showInfo)
                CreateInfoTextDrawing();

            LogUpdate($"{nameof(DebugDraw)}.{nameof(FrameUpdate)}");
            FrameDrawings.Clear();
            FrameDrawings.InsertRange(0, WaitingFrameDrawings);
            WaitingFrameDrawings.Clear();
        }

        #endregion

        #region Logging

        private static void LogEvent(object msg)
        {
            if(!_logEvents) return;
            Debug.Log(msg);
        }
        
        private static void LogUpdate(object msg)
        {
            if(!_logDrawUpdates) return;
            Debug.Log(msg);
        }

        #endregion

        private static void CreateInfoTextDrawing()
        {
            _infoText = "";
            _infoText += SceneView.lastActiveSceneView.sceneViewState.alwaysRefreshEnabled ? "Animating" : "Not Animating";
            // _infoText += "\n";
            // _infoText += _drawOrder == CameraEvent.BeforeImageEffects ? "Not AlwaysOnTop" : "AlwaysOnTop";   // TODO: Need to determine exact values causes to be always on top.
            NewDrawing(new TextDrawing(new Vector3(-.02f, -.01f), _infoText, new Color(1f, 0.84f, 0.2f), .5f, TextAlign.TopRight));  // Not working consistently, maybe just in edit mode
        }
    }
}
#define DEBUG_DRAW

#if USING_SHAPES

#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GamePack.Logging;
using GamePack.Utilities;
using Shapes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GamePack.DebugDrawSystem
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
        
        private struct OwnedDrawing
        {
            public IDrawing Drawing { get; }
            public Object Owner { get; }

            public OwnedDrawing(IDrawing drawing, Object owner)
            {
                Drawing = drawing;
                Owner = owner;
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
        private static readonly List<TimedDrawing> TimedDrawings = new List<TimedDrawing>();
        private static readonly List<OwnedDrawing> OwnedDrawings = new List<OwnedDrawing>();
        private static readonly List<IDrawing> FrameDrawingsBuffer = new List<IDrawing>();
        private static readonly List<IDrawing> FrameDrawings = new List<IDrawing>();

        // private static DebugDrawSceneHelper _helper;
        private static string _infoText;
        
        internal static Camera PlayModMainCamera { get; private set; }

        internal static void NewDrawing(IDrawing drawing, float duration = -1, Object owner = null)
        {
            if(owner) OwnedDrawings.Add(new OwnedDrawing(drawing, owner));
            else if(duration <= 0) FrameDrawingsBuffer.Add(drawing);
            else TimedDrawings.Add(new TimedDrawing(drawing, Time.time + duration));
        }
        
        #region Initialization

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        private static void InitializeOnLoad()
        {
            Initialize();
        }

        [Conditional("DEBUG_DRAW")]
        private static void Initialize()
        {
            _isInit = true;
            SceneManager.sceneLoaded += OnEditorSceneManagerOnSceneLoaded;
            ListenCamera();

            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(DebugDraw), CustomLateUpdate);

            // Default Drawings
            if (SHOW_ORIGIN_DATA)
            {
                DefaultDrawings.Add(new PointDrawing(Vector3.zero, .5f, 0.02f, isAxisColored: true));
                DefaultDrawings.Add(new TextDrawing(new Vector3(.02f, .01f), nameof(DebugDraw),
                    new Color(1f, 0.84f, 0.2f), fontSize: .5f,
                    textAlign: TextAlign.BottomLeft, lookAtCamera: false)); // Not working consistently, maybe just in edit mode
            }
        }

        private static void OnEditorSceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode mode)
        {
            PlayModMainCamera = Camera.main;
        }

        private static void ListenCamera()
        {
#if (SHAPES_URP || SHAPES_HDRP)
        RenderPipelineManager.beginCameraRendering += DrawShapesSrp;
        void DrawShapesSrp( ScriptableRenderContext ctx, Camera cam ) => OnCameraPreRender( cam );
#endif
        }

        #endregion

        #region Drawing

        private static void OnCameraPreRender( Camera cam ) {
            
            switch( cam.cameraType) {
                case CameraType.Preview:
                case CameraType.Reflection:
                    return; // Don't render in preview windows or in reflection probes in case we run this script in the editor
                case CameraType.SceneView:
#if UNITY_EDITOR
                    if(!SceneView.lastActiveSceneView.drawGizmos) return;
#endif
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
            LogUpdate($"{nameof(DebugDraw)}.DrawShapes cam: {cam.name} waiting: {FrameDrawingsBuffer.Count} frame: {FrameDrawings.Count} timed: {TimedDrawings.Count}");
            
            using (Draw.Command(cam, _drawOrder))
            {
                foreach (var drawing in DefaultDrawings)
                {
                    drawing.Draw(cam);
                }

                foreach (var drawing in FrameDrawings)
                {
                    drawing.Draw(cam);
                }

                foreach (var timedDrawing in TimedDrawings)
                {
                    timedDrawing.Drawing.Draw(cam);
                }

                foreach (var ownedDrawing in OwnedDrawings)
                {
                    ownedDrawing.Drawing.Draw(cam);
                }
            }
        }

        #endregion

        #region Drawing Management

        private static void CustomLateUpdate()
        {
            LogUpdate($"{nameof(DebugDraw)}.CustomLateUpdate time:{Time.time}");
            
            FrameUpdate();
            TimedDrawings.RemoveAll(timedDrawing => timedDrawing.EndTime < Time.time);
            OwnedDrawings.RemoveAll(drawing => !drawing.Owner);
        }

        private static void FrameUpdate()
        {
            if(_showInfo)
                CreateInfoTextDrawing();

            LogUpdate($"{nameof(DebugDraw)}.{nameof(FrameUpdate)}");
            FrameDrawings.Clear();
            FrameDrawings.InsertRange(0, FrameDrawingsBuffer);
            FrameDrawingsBuffer.Clear();
        }

        #endregion

        #region Logging

        private static void LogEvent(object msg)
        {
            if(!_logEvents) return;
            ManagedLog.Log(msg, ManagedLog.Type.Structure);
        }
        
        private static void LogUpdate(object msg)
        {
            if(!_logDrawUpdates) return;
            ManagedLog.Log(msg);
        }

        #endregion

        private static void CreateInfoTextDrawing()
        {
            _infoText = "";
#if UNITY_EDITOR
            _infoText += SceneView.lastActiveSceneView.sceneViewState.alwaysRefreshEnabled ? "Animating" : "Not Animating";
#endif
            // _infoText += "\n";
            // _infoText += _drawOrder == CameraEvent.BeforeImageEffects ? "Not AlwaysOnTop" : "AlwaysOnTop";   // TODO: Need to determine exact values causes to be always on top.
            NewDrawing(new TextDrawing(new Vector3(-.02f, -.01f), _infoText, new Color(1f, 0.84f, 0.2f), .5f, TextAlign.TopRight));  // Not working consistently, maybe just in edit mode
        }
    }
}

#endif
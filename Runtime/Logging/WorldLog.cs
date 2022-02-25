#if USING_SHAPES

using GamePack.TimerSystem;
using GamePack.Utilities.DebugDrawSystem.DrawingMethods;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.Logging
{
    public static class WorldLog
    {
        private const float Offset = 1;
        private const float Duration = 4f;
        private static readonly Color DefaultColor = Colors.Orangered;
        private static EasingFunction.Ease Easing = EasingFunction.Ease.EaseOutSine;
        private static Camera _camera;
        private const float LogOnScreenCameraZ = 5f;

        private static Camera Camera => _camera ? _camera : _camera = Camera.main;

        public static void Log(object msg, Vector3? pos = null, Transform localTransform = null, Color? color = null)
        {
            // var isFollowing = (bool) transform;

            // if (!isFollowing) Assert.IsTrue(pos.HasValue, "pos or transform must be supplied.");

            var upwardsMovement = Vector3.up * Offset;

            var startPos = pos ?? Vector3.zero;
            var finalPos = startPos + upwardsMovement;
            
            var startColor = color ?? DefaultColor;
            var endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

            ManagedLog.Log(msg, ManagedLog.Type.Default, localTransform ? localTransform.gameObject : null, color);

            new Operation("World Log", duration: Duration, ease: Easing,
                updateAction: val =>
                {
                    // var updateStartPos = isFollowing ? transform.position + (transformOffset ?? Vector3.zero) : startPos;
                    // var updateFinalPos = isFollowing ? transform.position + (transformOffset ?? Vector3.zero) + upwardsMovement : finalPos;

                    var p = Vector3.Lerp(startPos, finalPos, val);
                    
                    const float cThreshold = .5f;
                    var cVal = (val - cThreshold) * (1/cThreshold);
                    if (cVal < 0) cVal = 0;
                    var c = Color.Lerp(startColor, endColor, cVal);

                    Draw.Text(p, msg.ToString(), c, localTransform: localTransform);
                    
                    // if (isFollowing)
                    // {
                    // }
                    // else
                    // {
                        // Draw.Text(p, msg.ToString(), c);
                    // }
                }).Start(true);
        }

        public static void OnScreen(object msg, Color? color = null)
        {
            if (!Application.isPlaying)
            {
                ManagedLog.LogError("Can't log on screen in Edit Mode.");
                return;
            }

            // var pos = Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth / 2f, Camera.pixelHeight / 2f,
                // LogOnScreenCameraZ));
            var cameraTransform = Camera.transform;
            // var offset = pos - cameraTransform.position;
            var localPos = cameraTransform.InverseTransformPoint(
                Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth / 2f, Camera.pixelHeight / 2f,
                    LogOnScreenCameraZ)));
                
            Log(msg, pos: localPos, localTransform: cameraTransform);
        }
    }
}

#endif
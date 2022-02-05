using GamePack.Timer;
using GamePack.Utilities.DebugDrawSystem.DrawingMethods;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack.Logging
{
    public static class WorldLog
    {
        private const float Offset = 1;
        private const float Duration = 1f;
        private static readonly Color DefaultColor = Colors.Orangered;
        private static readonly EasingFunction.Ease Easing = EasingFunction.Ease.EaseInOutSine;

        public static void Log(object msg, Transform transform = null, Vector3? pos = null,  Color? color = null)
        {
            var isFollowing = (bool) transform;
            
            if(!isFollowing) Assert.IsTrue(pos.HasValue, "pos or transform must be supplied.");

            // ReSharper disable once PossibleNullReferenceException
            var startPos = isFollowing ? transform.position : pos.Value;
            var posOffset = Vector3.up * Offset;
            var finalPos = startPos + posOffset;
            var startColor = color ?? DefaultColor;
            var endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
            
            ManagedLog.Log(msg, ManagedLog.Type.Default, transform ? transform.gameObject : null, color);
            
            new Operation("World Log", duration: Duration,  ease: Easing, updateAction: val =>
            {
                var pX = isFollowing ? transform.position.x :   Mathf.Lerp(startPos.x, finalPos.x, val);
                var pY = Mathf.Lerp(startPos.y,  isFollowing ? transform.position.y + posOffset.y : finalPos.y, val);
                var pZ = isFollowing ? transform.position.z : Mathf.Lerp(startPos.z, finalPos.z, val);
                var p = new Vector3(pX, pY, pZ);
                
                var c = Color.Lerp(startColor, endColor, val);
                
                Draw.Text(p, msg.ToString(), c);
                
            }).Start(true);
        }

    }
}
using System;
using GamePack.Logging;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Utilities
{
    public partial class EditorUtilities
    {
        [MenuItem("Utilities/Time/Decrease Time Scale %&Z")]
        private static void DecreaseTimeScale()
        {
            Time.timeScale /= 2f;
            LogTimeScale();
        }
        
        [MenuItem("Utilities/Time/Increase Time Scale %&C")]
        private static void IncreaseTimeScale()
        {
            Time.timeScale *= 2f;
            LogTimeScale();
        }

        [MenuItem("Utilities/Time/Reset Time Scale %&X")]
        private static void ResetTimeScale()
        {
            Time.timeScale = 1f;
            LogTimeScale();
        }

        
        private const float TimeScaleChange = .333f;
        private const float TimeScaleChangeMinInterval = .3f;

        private static void TimeScaleUpdate()
        {
            if (!Application.isPlaying) return;
            // if(Time.time - _lastChangeTime < TimeScaleChangeMinInterval) return;

            float? newTimeScale = null;
            var isOptionalKeyDown = (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl));
            if(!isOptionalKeyDown) return;
            
            if ( Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f )
                newTimeScale = Time.timeScale + Mathf.Sign(Input.mouseScrollDelta.y) * TimeScaleChange;

            if (Input.GetMouseButtonDown(2) && Math.Abs(Time.timeScale - 1) > float.Epsilon)
                newTimeScale = 1;
            
            if(newTimeScale.HasValue)
            {
                // _lastChangeTime = Time.time;
                newTimeScale = Mathf.Clamp(newTimeScale.Value, 0, 100);
                Time.timeScale = newTimeScale.Value;
                LogTimeScale();
            }
        }

        private static void LogTimeScale()
        {
            ManagedLog.Log($"TimeScale: {Time.timeScale}", avoidFrameCount: true);
        }
    }
}
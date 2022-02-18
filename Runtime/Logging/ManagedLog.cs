using System;
using System.Diagnostics;
using UnityEditor;

using UnityEngine;
using System.Linq;
using GamePack.UnityUtilities;
using GamePack.Utilities;

using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace GamePack.Logging
{
    public static class ManagedLog
    {
        public enum Type
        {
            Default, 
            Verbose, 
            Structure, 
            Error,
            Info
        }
        
        private static int _frameCount;
        private static int _lastLogFrameCount;
        private static ManagedLogConfig _config;

        public static ManagedLogConfig Config
        {
            get
            {
                if(!_config) FindConfig();
                return _config;
            }
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        private static void InitializeOnLoadMethod()
        {
            Log($"{nameof(ManagedLog)}.{nameof(InitializeOnLoadMethod)}", Type.Structure);
            
            PlayerLoopUtilities.AppendToPlayerLoop<PostLateUpdate>(typeof(ManagedLog), LateUpdate);
        }

        [Conditional("UNITY_EDITOR")]
        private static void FindConfig()
        {
#if UNITY_EDITOR
            var managedLogConfigs = FindAllObjects.InEditor<ManagedLogConfig>();

            var assetPath = $"Assets/{nameof(ManagedLogConfig)}.asset";
            if (managedLogConfigs.Count > 0)
            {
                _config = managedLogConfigs.First();
            }
            else if (managedLogConfigs.Count == 0)
            {
                _config = AssetDatabase.LoadAssetAtPath<ManagedLogConfig>(assetPath);
            }

            if (!_config)
            {
                Debug.LogError($"Can't find config file for {typeof(ManagedLog)}, creating one!");
                var asset = ScriptableObject.CreateInstance<ManagedLogConfig>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
            }
#endif
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            Log($"{nameof(ManagedLog)}.{nameof(InitializeOnEnterPlayMode)}", Type.Structure);
            
            _frameCount = 0;
            _lastLogFrameCount = 0;
        }
#endif
        
        private static void LateUpdate()
        {
            _frameCount++;
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void Log(object obj, Type type = Type.Default, Object context = null, Color? color = null, bool avoidFrameCount = false)
        {
            var msg = obj.ToString();
            if(Config && Config.LogTypes != null && !Config.LogTypes.Contains(type)) return;  // Log everything if config is not found.
            
            if(Application.isPlaying
               && !avoidFrameCount
               && Config.ShowFrameCount
               && _frameCount > _lastLogFrameCount)
            {
                Debug.Log($"# Frame {_frameCount}");
                _lastLogFrameCount = _frameCount;
            }

            if ((!Config || Config.ShowLogType) && type != Type.Default && type != Type.Error)
            {
                msg = $"[{type.ToString()}]\t{msg}";
            }
            
            if (color.HasValue)
            {
                var colorHex = ColorUtility.ToHtmlStringRGB(color.Value);
                msg = $"<color=#{colorHex}>{msg}</color>";
                Debug.Log(msg);
            }
            else Debug.Log(msg, context);
        }
        
        public static void LogError(object obj, Object context = null)
        {
            var msg = "<color=\"red\">[ERROR]</color>\t" + obj;
            try
            {
                var frame = new StackFrame(1);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                msg += $" ( {type}.{method.Name} )";
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to get stacktrace.");
                Console.WriteLine(e);
            }
            
            // Log(obj, Type.Error, context, Colors.Tomato);
            Log(msg, Type.Error, context);
        }
    }
}
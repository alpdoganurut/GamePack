using System.Linq;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GamePack.Logging
{
    public static class ManagedLog
    {
        public enum Type
        {
            Default, Verbose, Structure, Error
        }
        
        private static int _frameCount;
        private static int _lastLogFrameCount;
        private static ManagedLogConfig _config;

        public static ManagedLogConfig Config => _config;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            PlayerLoopUtilities.AppendToPlayerLoop<PostLateUpdate>(typeof(ManagedLog), LateUpdate);

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
            
            if (!Config)
            {
                Debug.LogError($"Can't find config file for {typeof(ManagedLog)}, creating one!");
                var asset = ScriptableObject.CreateInstance<ManagedLogConfig>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                
                // Selection.activeObject = asset;
                return;
            }
            
        }
        
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            _frameCount = 0;
            _lastLogFrameCount = 0;
            Log($"ManagedLog.InitializeOnEnterPlayMode: {options}", Type.Structure);
        }
        
        private static void LateUpdate()
        {
            _frameCount++;
        }
        
        public static void Log(object obj, Type type = Type.Default, Object context = null, Color? color = null)
        {
            var msg = obj.ToString();
            if(!Config.LogTypes.Contains(type)) return;
            
            if(Application.isPlaying
               && Config.ShowFrameCount
               && _frameCount > _lastLogFrameCount)
            {
                Debug.Log($"Frame {_frameCount}");
                _lastLogFrameCount = _frameCount;
            }

            if (_config.ShowLogType && type != Type.Default)
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
            // Log(obj, Type.Error, context, Colors.Tomato);
            Log( "[ERROR] " + obj.ToString(), Type.Error, context);
        }
    }
}
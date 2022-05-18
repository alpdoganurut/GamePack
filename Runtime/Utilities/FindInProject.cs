using UnityEditor;
using UnityEngine;

namespace GamePack.Utilities
{
    public static class FindInProject
    {
        public static T ByType<T>() where T: Object
        {
            var typeName = typeof(T).Name;
            var guids = AssetDatabase.FindAssets($"t:{typeName}");
            if (guids.Length == 0)
            {
                Debug.LogWarning($"Project contains no {typeName}");
                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            if (guids.Length > 1)
                Debug.LogWarning(
                    $"Found more than one {typeName} in project. Choosing first one ({path}) as instance.");

            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
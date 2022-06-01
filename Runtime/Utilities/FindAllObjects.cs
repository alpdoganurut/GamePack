using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace GamePack.Utilities
{
    /// <summary>
    /// TR: Unity'nin kendi 'Object.FindObjectOfType' methodunun 'disabled GameObjects' dahil olarak çalışan versiyonunu içerir.
    /// Reference: https://docs.unity3d.com/ScriptReference/Resources.FindObjectsOfTypeAll.html
    /// </summary>
    public static class FindAllObjects
    {
        public static T FirstOfChild<T>(Transform parent) where T : Component
        {
            return OfChild<T>(parent).FirstOrDefault();
        }
        
        public static IEnumerable<T> OfChild<T>(Transform parent) where T : Component
        {
            return InScene<T>()
                .Where(handler => handler.transform.IsChildOf(parent));
        }
        
        /// <summary>
        /// TR: Unity'nin kendi 'Object.FindObjectOfType' methodunun 'disabled GameObjects' dahil olarak çalışan versiyonu.
        /// </summary>
        /// <typeparam name="T">Bulunacak GameObject'lerin type'ı.</typeparam>
        /// <returns></returns>
        public static T[] InScene<T>() where T: Object
        {
            return Resources.FindObjectsOfTypeAll<T>()
#if UNITY_EDITOR
                .Where(go => !EditorUtility.IsPersistent(go) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
#endif
                .ToArray();
        }

#if UNITY_EDITOR
        public static T[] InEditor<T>() where T: Object
        {
            return Resources.FindObjectsOfTypeAll<T>()
                .Where(go => EditorUtility.IsPersistent(go) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                .ToArray();
        }
#endif
    }
    
    
}
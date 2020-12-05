using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace GamePack.UnityUtilities.Base
{
    /// <summary>
    /// TR: Unity'nin kendi 'Object.FindObjectOfType' methodunun 'disabled GameObjects' dahil olarak çalışan versiyonunu içerir..
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
        public static List<T> InScene<T>() where T: Component
        {
            
            // foreach (var go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) // Original code from

            return Resources.FindObjectsOfTypeAll<T>()
#if UNITY_EDITOR
                .Where(go => !EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
#endif
                .ToList();
        }

#if UNITY_EDITOR
        public static List<T> InEditor<T>() where T: Object
        {
            return Resources.FindObjectsOfTypeAll<T>()
                .Where(go => EditorUtility.IsPersistent(go) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                .ToList();
        }
#endif
    }
    
    
}
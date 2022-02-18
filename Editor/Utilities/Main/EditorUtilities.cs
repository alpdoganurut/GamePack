#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Utilities
{
    [CreateAssetMenu(fileName = "Base Editor Utilities", menuName = "GamePack/Editor Utilities", order = 0)]
    public partial class EditorUtilities : OdinEditorWindow
    {
        [MenuItem("Utilities/Remove Colliders of Selection")]
        private static void RemoveColliders()
        {
            var sel = Selection.gameObjects;

            var destroyList = new List<Collider>();

            foreach (var obj in sel)
            {
                var colliders = obj.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    destroyList.Add(collider);
                }
            }

            if (EditorUtility.DisplayDialog("Collider Removal", $"Removing {destroyList.Count} colliders.", "OK",
                "Cancel"))
            {
                foreach (var collider in destroyList)
                {
                    DestroyImmediate(collider);
                }
            }
        }

        
        [MenuItem("Utilities/Remove MonoBehaviours of Selection")]
        private static void RemoveMonoBehaviours()
        {
            var sel = Selection.gameObjects;

            var destroyList = new List<MonoBehaviour>();

            foreach (var obj in sel)
            {
                var colliders = obj.GetComponentsInChildren<MonoBehaviour>();
                foreach (var collider in colliders)
                {
                    destroyList.Add(collider);
                }
            }

            if (EditorUtility.DisplayDialog("MonoBehaviour Removal", $"Removing {destroyList.Count} MonoBehaviours.", "OK",
                    "Cancel"))
            {
                foreach (var collider in destroyList)
                {
                    DestroyImmediate(collider);
                }
            }
        }
        
        public static void SortChildrenOf(GameObject go)
        {
            var transform = go.transform;
            var children = transform.Cast<Transform>().OrderBy(child => child.name);

            foreach (var child in children)
            {
                child.SetAsLastSibling();
            }
        }
    }
}
#endif
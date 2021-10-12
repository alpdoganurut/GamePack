using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Utilities
{
    public partial class EditorUtilities
    {
        [MenuItem("Utilities/Selection/Top Most Parent")]
        private static void SelectTopMostParent()
        {
            var selectedObjects = new List<GameObject>();

            foreach (var obj in Selection.gameObjects)
            {
                if (!selectedObjects.Any(o => obj.transform.IsChildOf(o.transform)))
                {
                    selectedObjects.Add(obj);
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        [MenuItem("Utilities/Selection/Parents")]
        private static void SelectParents()
        {
            var selectedObjects = new List<GameObject>();

            foreach (var obj in Selection.gameObjects)
            {
                var go = obj as GameObject;
                if (go != null && go.transform.parent)
                {
                    selectedObjects.Add(go.transform.parent.gameObject);
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        [MenuItem("Utilities/Selection/Children")]
        private static void SelectChildren()
        {
            var selectedObjects = new List<GameObject>();

            foreach (var obj in Selection.gameObjects)
            {
                var go = obj as GameObject;
                if (go != null && go.transform.childCount > 0)
                {
                    // selectedObjects.Add(go.transform.parent.gameObject);
                    selectedObjects.AddRange(go.transform.Cast<Transform>().Select(transform => transform.gameObject));
                }
                else
                {
                    selectedObjects.Add(go);
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        [MenuItem("Utilities/Selection/Child TMP")]
        private static void SelectChildTMP()
        {
            var selectedObjects = new List<GameObject>();

            foreach (var obj in Selection.gameObjects)
            {
                var go = obj as GameObject;
                if (go != null && go.transform.childCount > 0)
                {
                    // selectedObjects.Add(go.transform.parent.gameObject);
                    var tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp)
                        selectedObjects.Add(tmp.gameObject);
                    // selectedObjects.AddRange(go.transform.Cast<Transform>().Select(transform => transform.gameObject));
                }
                else
                {
                    selectedObjects.Add(go);
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        [MenuItem("Utilities/Selection/Select Siblings")]
        private static void SelectSiblings()
        {
            var sel = Selection.gameObjects.ToList();
            if (sel.Count <= 0) return;

            foreach (Transform sibling in sel.FirstOrDefault().transform.parent)
            {
                sel.Add(sibling.gameObject);
            }

            Selection.objects = sel.ToArray();
        }
    }
}
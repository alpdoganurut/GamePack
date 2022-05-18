using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Tools
{
    public partial class EditorTools
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
                if (obj != null && obj.transform.parent)
                {
                    selectedObjects.Add(obj.transform.parent.gameObject);
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
                if (obj != null && obj.transform.childCount > 0)
                {
                    // selectedObjects.Add(go.transform.parent.gameObject);
                    selectedObjects.AddRange(obj.transform.Cast<Transform>().Select(transform => transform.gameObject));
                }
                else
                {
                    selectedObjects.Add(obj);
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
                if (obj != null && obj.transform.childCount > 0)
                {
                    // selectedObjects.Add(go.transform.parent.gameObject);
                    var tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp)
                        selectedObjects.Add(tmp.gameObject);
                    // selectedObjects.AddRange(go.transform.Cast<Transform>().Select(transform => transform.gameObject));
                }
                else
                {
                    selectedObjects.Add(obj);
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        [MenuItem("Utilities/Selection/Select Siblings")]
        private static void SelectSiblings()
        {
            var sel = Selection.gameObjects.ToList();
            if (sel.Count <= 0) return;

            var transformParent = sel.FirstOrDefault()?.transform.parent;
            if (transformParent != null) sel.AddRange(from Transform sibling in transformParent select sibling.gameObject);

            Selection.objects = sel.ToArray();
        }
    }
}
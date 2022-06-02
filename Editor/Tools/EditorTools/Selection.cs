using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
// ReSharper disable CoVariantArrayConversion

namespace GamePack.Editor.Tools
{
    [Serializable]
    public class SelectionHelper
    {
        [Button, MenuItem("Utilities/Selection/Parents"), HorizontalGroup("Selection")]
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

        [Button, MenuItem("Utilities/Selection/Children"), HorizontalGroup("Selection")]
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

        [Button, MenuItem("Utilities/Selection/Siblings"), HorizontalGroup("Selection")]
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
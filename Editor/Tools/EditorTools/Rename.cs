using System;
using System.Linq;
using System.Text.RegularExpressions;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Tools
{
    [Serializable]
    public class Rename
    {
        [SerializeField, HorizontalGroup("Numbering"), PropertyOrder(1)] 
        private string _NumberingSeparator = "_";
        
        [Button, HorizontalGroup("Numbering"), PropertyOrder(0)]
        private void Number()
        {
            var selection = Selection.gameObjects.ToList();
            if(selection.Count == 0) return;
            
            var startParent = selection[0].transform.parent;
            selection = selection
                .Where(o => o.transform.parent == startParent)
                .OrderBy(o => o.transform.GetSiblingIndex())
                .ToList();

            for (var index = 0; index < selection.Count; index++)
            {
                var gameObject = selection[index];
                
                Undo.RecordObject(gameObject, "Number");

                var name = Regex.Replace(gameObject.name, @"(_+)(\d+)$", "");
                name = Regex.Replace(name, @"(_+)(\(\d+\))$", "");
                gameObject.name = $"{name}{_NumberingSeparator}{index}";
            }
        }
        
        [Button]
        private void AddComponentNamesInScene()
        {
            foreach (var gObject in FindAllObjects.InScene<GameObject>()) 
                RenameGameObject(gObject);
        }
        
        [Button]
        private void AddComponentNamesOfSelected()
        {
            foreach (var gObject in Selection.gameObjects) 
                RenameGameObject(gObject);
        }
        
        private void RenameGameObject(GameObject gObject)
        {
            var oldName = gObject.name;
            // Get components (not in UnityEngine namespace)
            var components = gObject.GetComponents<MonoBehaviour>().Where(behaviour =>
            {
                // Filter components with UnityEngine namespace
                var type = behaviour.GetType();
                var baseNameSpace = type.Namespace?.Split('.')[0];

                return baseNameSpace == null || baseNameSpace != "UnityEngine";
            }).ToArray();

            // Return if no component exists
            if (!components.Any()) return;

            Undo.RecordObject(gObject, "BaseEditorUtilities/Rename ");

            var suffix = "";
            foreach (var component in components)
            {
                if (suffix != "") suffix += ", ";
                suffix += $"{component.GetType().Name}";
            }
            // Add braces
            suffix = $" [{suffix}]";

            // Check if name already contains suffix
            if (gObject.name.Length >= suffix.Length &&
                gObject.name.Contains(suffix)) return;

            
            // Trim
            gObject.name = gObject.name.Trim();

            gObject.name += suffix;
            Debug.Log($"Renamed '{oldName}' to '{gObject.name}'");
        }

        private int GetDepth(GameObject go)
        {
            var t = go.transform;
            var depth = 0;
            while (t.parent)
            {
                depth++;
                t = t.parent;
            }

            return depth;
        }
    }
}
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Tools
{
    public partial class EditorTools
    {
        #region Rename

        private const string GenericGameObjectName = "GameObject";

        // [SerializeField, TabGroup("Rename")] private string _Separator = "-";
        [SerializeField, TabGroup("Rename")] private bool _ForceRename;

        [Button, TabGroup("Rename")]
        private void RenameAll()
        {
            foreach (var gObject in FindObjectsOfType<GameObject>()) 
                RenameGameObject(gObject);
        }
        
        [Button, TabGroup("Rename")]
        private void RenameSelected()
        {
            foreach (var gObject in Selection.gameObjects) 
                RenameGameObject(gObject);
        }
        
        private void RenameGameObject(GameObject gObject)
        {
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

            
            // Check if name already contains suffix
            if (!_ForceRename && gObject.name.Length >= suffix.Length &&
                gObject.name.Substring(gObject.name.Length - suffix.Length, suffix.Length) == suffix) return;

            var oldName = gObject.name;

            // gObject.name = gObject.name.TrimEnd();
            gObject.name = gObject.name.Trim();
            if (gObject.name == GenericGameObjectName) gObject.name = "";

            // Don't use separator if name is empty
            // var separator = string.IsNullOrWhiteSpace(gObject.name) ? "" : $" {_Separator} ";
            // gObject.name += separator + suffix;
            
            suffix = $" [{suffix}]";
            gObject.name += suffix;
            Debug.Log($"Renamed '{oldName}' to '{gObject.name}'");
        }

        #endregion
    }
}
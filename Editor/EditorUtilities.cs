#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GamePack.UnityUtilities.Base
{
    [CreateAssetMenu(fileName = "Base Editor Utilities", menuName = "GamePack/Editor Utilities", order = 0)]
    public class EditorUtilities : ScriptableObject
    {
        #region Menu Items

        [MenuItem("Utilities/Random Vertical Rotation")]
        private static void RandomRotation()
        {
            foreach (var o in Selection.gameObjects)
            {
                o.transform.Rotate( Vector3.up, Random.Range(0,360f),Space.World);
            }
        }
        
        [MenuItem("Utilities/Select Top Most Parent")]
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
        
        [MenuItem("Utilities/Select Parents")]
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
    
        [MenuItem("Utilities/Select Children")]
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
        
        [MenuItem("Utilities/Select Child TMP")]
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
                    if(tmp)
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

        [MenuItem("Utilities/Sort Children")]
        private static void SortChildren()
        {
            var parents = Selection.gameObjects;

            foreach (var parent in parents)
            {
                SortChildrenOf(parent);
                
                /*var children = parent.transform.Cast<Transform>().OrderBy(transform => transform.name);
                
                foreach (var child in children)
                {
                    child.SetAsLastSibling();
                }*/
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
        
        #endregion
    
        #region Rename

        private const string GenericGameObjectName = "GameObject";

        [SerializeField, TabGroup("Rename")] private string _Separator = "-";
        [SerializeField, TabGroup("Rename")] private bool _ForceRename;

        [Button, TabGroup("Rename")]
        private void RenameAll()
        {
            foreach (var gObject in FindObjectsOfType<GameObject>())
            {
                RenameGameObject(gObject);
            }
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
            Undo.RecordObjects(components, "BaseEditorUtilities/Rename ");

            // Return if no component exists
            if (!components.Any()) return;

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
            var separator = string.IsNullOrWhiteSpace(gObject.name) ? "" : $" {_Separator} ";
            gObject.name += separator + suffix;
            Debug.Log($"Renamed {oldName} to {gObject.name}!");
        }

        #endregion

        #region Replace
    
        [Button, TabGroup("Replace")]
        private void ReplaceTmpTextString(string searchFor, string replaceWith)
        {
            var textMeshProUguis = FindObjectsOfType<TMP_Text>();
            // Undo.RecordObjects(textMeshProUguis.Select(ugui => ugui.gameObject).ToArray(), "ReplaceTmpTextString");

            var changed = new List<TMP_Text>();
            foreach (var textMeshProUgui in textMeshProUguis.Where(ugui => ugui.text.Contains(searchFor)))
            {
                textMeshProUgui.text = textMeshProUgui.text.Replace(searchFor, replaceWith);
                changed.Add(textMeshProUgui);
                EditorUtility.SetDirty(textMeshProUgui);
            }
            Debug.Log($"Replaced {changed.Count} names!");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); 
            UnityEditor.Selection.objects = changed.Select(ugui => ugui.gameObject).ToArray();
        }
    
        [Button, TabGroup("Replace")]
        private void ReplaceTmpUguiTextString(string searchFor, string replaceWith)
        {
            // var textMeshProUguis = FindObjectsOfType<TextMeshProUGUI>();
            var textMeshProUguis = FindAllObjects.InScene<TextMeshProUGUI>();
            Undo.RecordObjects(textMeshProUguis.Select(ugui => ugui.gameObject).ToArray(), "ReplaceTmpTextString");

            var changed = new List<TextMeshProUGUI>();
            foreach (var textMeshProUgui in textMeshProUguis.Where(ugui => ugui.text.Contains(searchFor)))
            {
                textMeshProUgui.text = textMeshProUgui.text.Replace(searchFor, replaceWith);
                changed.Add(textMeshProUgui);
            }
            Debug.Log($"Replaced {changed.Count} names!");
            UnityEditor.Selection.objects = changed.Select(ugui => ugui.gameObject).ToArray();
        }
    
        [Button, TabGroup("Replace")]
        private void ReplaceNameString(string searchFor, string replaceWith)
        {
            var gameObjects = FindObjectsOfType<GameObject>();
            Undo.RecordObjects(gameObjects, "ReplaceNameString");

            var changed = new List<GameObject>();
            foreach (var image in gameObjects.Where(go => go.name.Contains(searchFor)))
            {
                image.name = image.name.Replace(searchFor, replaceWith);
                changed.Add(image);
            }
            Debug.Log($"Replaced {changed.Count} names!");
            UnityEditor.Selection.objects = changed.ToArray();
        }

        [Button, TabGroup("Replace")]
        private void ReplaceImageSprite(Sprite searchFor, Sprite replaceWith)
        {
            var images = FindAllObjects.InScene<Image>();
            Undo.RecordObjects(images.ToArray(), "Replace Sprites");

            var changedImages = new List<Image>();
            foreach (var image in images.Where(spriteRenderer => spriteRenderer.sprite == searchFor))
            {
                image.sprite = replaceWith;
                Debug.Log($"Replacing {image.name} sprite");
                changedImages.Add(image);
            }
            Debug.Log($"Replaced {changedImages.Count} sprites!");
            UnityEditor.Selection.objects = changedImages.ToArray();
        }
    
        [Button, TabGroup("Replace")]
        private void ReplaceImageColors(Color searchFor, Color replaceWith)
        {
            var images = FindAllObjects.InScene<Image>();
            Undo.RecordObjects(images.ToArray(), "Replace Colors");
        
            var changed = new List<Image>();
            foreach (var image in images.Where(image => image.color == searchFor))
            {
                image.color = replaceWith;
                Debug.Log($"Replacing {image.name} sprite");
                changed.Add(image);

            }
            Debug.Log($"Replaced {changed.Count} sprites!");
        }
    
        [Button, TabGroup("Replace")]
        private void ReplaceTmpTextColors(Color searchFor, Color replaceWith)
        {
            var texts = FindAllObjects.InScene<TextMeshProUGUI>();
            Undo.RecordObjects(texts.ToArray(), "Replace TMP Text Colors");
        
            var changed = new List<TextMeshProUGUI>();
            foreach (var text in texts.Where(text => text.color == searchFor))
            {
                text.color = replaceWith;
                Debug.Log($"Replacing {text.name} text");
                changed.Add(text);
            }
            Debug.Log($"Replaced {changed.Count} texts!");
        }
    
        [Button, TabGroup("Replace")]
        private void ReplaceUnityTextColors(Color searchFor, Color replaceWith)
        {
            var texts = FindAllObjects.InScene<Text>();
            Undo.RecordObjects(texts.ToArray(), "Replace Colors");
        
            var changed = new List<Text>();
            foreach (var text in texts.Where(text => text.color == searchFor))
            {
                text.color = replaceWith;
                Debug.Log($"Replacing {text.name} color");
                changed.Add(text);
            }
            Debug.Log($"Replaced {changed.Count} texts!");
        }

        #endregion

        [Button]
        [MenuItem("Utilities/QuickScript")]
        private static void QuickScript()
        {
            #region Prompt

            if (!EditorUtility.DisplayDialog($"QuickScript",
                $"Run QuickScript? (Defined in EditorUtilities) ", "OK", "Cancel"))
                return;

            #endregion
            
            // Select 3DElements with no scenery
            // var elements = FindObjectsOfType<ThreeDElement>();

            /*
            // Custom rename
            foreach (var o in Selection.gameObjects)
            {
                var n = o.name;
                var split = o.name.Split(new []{"_00", "_0", "_"}, StringSplitOptions.None);
                o.name = split[0] + split[split.Length - 1];
            }*/
        }
    }
}
#endif
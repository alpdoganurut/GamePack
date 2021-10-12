using System.Collections.Generic;
using System.Linq;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GamePack.Editor.Utilities
{
    public partial class EditorUtilities
    {
        [MenuItem("Utilities/Window")]
        public static void ShowWindow()
        {
            GetWindow<EditorUtilities>();
        }

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

            // Return if no component exists
            if (!components.Any()) return;

            Undo.RecordObjects(components, "BaseEditorUtilities/Rename ");

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
            Selection.objects = changed.Select(ugui => ugui.gameObject).ToArray();
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

        #region Design

        [Button, TabGroup("Design")]
        private static void CenterMeshInParent()
        {
            var sel = Selection.gameObjects;
            if (sel.Length < 1)
            {
                Debug.LogError("Select one or more object to center.");
                return;
            }

            foreach (var obj in sel)
            {
                if (!obj.transform.parent)
                {
                    Debug.LogError($"{obj} has no parent.");
                    continue;
                }

                var bounds = obj.GetComponent<MeshRenderer>().bounds;
                obj.transform.localPosition = -bounds.center;
            }
        }

        [Button, TabGroup("Design")]
        private static void RandomRotation()
        {
            foreach (var o in Selection.gameObjects)
            {
                o.transform.Rotate(Vector3.up, Random.Range(0, 360f), Space.World);
            }
        }
        
        [Button, TabGroup("Design")]
        private static void CreateWrapper()
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;
            var firstOrDefault = sel.FirstOrDefault();
            if (!firstOrDefault) return;

            var firstParent = firstOrDefault ? firstOrDefault.transform.parent : null;
            var center = Vector3.zero;
            sel.ForEach(o => { center += o.transform.position / sel.Length; });

            var wrapperGo = new GameObject("Wrapper");
            wrapperGo.transform.position = center;
            wrapperGo.transform.SetParent(firstParent);

            sel.ForEach(o => { o.transform.SetParent(wrapperGo.transform); });
        }

        [Button, TabGroup("Design")]
        private static void PlaceItems(float verticalOffset = 100)
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;

            Undo.RecordObjects(sel.ToArray<Object>(), "Place Items");

            // ReSharper disable once PossibleNullReferenceException

            sel.ForEach(o =>
            {
                var origin = o.transform.position + (Vector3.up * 10);
                Debug.DrawRay(origin, Vector3.down * 100, Color.red, 5);
                if (!Physics.Raycast(origin, Vector3.down, out var hit)) return;

                Debug.DrawLine(origin, hit.point, Color.green, 5);
                o.transform.position = hit.point + new Vector3(0, verticalOffset, 0);
            });
        }
        
        [Button, TabGroup("Design")]
        private static void RandomizeScale(float min = .5f, float max = 1)
        {
            var sel = Selection.gameObjects;
            Undo.RecordObjects(sel.ToArray<Object>(), "RandomizeScale");

            foreach (var gameObject in sel)
            {
                gameObject.transform.localScale *= Random.Range(min, max);
            }
        }

        #endregion
        
    }
}
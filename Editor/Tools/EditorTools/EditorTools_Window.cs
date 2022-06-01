using System.Collections.Generic;
using System.Linq;
using GamePack.Tools.Helper;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GamePack.Editor.Tools
{
    public partial class EditorTools
    {
        [MenuItem("Utilities/Show Editor Tools Window", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<EditorTools>();
        }
        
        #region Replace

        [Button, TabGroup("Replace")]
        private void ReplaceTmpTextString(string searchFor, string replaceWith)
        {
            var textMeshProUguis = FindAllObjects.InScene<TMP_Text>();
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
            // var textMeshProUguis = FindAllObjects.InScene<TextMeshProUGUI>();
            var textMeshProUguis = FindAllObjects.InScene<TextMeshProUGUI>();
            Undo.RecordObjects(textMeshProUguis.Select(ugui => ugui.gameObject).ToArray(), "ReplaceTmpTextString");

            var changed = new List<TextMeshProUGUI>();
            foreach (var textMeshProUgui in textMeshProUguis.Where(ugui => ugui.text.Contains(searchFor)))
            {
                textMeshProUgui.text = textMeshProUgui.text.Replace(searchFor, replaceWith);
                changed.Add(textMeshProUgui);
            }

            Debug.Log($"Replaced {changed.Count} names!");
            Selection.objects = changed.Select(ugui => ugui.gameObject).ToArray();
        }

        [Button, TabGroup("Replace")]
        private void ReplaceNameString(string searchFor, string replaceWith)
        {
            var gameObjects = FindAllObjects.InScene<GameObject>();
            Undo.RecordObjects(gameObjects, "ReplaceNameString");

            var changed = new List<GameObject>();
            foreach (var image in gameObjects.Where(go => go.name.Contains(searchFor)))
            {
                image.name = image.name.Replace(searchFor, replaceWith);
                changed.Add(image);
            }

            Debug.Log($"Replaced {changed.Count} names!");
            Selection.objects = changed.ToArray();
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
            Selection.objects = changedImages.ToArray();
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

        #region Replace GameObject

        [SerializeField, Required, FoldoutGroup("Replace Selected GameObjects")] private bool _DeleteOld = true;
        [SerializeField, Required, AssetsOnly, FoldoutGroup("Replace Selected GameObjects")] private GameObject _Target;
        
        [Button, FoldoutGroup("Replace Selected GameObjects")]
        private void ReplaceSelectionWithTarget()
        {
            var selection = Selection.gameObjects;
            foreach (var gameObject in selection)
            {
                var newPrefab = (GameObject) PrefabUtility.InstantiatePrefab(_Target);
                newPrefab.transform.SetParent(gameObject.transform.parent);
                
                var info = new TransformInfo(gameObject.transform);
                info.ApplyLocal(newPrefab.transform);
                
                if(_DeleteOld) DestroyImmediate(gameObject);
            }
        }

        #endregion
        
        #region Tools

        [Button, TabGroup("Tools")]
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

        [Button, TabGroup("Tools")]
        private static void RandomYRotation()
        {
            foreach (var o in Selection.gameObjects)
            {
                o.transform.Rotate(Vector3.up, Random.Range(0, 360f), Space.World);
            }
        }
        
        [Button, TabGroup("Tools")]
        private static void CreateWrapper()
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;
            var firstOrDefault = sel.FirstOrDefault();
            if (!firstOrDefault) return;

            var firstParent = firstOrDefault ? firstOrDefault.transform.parent : null;
            var center = Vector3.zero;
            LinqExtensions.ForEach(sel, o => { center += o.transform.position / sel.Length; });

            var wrapperGo = new GameObject("Wrapper");
            wrapperGo.transform.position = center;
            wrapperGo.transform.SetParent(firstParent);

            LinqExtensions.ForEach(sel, o => { o.transform.SetParent(wrapperGo.transform); });
        }
        
        [Button, TabGroup("Tools"), Tooltip("Create one parent gameobject for each selected gameobject.Selected gameobject will be centered by position or by its bounds if it has a MeshRenderer component")]
        private static void CreateIndividualWrappers()
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;

            foreach (var o in sel)
            {
                var parent = o.transform.parent;
                var meshRenderer = o.GetComponent<MeshRenderer>();
                var wrapperPos = meshRenderer ? meshRenderer.bounds.center : o.transform.position;

                var wrapperGo = new GameObject($"{o.name} Wrapper");
                wrapperGo.transform.position = wrapperPos;
                
                if(parent)
                    wrapperGo.transform.SetParent(parent);
                
                o.transform.SetParent(wrapperGo.transform, true);
            }
        }

        [Button, TabGroup("Tools")]
        private static void PlaceItems(float placementYOffset = 0, float projectionOffset = 1)
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;

            Undo.RecordObjects(sel.ToArray<Object>(), "Place Items");

            // ReSharper disable once PossibleNullReferenceException
            
            LinqExtensions.ForEach(sel, o =>
            {
                o.SetActive(false);
                
                var pos = o.transform.position;
                var origin = pos + (Vector3.up * projectionOffset);
                Debug.DrawRay(origin, Vector3.down * 100, Color.red, 5);
                if (!Physics.Raycast(origin, Vector3.down, out var hit)) return;
                // if(hit.collider.gameObject == o) return;

                Debug.DrawLine(pos, hit.point, Color.green, 5);
                o.transform.position = hit.point + new Vector3(0, placementYOffset, 0);
                
                o.SetActive(true);
            });
        }
        
        [Button, TabGroup("Tools")]
        private static void RandomizeScale(float min = .5f, float max = 1)
        {
            var sel = Selection.gameObjects;
            Undo.RecordObjects(sel.ToArray<Object>(), "RandomizeScale");

            foreach (var gameObject in sel)
            {
                gameObject.transform.localScale *= Random.Range(min, max);
            }
        }

        
        [Button, TabGroup("Tools")]
        private static void ArrangeItemsWithSpacing(float spacing = 1)
        {
            var sel = Selection.gameObjects;


            for (var index = 0; index < sel.Length; index++)
            {
                var obj = sel[index];
                obj.transform.position = new Vector3(index * spacing, 0, 0);
            }
        }
        
        #endregion
     
        #region Add Component Name

        [Button, FoldoutGroup("Component Name"), PropertyOrder(-1)]
        private void AddComponentNamesInScene()
        {
            foreach (var gObject in FindAllObjects.InScene<GameObject>()) 
                RenameGameObject(gObject);
        }
        
        [Button, FoldoutGroup("Component Name"), PropertyOrder(-1)]
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

        #endregion
    }
}
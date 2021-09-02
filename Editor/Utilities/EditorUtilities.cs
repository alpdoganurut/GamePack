#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Utilities;
using GamePack.UnityUtilities.Vendor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GamePack.UnityUtilities
{
    [CreateAssetMenu(fileName = "Base Editor Utilities", menuName = "GamePack/Editor Utilities", order = 0)]
    public class EditorUtilities : OdinEditorWindow
    {
        private static EditorUtilitiesHelper _helper;
        
        static EditorUtilities()
        {
            Debug.Log($"Initializing static {nameof(EditorUtilities)} context.");

            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerOnSceneLoaded;
            SceneManager.sceneLoaded += (arg0, mode) => EnsureHelper();
        }

        private static void EditorSceneManagerOnSceneLoaded(Scene arg0, Scene arg1)
        {
            Debug.Log("Scene changed!");
            EnsureHelper();
        }

        private static void EnsureHelper()
        {
            if (!_helper)
            {
                _helper = FindObjectOfType<EditorUtilitiesHelper>();
                if(_helper)
                    _helper.OnUpdate += HelperOnOnUpdate;
            }
            
            if(!_helper)
            {
                _helper = new GameObject(nameof(EditorUtilitiesHelper)).AddComponent<EditorUtilitiesHelper>();
                _helper.gameObject.tag = "EditorOnly";
                _helper.OnUpdate += HelperOnOnUpdate;
            }
            
            _helper.gameObject.hideFlags = HideFlags.HideInHierarchy;
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.DirtyHierarchyWindowSorting();
        }
        
        private static void HelperOnOnUpdate(bool isPlaying)
        {
            if(!Application.isPlaying) return;
            
            if (
                (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) 
                && Mathf.Abs(Input.mouseScrollDelta.sqrMagnitude) > 0.01f
                )
            {
                if (Input.mouseScrollDelta.y > 0)
                {
                    Time.timeScale *= 2;
                }
                else
                {
                    Time.timeScale /= 2;
                }

                Debug.Log($"TimeScale = {Time.timeScale}");
            }

            if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) && Input.GetMouseButtonDown(2))
            {
                Time.timeScale = 1;
            }
        }

        [MenuItem("Utilities/Window")]
        public static void ShowWindow()
        {
            GetWindow<EditorUtilities>();
        }

        
        /*[InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            // Log($"OnEnterPlaymodeInEditor: {options}");

            
            if (!_instance || !options.HasFlag(EnterPlayModeOptions.DisableDomainReload)) return;
            
            _instance.StopListeningSceneChange();
        }*/

        
        private void Update()
        {
        }

        #region Menu Items

        // This totally blows, skinned mesh renderer to mesh renderer conversion is not that straightforward.
        /*[MenuItem("Utilities/Convert SkinnedMeshRenderers To")]
        public static void ConvertSkinnedMeshRenderersTo()
        {
            foreach (var sel in Selection.gameObjects)
            {
                // var sel = Selection.activeGameObject;
                var smr = sel.GetComponent<SkinnedMeshRenderer>();
                var bone = smr.bones[0];
                
                var scale = bone.lossyScale;
                var pos = bone.transform.position;
                sel.transform.position = pos;
                sel.transform.SetGlobalScale(scale);
                sel.transform.rotation = bone.transform.rotation;
                
                var newMr = sel.AddComponent<MeshRenderer>();
                newMr.sharedMaterials = smr.sharedMaterials.ToArray();
                
                var newMf = sel.AddComponent<MeshFilter>();
                newMf.sharedMesh = smr.sharedMesh;
                
                DestroyImmediate(smr);   
                
            }
        }*/
        
        
        [MenuItem("Utilities/Remove Colliders")]
        private static void RemoveColliders()
        {
            var sel = Selection.gameObjects;
            /*if (sel.Length < 1)
            {
                Debug.LogError("Select one or more object to center.");
                return;
            }*/

            var destroyList = new List<Collider>();
            
            foreach (var obj in sel)
            {
                var colliders = obj.GetComponents<Collider>();
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
        
        [MenuItem("Utilities/Center Mesh In Parent")]
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
        
        [MenuItem("Utilities/Slow Time Down %&T")]
        private static void SlowTimeDown()
        {
            Time.timeScale /= 2;
        }
        
        [MenuItem("Utilities/Reset Slow Time Down %&R")]
        private static void ResetSlowTimeDown()
        {
            Time.timeScale = 1;
        }

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

        /*
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
            }#1#
        }
        */
        
        
    }
}
#endif
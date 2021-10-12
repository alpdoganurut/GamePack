#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Editor.Utilities;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Utilities
{
    [CreateAssetMenu(fileName = "Base Editor Utilities", menuName = "GamePack/Editor Utilities", order = 0)]
    public partial class EditorUtilities : OdinEditorWindow
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
                if (_helper)
                    _helper.OnUpdate += HelperOnOnUpdate;
            }

            if (!_helper)
            {
                _helper = new GameObject(nameof(EditorUtilitiesHelper)).AddComponent<EditorUtilitiesHelper>();
                _helper.gameObject.tag = "EditorOnly";
                _helper.OnUpdate += HelperOnOnUpdate;
            }

            // _helper.gameObject.hideFlags = HideFlags.HideInHierarchy;
            // EditorApplication.RepaintHierarchyWindow();
            // EditorApplication.DirtyHierarchyWindowSorting();
        }

        private static void HelperOnOnUpdate(bool isPlaying)
        {
            if (!Application.isPlaying) return;

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
        
        [MenuItem("Utilities/Remove Colliders of Selection")]
        private static void RemoveColliders()
        {
            var sel = Selection.gameObjects;

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
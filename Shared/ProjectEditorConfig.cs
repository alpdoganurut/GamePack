using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GamePack.Editor.Boilerplate
{
    [CreateAssetMenu(fileName = "Project Config", menuName = "GamePack/Project Editor Config", order = 0)]
    public class ProjectEditorConfig : ScriptableObject
    {
        #region Initilization

        private static ProjectEditorConfig _instance;

        public static ProjectEditorConfig Instance
        {
            get
            {
                if (!_instance) FindInProject();
                return _instance;
            }
            private set => _instance = value;
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod() => FindInProject();

        private static void FindInProject()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(ProjectEditorConfig)}");
            if (guids.Length == 0)
            {
                Debug.LogError($"Project contains no {nameof(ProjectEditorConfig)}");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            if (guids.Length > 1)
                Debug.LogError(
                    $"Shouldn't have more than one {nameof(ProjectEditorConfig)} in project. Choosing first one ({path}) as instance.");

            Instance = AssetDatabase.LoadAssetAtPath<ProjectEditorConfig>(path);
        }

        #endregion
        
        [field: SerializeField ] public bool AutoTestLevelScenes { get; set; }
        [field: SerializeField ] public bool ShowScreenButtons { get; set; }

        [field: SerializeField, FoldoutGroup("Cursor")] public bool ShowCursor { get; set; }
        [field: SerializeField, FoldoutGroup("Cursor")] public Image CursorPrefab { get; set; }
    }
}
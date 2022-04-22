using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Boilerplate
{
    [CreateAssetMenu(fileName = "Project Config", menuName = "GamePack/Project Config", order = 0)]
    public class ProjectConfig : ScriptableObject
    {
        private static ProjectConfig _instance;

        public static ProjectConfig Instance
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
            var guids = AssetDatabase.FindAssets($"t:{nameof(ProjectConfig)}");
            if (guids.Length == 0)
            {
                Debug.LogError($"Project contains no {nameof(ProjectConfig)}");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            if (guids.Length > 1)
                Debug.LogError(
                    $"Shouldn't have more than one {nameof(ProjectConfig)} in project. Choosing first one ({path}) as instance.");

            Instance = AssetDatabase.LoadAssetAtPath<ProjectConfig>(path);
        }
        
        [SerializeField] private bool _AutoEnterMainScene = true;
        public bool AutoEnterMainScene
        {
            get => _AutoEnterMainScene;
            set => _AutoEnterMainScene = value;
        }
    }
}
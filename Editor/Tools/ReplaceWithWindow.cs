#if UNITY_EDITOR
using GamePack.Tools.Helper;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Tools
{
    public class ReplaceWithWindow : OdinEditorWindow
    {
        [MenuItem("Utilities/Show Replace Tools Window", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<ReplaceWithWindow>();
        }

        [SerializeField, Required] private bool _DeleteOld = true;
        [SerializeField, Required, AssetsOnly] private GameObject _Target;
        
        [Button]
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
    }
}
#endif
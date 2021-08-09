#if UNITY_EDITOR
using BabyCatcher.GamePack;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GamePack.UnityUtilities
{
    public class ReplaceWith : OdinEditorWindow
    {
        [MenuItem("Utilities/ReplaceWithTarget")]
        public static void ShowWindow()
        {
            GetWindow<ReplaceWith>();
        }

        [SerializeField, Required] private bool _DeleteOld = true;
        [SerializeField, Required, AssetsOnly] private GameObject _Target;
        
        [Button]
        private void ReplaceWithTarget()
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
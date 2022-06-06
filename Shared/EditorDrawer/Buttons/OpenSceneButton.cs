using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared.EditorDrawer.Buttons
{
    [Serializable]
    public class OpenSceneButton: ScreenButtonBase
    {
        [SerializeField, Required, OnValueChanged("UpdateSize")] 
        private SceneAsset _SceneAsset;

        public override string Label => $"Scene: {(_SceneAsset != null ? _SceneAsset.name : "")}";

        // Required for adding in editor
        public OpenSceneButton()
        {
        }

        public OpenSceneButton(SceneAsset sceneAsset)
        {
            _SceneAsset = sceneAsset;
        }

        public override void Action()
        {
            var sceneCt = SceneManager.sceneCount;
            for (var i = 0; i < sceneCt; i++)
            {
                if (!SceneManager.GetSceneAt(i).isDirty) continue;
                if (EditorUtility.DisplayDialog("Scene Not Saved",
                        $"Scene is not saved, open new scene and lose changes?", "Lose Changes",
                        "Cancel")) continue;
                Debug.Log("Cancelled open scene.");
                return;
            }
            
            var path = AssetDatabase.GetAssetPath(_SceneAsset);
            EditorSceneManager.OpenScene(path);
        }
    }
}
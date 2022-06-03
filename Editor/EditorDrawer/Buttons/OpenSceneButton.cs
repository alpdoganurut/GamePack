using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor.EditorDrawer.Buttons
{
    [Serializable]
    public class OpenSceneButton: ScreenButtonBase
    {
        [SerializeField, Required, OnValueChanged("UpdateSize")] 
        private SceneAsset _SceneAsset;

        public override string Label => $"Scene: {(_SceneAsset != null ? _SceneAsset.name : "")}";

        public OpenSceneButton()
        {
        }

        public OpenSceneButton(SceneAsset sceneAsset)
        {
            _SceneAsset = sceneAsset;
        }

        public override void Action()
        {
            var path = AssetDatabase.GetAssetPath(_SceneAsset);
            EditorSceneManager.OpenScene(path);
        }
    }
}
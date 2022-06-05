using System;
using System.Collections.Generic;
using Editor.EditorDrawer.Buttons;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorDrawer
{
    [CreateAssetMenu(fileName = "ScreenButtonsConfig", menuName = "GamePack/ScreenButtonsConfig", order = 0)]
    public class ScreenButtonsConfig : ScriptableObject
    {
        private static List<ScreenButtonBase> DefaultButtons => new()
        {
            new OpenSceneButton(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/01_Scenes/main.unity")),
            new OpenSceneButton(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/01_Scenes/Art/art.unity")),
            new ToggleUIButton()
        };

        [field: SerializeReference] public List<ScreenButtonBase> Buttons { get; private set; }

        private void Reset()
        {
            Buttons = DefaultButtons;
        }

        private void Awake()
        {
            EditorDrawerSystem.RegisterScreenButtonsConfig(this);
        }
    }
}
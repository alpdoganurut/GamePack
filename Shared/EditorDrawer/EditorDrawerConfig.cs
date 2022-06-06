using System.Collections.Generic;
using Shared.EditorDrawer.Buttons;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Shared.EditorDrawer
{
    [CreateAssetMenu(fileName = "ScreenButtonsConfig", menuName = "GamePack/ScreenButtonsConfig", order = 0)]
    public class EditorDrawerConfig : ScriptableObject
    {
        private static List<ScreenButtonBase> DefaultButtons => new()
        {
            new OpenSceneButton(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/01_Scenes/main.unity")),
            new OpenSceneButton(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/01_Scenes/Art/art.unity")),
            new ToggleUIButton()
        };

        [field: SerializeField] public float ButtonGroupWidth { get; private set; } = 100;

        [field: SerializeReference] public List<ScreenButtonBase> Buttons { get; private set; }

        [field: SerializeField, Required] public CustomGUIStyle ButtonStyle { get; private set; }
        
        [field: SerializeField, Required] public CustomGUIStyle ScreenInfoStyle { get; private set; }

        private void Awake()
        {
            EditorDrawerSystem.RegisterScreenButtonsConfig(this);
            ScreenInfoStyle.RefreshStyle();
            ButtonStyle.RefreshStyle();
        }

        private void Reset()
        {
            Buttons = DefaultButtons;
            // ButtonGUIStyle = EditorStyles.miniButton;
            ButtonStyle = new CustomGUIStyle("miniButton", Color.white, Color.black, false);;
            // ScreenInfoGUIStyle = EditorStyles.helpBox;
            ScreenInfoStyle = new CustomGUIStyle("helpBox", Color.white, Color.black, false);
        }
    }
}
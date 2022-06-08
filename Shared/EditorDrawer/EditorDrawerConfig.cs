using System.Collections.Generic;
using Shared.EditorDrawer.Buttons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shared.EditorDrawer
{
    [CreateAssetMenu(fileName = "ScreenButtonsConfig", menuName = "GamePack/ScreenButtonsConfig", order = 0)]
    public class EditorDrawerConfig : ScriptableObject
    {
        [field: SerializeField] public float IsWindowVisible { get; private set; } = 500;

        [field: SerializeField] public List<ScriptableSceneButtonBase> Buttons { get; private set; }

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
            ButtonStyle = new CustomGUIStyle("miniButton", Color.white, Color.black, false);;
            ScreenInfoStyle = new CustomGUIStyle("helpBox", Color.white, Color.black, false);
        }
    }
}
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

// ReSharper disable NotAccessedField.Local

namespace GamePack.Editor.Tools
{
    public partial class EditorTools
    {
        [SerializeField, Required] private Helper _Helper;
        [SerializeField, Required] private SelectionHelper _Selection;
        [SerializeField, Required] private Replace _Replace;
        [SerializeField, Required] private Rename _Rename;
        
        [MenuItem("Utilities/Show Editor Tools Window", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<EditorTools>();
        }
    }
}
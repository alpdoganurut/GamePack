using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Editor.EditorDrawer.Buttons
{
    [Serializable]
    public abstract class ScreenButtonBase
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public abstract string Label { get; }
        
        private Vector2 _calculatedSize = Vector2.zero;
        
        public Vector2 Size
        {
            get
            {
                if (_calculatedSize == Vector2.zero) 
                    _calculatedSize = GUI.skin.button.CalcSize(new GUIContent(Label));

                return _calculatedSize;
            }
        }

        public abstract void Action();
        
        [Button]
        private void UpdateSize() => _calculatedSize = Vector2.zero;
    }
}
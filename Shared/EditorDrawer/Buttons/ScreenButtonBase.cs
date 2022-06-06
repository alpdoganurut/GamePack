using System;
using UnityEngine;

namespace Shared.EditorDrawer.Buttons
{
    [Serializable]
    public class ScreenButtonBase
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public virtual string Label { get; }
        
        // private Vector2 _calculatedSize = Vector2.zero;
        
        public Vector2 Size => GUI.skin.button.CalcSize(new GUIContent(Label));

        public virtual void Action(){}
        
        private void UpdateSize()
        {
            /*_calculatedSize = Vector2.zero;*/
        }
    }
}
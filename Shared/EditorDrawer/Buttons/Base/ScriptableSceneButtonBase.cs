using System;
using UnityEngine;

namespace Shared.EditorDrawer.Buttons
{
    [Serializable]
    public abstract class ScriptableSceneButtonBase: ScriptableObject, IScreenButton
    {
        public abstract string Label { get; }
        public abstract void Action();

        // public Vector2 Size => ((IScreenButton) this).Size;
    }
}
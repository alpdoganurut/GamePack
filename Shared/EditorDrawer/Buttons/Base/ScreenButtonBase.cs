using System;
using UnityEngine;

namespace Shared.EditorDrawer.Buttons
{
    public interface IScreenButton
    {
        public string Label { get; }

        // public Vector2 Size { get; }

        public void Action();

    }
}
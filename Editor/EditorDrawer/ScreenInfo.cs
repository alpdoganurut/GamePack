using UnityEngine;

namespace Editor.EditorDrawer
{
    public class ScreenInfo
    {

        public delegate object MessageDelegate();

        private readonly MessageDelegate _messageEvent;

        public Vector2 Size => GUI.skin.box.CalcSize(new GUIContent(Message));

        public string Message => _messageEvent?.Invoke().ToString();

        public ScreenInfo(MessageDelegate messageEvent)
        {
            _messageEvent = messageEvent;
            
            // size ??= DefaultSize;
            // Size = size.Value;
            
            SceneScreenButtonsDrawer.RegisterInfo(this);
        }

    }
}
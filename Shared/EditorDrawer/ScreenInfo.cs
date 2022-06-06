using UnityEngine;

namespace Shared.EditorDrawer
{
    public class ScreenInfo
    {
        public delegate object MessageDelegate();

        private readonly MessageDelegate _messageEvent;

        public string Message => _messageEvent?.Invoke().ToString();

        public GameObject BoundGameObject { get; }

        public ScreenInfo(MessageDelegate messageEvent, GameObject boundGameObject)
        {
            _messageEvent = messageEvent;
            BoundGameObject = boundGameObject;
            EditorDrawerSystem.RegisterInfo(this);
        }

        public void Delete() => EditorDrawerSystem.UnregisterInfo(this);
    }
}
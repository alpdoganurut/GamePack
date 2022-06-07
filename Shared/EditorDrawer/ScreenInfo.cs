using GamePack.Utilities;
using UnityEngine;

namespace Shared.EditorDrawer
{
    public class ScreenInfo
    {
        private readonly MessageDelegate _messageEvent;

        public string Message => _messageEvent?.Invoke().ToString();

        public GameObject BoundGameObject { get; }

        public ScreenInfo(MessageDelegate messageEvent, GameObject boundGameObject = null)
        {
            _messageEvent = messageEvent;
            BoundGameObject = boundGameObject;
            EditorDrawerSystem.RegisterInfo(this);
        }

        public void Delete() => EditorDrawerSystem.UnregisterInfo(this);
    }
}
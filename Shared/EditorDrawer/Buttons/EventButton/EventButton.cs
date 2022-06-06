using UnityEngine.Events;

namespace Shared.EditorDrawer.Buttons.EventButton
{
    public class EventButton: ScreenButtonBase
    {
        private readonly UnityEvent _event;

        public override string Label { get; }
        
        public ScreenButtonComponent Component { get; }

        public EventButton(ScreenButtonComponent component, string text)
        {
            Component = component;
            _event = component.Event;
            Label = text;
        }

        public override void Action() => _event.Invoke();
    }
}
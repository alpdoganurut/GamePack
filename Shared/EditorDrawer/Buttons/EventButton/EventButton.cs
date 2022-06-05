using UnityEngine.Events;

namespace Editor.EditorDrawer.Buttons.EventButton
{
    public class EventButton: ScreenButtonBase
    {
        private readonly UnityEvent _event;

        public override string Label { get; }
        
        public EventButtonComponent Component { get; }

        public EventButton(EventButtonComponent component, string text)
        {
            Component = component;
            _event = component.Event;
            Label = text;
        }

        public override void Action() => _event.Invoke();
    }
}
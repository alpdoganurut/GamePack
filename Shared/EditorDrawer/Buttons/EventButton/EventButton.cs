using UnityEngine.Events;

namespace Shared.EditorDrawer.Buttons.EventButton
{
    public class EventButton: IScreenButton
    {
        private readonly UnityEvent _event;

        public string Label { get; }
        
        public ScreenButtonComponent Component { get; }

        public EventButton(ScreenButtonComponent component, string text)
        {
            Component = component;
            _event = component.Event;
            Label = text;
        }

        public void Action() => _event.Invoke();
    }
}
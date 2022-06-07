using System;

namespace Shared.EditorDrawer.Buttons
{
    public class DynamicButton: IScreenButton
    {
        private readonly Action _action;

        public string Label { get; }

        public DynamicButton(string label, Action action)
        {
            Label = label;
            _action = action;
        }

        public void Action()
        {
            _action?.Invoke();
        }
    }
}
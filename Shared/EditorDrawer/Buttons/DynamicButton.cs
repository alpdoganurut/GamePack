using System;

namespace Shared.EditorDrawer.Buttons
{
    public class DynamicButton: ScreenButtonBase
    {
        private readonly Action _action;

        public override string Label { get; }

        public DynamicButton(string label, Action action)
        {
            Label = label;
            _action = action;
        }

        public override void Action()
        {
            _action?.Invoke();
        }
    }
}
// This script solely exist for the purpose of keeping ScreenInfo creation code present on build
#if !UNITY_EDITOR
namespace Editor.EditorDrawer
{
    public class ScreenInfo
    {
        public delegate object MessageDelegate();

        private readonly MessageDelegate _messageEvent;

        public ScreenInfo(MessageDelegate messageEvent)
        {
        }
    }
}
#endif
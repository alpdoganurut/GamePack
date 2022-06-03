using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Editor.EditorDrawer
{
    [ExecuteAlways]
    public class EventButtonComponent: MonoBehaviour
    {
        [SerializeField, Required] private string _Label;
        [SerializeField, Required] private UnityEvent _Event;

        public UnityEvent Event => _Event;

        public string Label => _Label;

        private void OnEnable() => SceneScreenButtonsDrawer.RegisterButtonComponent(this);

        private void OnDisable() => SceneScreenButtonsDrawer.UnRegisterButtonComponent(this);

        private void OnValidate() => name = $"Button: {_Label}";
    }
}
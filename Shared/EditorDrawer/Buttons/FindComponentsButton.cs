using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.EditorDrawer.Buttons
{
    [Serializable]
    public class FindComponentsButton: ScreenButtonBase
    {
        [SerializeReference, Required] private string _ComponentName;

        public override string Label => $"Select: {_ComponentName}";

        public override void Action()
        {
            var type = FindTypeByName(_ComponentName);
            
            if(type != null)
                UnityEditor.Selection.objects = Object.FindObjectsOfType(type).Select(o => (o as Component)?.gameObject).ToArray();
        }
        
        public static Type FindTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetTypes().FirstOrDefault(type => type.Name.Equals(name));
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }
    }
}
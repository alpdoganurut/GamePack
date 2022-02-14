using GamePack.CustomAttribute.Attributes;
using UnityEditor;
using UnityEngine;

namespace GamePack.CustomAttribute.Editor
{
    [CustomPropertyDrawer(typeof(RenameInHierarchyAttribute))]
    public class RenameInHierarchyAttributeDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = $"{property.displayName} (Renamed)";
            PropertyDrawerTools.DrawProperty(position, property, label);
            PropertyDrawerTools.RenamePropertyReference(property);
        }
    }
}
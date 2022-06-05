using GamePack.CustomAttribute.Attributes;
using UnityEditor;
using UnityEngine;

namespace GamePack.CustomAttribute.Editor
{
    [CustomPropertyDrawer(typeof(AutoFillChildrenAttribute))]
    public class AutoFillChildrenAttributeDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = $"{property.displayName} (AutoFillChildren)";
            PropertyDrawerTools.DrawProperty(position, property, label);
            // PropertyDrawerTools.RenamePropertyReference(property);

            /*if (property.objectReferenceValue) return;
            
            property.objectReferenceValue =
                PropertyDrawerTools.FindComponentOfSerializedField(property, PropertyDrawerTools.GetComponentPlace.Children);*/
            
        }
    }
}
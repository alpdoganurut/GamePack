using GamePack.CustomAttribute.Attributes;
using UnityEditor;
using UnityEngine;

namespace GamePack.CustomAttribute.Editor
{
    [CustomPropertyDrawer(typeof(AutoFillSceneAttribute))]
    public class AutoFillSceneAttributeDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = $"{property.displayName} (AutoFillScene)";
            PropertyDrawerTools.DrawProperty(position, property, label);
            PropertyDrawerTools.RenamePropertyReference(property);
            
            if (property.objectReferenceValue) return;
            
            property.objectReferenceValue =
                PropertyDrawerTools.FindComponentOfSerializedField(property, PropertyDrawerTools.GetComponentPlace.Scene);
        }
    }
}
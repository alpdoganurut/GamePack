using System.Collections.Generic;
using System.Linq;
using GamePack.CustomAttribute.Attributes;
using GamePack.Logging;
using UnityEditor;
using UnityEngine;

namespace GamePack.CustomAttribute.Editor
{
    [CustomPropertyDrawer(typeof(AutoFillSelfAttribute))]
    public class AutoFillSelfAttributeDrawer: PropertyDrawer
    {
        private static readonly List<string> SentErrorPropertyPathList = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = $"{property.displayName} (AutoFillSelf)";
            PropertyDrawerTools.DrawProperty(position, property, label);
            
            // var handledArrayProperties = new List<SerializedProperty>();
            if (property.name == "data")
            {
                /*
                 // Can't access or set elements of array (vector) type SerializedProperty on with PropertyDrawer
                 
                 var arrayProp = property.serializedObject.FindProperty(property.propertyPath.Split('.').FirstOrDefault());
                if(arrayProp == null) ManagedLog.Log("Can't find array property");

                if (handledArrayProperties.Contains(arrayProp))
                {
                    ManagedLog.Log("Skipping property because it's handled.");
                    return;
                }
                else
                {
                    ManagedLog.Log("Handling array property");
                }
                
                handledArrayProperties.Add(arrayProp);

                // var arraySize = arrayProp.arraySize;
                arrayProp.ClearArray();
                var components =
                    PropertyDrawerTools.FindMultiComponentOfType(property,
                        PropertyDrawerTools.GetComponentPlace.Children);

                for (var index = 0; index < components.Length; index++)
                {
                    var component = components[index];
                    arrayProp.InsertArrayElementAtIndex(index);
                    arrayProp.GetArrayElementAtIndex(index).objectReferenceValue = component;
                }*/
                
                if(!SentErrorPropertyPathList.Contains(property.propertyPath))
                {
                    SentErrorPropertyPathList.Add(property.propertyPath);
                    ManagedLog.LogError($"Can't use AutoFill with array or lists! Type: {property.serializedObject.targetObject.GetType().Name} property: {property.propertyPath.Split('.').FirstOrDefault()}");
                }
                
                return;
            }
            
            if (property.objectReferenceValue) return;

            property.objectReferenceValue =
                PropertyDrawerTools.FindComponentOfSerializedField(property, PropertyDrawerTools.GetComponentPlace.Self);
        }
    }
}
using System;
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
            
            /*
            if (property.objectReferenceValue) return;

            property.objectReferenceValue =
                PropertyDrawerTools.FindComponentOfSerializedField(property, PropertyDrawerTools.GetComponentPlace.Self);*/
        }
    }
}
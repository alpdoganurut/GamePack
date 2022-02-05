using System;
using System.Linq;
using System.Reflection;
using GamePack.Logging;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace GamePack.CustomAttribute.Editor
{
    public static class PropertyDrawerTools
    {
        private static readonly BindingFlags BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        public static void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
        }

        /// <summary>
        /// Uses reflection to get the type from a serialized property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static System.Type GetTypeFromProperty(SerializedProperty property)
        {
            // first, lets get the Type of component this serialized property is part of...
            var parentComponentType = property.serializedObject.targetObject.GetType();
            // ... then, using reflection well get the raw field info of the property this
            // SerializedProperty represents...
            var fieldInfo = parentComponentType.GetField(property.propertyPath, BindingFlags);
            // ... using that we can return the raw .net type!
            return fieldInfo.FieldType;
        }
        
        internal enum GetComponentPlace
        {
            Self, Children, Scene,
            // SceneMulti, ChildrenMulti
        }
        
        internal static Component FindComponentOfType(SerializedProperty property, GetComponentPlace place)
        {
            var serializedObjectTargetObject = property.serializedObject.targetObject;
            var rootTargetObject = serializedObjectTargetObject;

            if (!(rootTargetObject is Component rootComponent)) return null;

            var type = GetTypeFromProperty(property);
            var foundMultiErrorMessage = $"Found more than one {type.Name}!  field: {property.name}, scenePath: \"{serializedObjectTargetObject.GetScenePath()}\", Type: {serializedObjectTargetObject.GetType().Name}.";
            switch (place)
            {
                case GetComponentPlace.Self:
                    var foundSelf = rootComponent.GetComponents(type);
                    if (foundSelf.Length > 1)
                    {
                        ManagedLog.LogError(foundMultiErrorMessage);
                    }
                    return foundSelf.FirstOrDefault();
                case GetComponentPlace.Children:
                    var foundChildren = rootComponent.GetComponentsInChildren(type);
                    if (foundChildren.Length > 1)
                    {
                        ManagedLog.LogError(foundMultiErrorMessage);
                    }
                    return foundChildren.FirstOrDefault();
                case GetComponentPlace.Scene:
                    var foundInScene = Object.FindObjectsOfType(type) as Component[];
                    if (foundInScene?.Length > 1)
                    {
                        ManagedLog.LogError(foundMultiErrorMessage);
                    }
                    return foundInScene?.FirstOrDefault();
                /*case GetComponentPlace.SceneMulti:
                case GetComponentPlace.ChildrenMulti:
                    Assert.IsTrue(false, $"Can't use {place} with {nameof(FindComponentOfType)}");
                    return null;*/
                default:
                    throw new ArgumentOutOfRangeException(nameof(place), place, null);
            }
        }
        
        internal static Component[] FindMultiComponentOfType(SerializedProperty property, GetComponentPlace place)
        {
            var rootTargetObject = property.serializedObject.targetObject;
            if (!(rootTargetObject is Component rootComponent)) return null;

            var type = GetTypeFromProperty(property);
            switch (place)
            {
                case GetComponentPlace.Self:
                case GetComponentPlace.Children:
                case GetComponentPlace.Scene:
                    Assert.IsTrue(false, $"Can't use {place} with {nameof(FindMultiComponentOfType)}");
                    return null;
                /*case GetComponentPlace.SceneMulti:
                    return Object.FindObjectsOfType(type) as Component[];
                case GetComponentPlace.ChildrenMulti:
                    return rootComponent.GetComponentsInChildren(type);*/
                default:
                    throw new ArgumentOutOfRangeException(nameof(place), place, null);
            }
        }
    }
}
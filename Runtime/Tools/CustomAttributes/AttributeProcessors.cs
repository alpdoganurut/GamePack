using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Editor.EditorDrawer;
using UnityEngine;

namespace GamePack.CustomAttributes
{
    public static class AttributeProcessors
    {
        public static void ProcessAutoFillSelfAttribute(FieldInfo fieldInfo, Component ownerComponent)
        {
            var value = ownerComponent.GetComponent(fieldInfo.FieldType);
            fieldInfo.SetValue(ownerComponent, value);
        }
        
        public static void ProcessAutoFillChildrenAttribute(FieldInfo fieldInfo, Component ownerComponent)
        {
            var isEnumerable = IsTypeEnumerable(fieldInfo.FieldType);
            if (isEnumerable)
            {
                ProcessEnumerableFieldInfoForComponent(fieldInfo, ownerComponent,
                    type => ownerComponent.GetComponentsInChildren(type).Where(component => component.gameObject != ownerComponent.gameObject).ToArray());
                return;
            }

            var values = ownerComponent.GetComponentsInChildren(fieldInfo.FieldType);
            if (values.Length > 1)
                CustomAttributeSystem.LogValidationError($"Found more than one {fieldInfo.FieldType} at {ownerComponent}.");
            var value = values.FirstOrDefault(component => component.gameObject != ownerComponent.gameObject);

            fieldInfo.SetValue(ownerComponent, value);
        }
        
        public static void ProcessAutoFillSceneAttribute(FieldInfo fieldInfo, Component ownerComponent)
        {
            var isEnumerable = IsTypeEnumerable(fieldInfo.FieldType);
            if (isEnumerable)
            {
                ProcessEnumerableFieldInfoForComponent(
                    fieldInfo,
                    ownerComponent,
                    type => UnityEngine.Object.FindObjectsOfType(type, true) as Component[]);
                return;
            }

            var values = UnityEngine.Object.FindObjectsOfType(fieldInfo.FieldType, true);
            if (values.Length > 1)
                CustomAttributeSystem.LogValidationError($"Found more than one {fieldInfo.FieldType} at {ownerComponent}.");
            var value = values.FirstOrDefault();
            fieldInfo.SetValue(ownerComponent, value);
        }

        public static ScreenInfo ProcessMonitorAttribute(FieldInfo fieldInfo, Component ownerComponent, string fieldInfoName)
        {
            if (!Application.isPlaying) return null;

            var isList = fieldInfo.GetValue(ownerComponent) is IList;
            ScreenInfo screenInfo;
            // Display all
            if (isList)
            {
                screenInfo = new ScreenInfo(() =>
                {
                    var listValue = fieldInfo.GetValue(ownerComponent) as IList;

                    var builder = new StringBuilder();
                    if (listValue == null) return builder.ToString();

                    builder.Append($"({listValue.Count}) {ownerComponent.name}.{fieldInfoName}:");
                    for (var index = 0; index < listValue.Count; index++)
                    {
                        builder.Append("\n");
                        if (index >= 5)
                        {
                            builder.Append("...");
                            break;
                        }

                        var o = listValue[index];
                        builder.Append(o);
                    }

                    return builder.ToString();
                });
            }
            else
            {
                screenInfo = new ScreenInfo(() =>
                {
                    var val = fieldInfo.GetValue(ownerComponent);
                    var prefix = (ownerComponent != null ? ownerComponent.name : fieldInfo.DeclaringType?.Name);
                    return val != null
                        ? $"{prefix}.{fieldInfoName}: {val}"
                        : $"{prefix}.{fieldInfoName}: null";
                });
            }
            return screenInfo;
        }

        private static void ProcessEnumerableFieldInfoForComponent(
            FieldInfo fieldInfo,
            Component ownerComponent,
            Func<Type, IReadOnlyList<Component>> mapper)
        {
            var isArray = fieldInfo.FieldType.IsArray;  // Else it is a List, rest is filtered 

            var subType = isArray
                ? fieldInfo.FieldType.GetElementType()
                : fieldInfo.FieldType.GetGenericArguments()[0];
            var components = mapper?.Invoke(subType);
            if (isArray)
            {
                var converted = ConvertToArrayOfType(subType, components);
                fieldInfo.SetValue(ownerComponent, converted);
            }
            else
            {
                var converted = ConvertToListOfType(subType, components);
                fieldInfo.SetValue(ownerComponent, converted);
            }
        }

        private static Array ConvertToArrayOfType(Type type, IReadOnlyList<Component> sourceArray)
        {
            var arr = Array.CreateInstance(type, sourceArray.Count);
            for (var index = 0; index < sourceArray.Count; index++)
            {
                var component = sourceArray[index];
                arr.SetValue(Convert.ChangeType(component, type), index);
            }

            return arr;
        }

        private static IList ConvertToListOfType(Type type, IEnumerable<Component> components)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);
            var instance = Activator.CreateInstance(constructedListType) as IList;
            foreach (var component in components)
            {
                instance!.Add(Convert.ChangeType(component, type));
            }

            return instance;
        }
        
        private static bool IsTypeEnumerable(Type fieldType) => fieldType.IsArray || typeof(IEnumerable).IsAssignableFrom(fieldType);
    }
}
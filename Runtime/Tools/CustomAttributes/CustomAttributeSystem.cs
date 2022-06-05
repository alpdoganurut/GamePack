using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GamePack.CustomAttribute.Attributes;
using GamePack.Logging;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamePack.CustomAttributes
{
    public static class CustomAttributeSystem
    {
        private static readonly BindingFlags BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private static readonly Type[] AttributeTypeOrder = {
            typeof(AutoFillSelfAttribute),
            typeof(AutoFillChildrenAttribute),
            typeof(AutoFillSceneAttribute),
            typeof(RequireComponent),
        };

        private static readonly Dictionary<Type, Type[]> AttributeValidComponents = new Dictionary<Type, Type[]>
        {
            {typeof(AutoFillSelfAttribute), new []{typeof(Component)}},
            {typeof(AutoFillChildrenAttribute), new []{typeof(Component)}},
            {typeof(AutoFillSceneAttribute), new []{typeof(Component)}},
            {typeof(RequiredAttribute), new []{typeof(object)}},
        };

        private static readonly List<Type> OwnerTypes = new();
        private static readonly Dictionary<FieldInfo, List<Type>> FieldAttributeTypes = new();
        private static readonly Dictionary<Type, List<FieldInfo>> TypeFieldInfos = new();

        static CustomAttributeSystem()
        {
            // UnityEditor.Compilation.CompilationPipeline.compilationFinished += CompilationPipelineOnCompilationFinished;
            AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEventsOnAfterAssemblyReload;
        }

        private static void AssemblyReloadEventsOnAfterAssemblyReload()
        {
            FindTypesWithAutoFillSelf();
            ValidateAllInScene();
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
            
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj is PlayModeStateChange.EnteredPlayMode or PlayModeStateChange.EnteredEditMode) ValidateAllInScene();
        }

        [Button]
        private static void FindTypesWithAutoFillSelf()
        {
            ManagedLog.LogMethod();
            
            Clear();
            
            var asm = Assembly.Load("Assembly-CSharp");
            foreach (var type in asm.GetTypes())
            {
                if(!type.IsSubclassOf(typeof(Component))) continue;
                
                var fieldInfos = type.GetFields(BindingFlags);
                foreach (var fieldInfo in fieldInfos)
                {
                    // if(!(fieldInfo.FieldType.IsSubclassOf(typeof(Component)) || fieldInfo.FieldType.IsSubclassOf(typeof(GameObject)))) continue;
                    
                    var attributes = fieldInfo.GetCustomAttributes();
                    foreach (var fieldAttribute in attributes)
                    {
                        CheckFieldAttributeAndAdd<AutoFillSelfAttribute>(fieldAttribute, type, fieldInfo);
                        CheckFieldAttributeAndAdd<AutoFillChildrenAttribute>(fieldAttribute, type, fieldInfo);
                        CheckFieldAttributeAndAdd<AutoFillSceneAttribute>(fieldAttribute, type, fieldInfo);
                        CheckFieldAttributeAndAdd<RequiredAttribute>(fieldAttribute, type, fieldInfo);
                    }
                }
            }

            foreach (var attributeList in FieldAttributeTypes.Values)
            {
                attributeList.Sort((type, type1) => Array.IndexOf(AttributeTypeOrder, type1) - Array.IndexOf(AttributeTypeOrder, type) );
            }
            
            Debug.Log("Found attributes for:");
            foreach (var type in OwnerTypes)
            {
                Debug.Log(type);
            }
        }

        private static void CheckFieldAttributeAndAdd<T>(Attribute attribute, Type ownerType, FieldInfo fieldInfo)
        {
            if (attribute is not T) return;
            
            var attributeType = typeof(T);
            
            if (!AttributeValidComponents[attributeType].Any(type => fieldInfo.FieldType.IsSubclassOf(type) || type == fieldInfo.FieldType))
            {
                LogValidationError($"type: {fieldInfo.FieldType} for field: {fieldInfo.Name} in class: {fieldInfo.DeclaringType} is not valid for attribute {attributeType}");
                return;
            }
            
            AddNewField(ownerType, attributeType, fieldInfo);
        }

        private static void AddNewField(Type ownerType, Type attributeType, FieldInfo fieldInfo)
        {
            if(!FieldAttributeTypes.ContainsKey(fieldInfo))
                FieldAttributeTypes.Add(fieldInfo, new List<Type> {attributeType});
            
            if (!OwnerTypes.Contains(ownerType))
            {
                OwnerTypes.Add(ownerType);
                TypeFieldInfos.Add(ownerType, new List<FieldInfo> {fieldInfo});
            }
            else
            {
                if(!TypeFieldInfos[ownerType].Contains(fieldInfo))
                    TypeFieldInfos[ownerType].Add(fieldInfo);
                if (!FieldAttributeTypes[fieldInfo].Contains(attributeType))
                    FieldAttributeTypes[fieldInfo].Add(attributeType);
            }
        }

        [Button]
        private static void ValidateAllInScene()
        {
            ManagedLog.LogMethod();
            
            foreach (var ownerType in OwnerTypes)
            {
                var ownerComponents = Object.FindObjectsOfType(ownerType).Cast<Component>();

                foreach (var ownerComponent in ownerComponents)
                {
                    Debug.Log($"Filling {ownerComponent}");

                    foreach (var fieldInfo in TypeFieldInfos[ownerType])
                    {
                        var isUnityObject = ownerType.IsSubclassOf(typeof(Object)) || fieldInfo.FieldType == typeof(Object);
                        var isString = ownerType.IsSubclassOf(typeof(string)) || fieldInfo.FieldType == typeof(string);
                        
                        foreach (var attributeType in FieldAttributeTypes[fieldInfo])
                        {
                            var val = fieldInfo.GetValue(ownerComponent);
                            
                            if(isUnityObject && val as Object != null) continue;
                            if(isString && !string.IsNullOrWhiteSpace(val as string)) continue;
                            if(val is null) continue;
                            
                            ValidateFieldForAttributeType(attributeType, fieldInfo, ownerComponent);
                        }
                    }
                }
            }
        }

        private static void ValidateFieldForAttributeType(Type attributeType, FieldInfo fieldInfo, Component ownerComponent)
        {
            if (attributeType == typeof(AutoFillSelfAttribute))
            {
                var value = ownerComponent.GetComponent(fieldInfo.FieldType);
                fieldInfo.SetValue(ownerComponent, value);
            }
            else if (attributeType == typeof(AutoFillChildrenAttribute))
            {
                var value = ownerComponent.GetComponentsInChildren(fieldInfo.FieldType)
                    .FirstOrDefault(component => component.gameObject != ownerComponent.gameObject);
                
                fieldInfo.SetValue(ownerComponent, value);
            }
            else if(attributeType == typeof(AutoFillSceneAttribute))
            {
                var value = Object.FindObjectOfType(fieldInfo.FieldType, true);
                fieldInfo.SetValue(ownerComponent, value);
            }
            else if (attributeType == typeof(RequiredAttribute))
                LogValidationError( $"Field {fieldInfo.Name} of type {fieldInfo.DeclaringType?.Name} is not set. ({ownerComponent.GetScenePath()})",
                    ownerComponent);
            else
                LogValidationError($"{attributeType} is not handled.");
        }

        [Button]
        private static void Clear()
        {
            OwnerTypes.Clear();
            FieldAttributeTypes.Clear();
            TypeFieldInfos.Clear();
        }

        private static void LogValidationError(string msg, Object obj = null)
        {
            Debug.Log($"<color=red>[VALIDATION]</color> {msg}", obj);
        }
    }
}
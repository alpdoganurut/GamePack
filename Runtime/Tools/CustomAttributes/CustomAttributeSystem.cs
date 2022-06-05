using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Editor.EditorDrawer;
using Editor.EditorDrawer.Buttons;
using GamePack.CustomAttributes.Attributes;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamePack.CustomAttributes
{
    public static class CustomAttributeSystem
    {
        #region Configuration

        private static readonly BindingFlags BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;

        private static readonly Type[] AttributeTypesOrdered = {
            typeof(AutoFillSelfAttribute),
            typeof(AutoFillChildrenAttribute),
            typeof(AutoFillSceneAttribute),
            typeof(RenameInHierarchyAttribute),
            typeof(RequiredAttribute),
            typeof(MonitorAttribute),
        };

        private static readonly Dictionary<Type, Type[]> AttributeValidComponents = new Dictionary<Type, Type[]>
        {
            {typeof(AutoFillSelfAttribute), new []{typeof(Component)}},
            {typeof(AutoFillChildrenAttribute), new []{typeof(Component), typeof(IEnumerable<Component>)}},
            {typeof(AutoFillSceneAttribute), new []{typeof(Component), typeof(IEnumerable<Component>)}},
            {typeof(RenameInHierarchyAttribute), new []{typeof(GameObject), typeof(Component)}},
            {typeof(RequiredAttribute), new []{typeof(object)}},
            {typeof(MonitorAttribute), new []{typeof(object)}},
        };

        #endregion

        #region Field Cache

        private static readonly List<Type> OwnerTypes = new();
        private static readonly Dictionary<Type, List<FieldInfo>> TypeFieldInfos = new();
        private static readonly Dictionary<FieldInfo, List<Type>> FieldAttributeTypes = new();

        private static readonly List<ScreenInfo> ScreenInfos = new();

        #endregion
        
        #region Method Cache

        private static readonly Type[] MethodAttributeTypesOrdered = {
            typeof(ScreenButtonAttribute),
        };
        
        private static readonly List<Type> MethodOwnerTypes = new();
        private static readonly Dictionary<Type, List<MethodInfo>> TypeMethodInfos = new();
        private static readonly Dictionary<MethodInfo, List<Type>> MethodAttributeTypes = new();

        private static readonly List<DynamicButton> MethodScreenInfos = new();

        #endregion

        #region Initilisation & Reloading & Clearing

        static CustomAttributeSystem()
        {
            // UnityEditor.Compilation.CompilationPipeline.compilationFinished += CompilationPipelineOnCompilationFinished;
            AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEventsOnAfterAssemblyReload;
        }

        private static void AssemblyReloadEventsOnAfterAssemblyReload()
        {
            CacheFields();
            CacheMethods();
            ProcessScene();
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
            EditorDrawerSystem.RegisterDynamicButton(new DynamicButton("Validate Scene", ProcessScene));
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj is PlayModeStateChange.EnteredPlayMode or PlayModeStateChange.EnteredEditMode) ProcessScene();
        }

        private static void ClearCache()
        {
            OwnerTypes.Clear();
            TypeFieldInfos.Clear();
            FieldAttributeTypes.Clear();
            
            MethodOwnerTypes.Clear();
            TypeMethodInfos.Clear();
            MethodAttributeTypes.Clear();
        }

        #endregion

        #region Field Caching

        private static void CacheFields()
        {
            ClearCache();
            
            var asm = Assembly.Load("Assembly-CSharp");
            foreach (var type in asm.GetTypes())
            {
                if(!type.IsSubclassOf(typeof(Component))) continue;
                
                var fieldInfos = type.GetFields(BindingFlags);
                foreach (var fieldInfo in fieldInfos)
                {
                    var attributes = fieldInfo.GetCustomAttributes();
                    foreach (var fieldAttribute in attributes)
                    foreach (var attributeType in AttributeTypesOrdered)
                        MatchFieldAttributeAndAdd(attributeType, fieldAttribute, type, fieldInfo);
                }
            }

            foreach (var attributeList in FieldAttributeTypes.Values)
            {
                attributeList.Sort((type, type1) => Array.IndexOf(AttributeTypesOrdered, type) - Array.IndexOf(AttributeTypesOrdered, type1) );
            }
        }

        private static void MatchFieldAttributeAndAdd(Type attributeTypeToMatch, Attribute attribute, Type ownerType, FieldInfo fieldInfo)
        {
            if (attribute.GetType() != attributeTypeToMatch) return;
            
            if (!AttributeValidComponents[attributeTypeToMatch].Any(type => fieldInfo.FieldType.IsSubclassOf(type) || type == fieldInfo.FieldType || type.IsAssignableFrom(fieldInfo.FieldType)))
            {
                LogValidationError($"type: {fieldInfo.FieldType} for field: {fieldInfo.Name} in class: {fieldInfo.DeclaringType} is not valid for attribute {attributeTypeToMatch}");
                return;
            }
            
            AddNewFieldToCache(ownerType, attributeTypeToMatch, fieldInfo);
        }

        private static void AddNewFieldToCache(Type ownerType, Type attributeType, FieldInfo fieldInfo)
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

        #endregion

        #region Method Caching

        private static void CacheMethods()
        {
            var asm = Assembly.Load("Assembly-CSharp");
            foreach (var type in asm.GetTypes())
            {
                if(!type.IsSubclassOf(typeof(Component))) continue;

                var methodInfos = type.GetMethods(BindingFlags);
                foreach (var methodInfo in methodInfos)
                {
                    var customAttributes = methodInfo.GetCustomAttributes();
                    foreach (var methodAttribute in customAttributes)
                    {
                        foreach (var attributeTypeToMatch in MethodAttributeTypesOrdered)
                        {
                            if (methodAttribute.GetType() != attributeTypeToMatch) continue;
                            
                            AddNewMethodToCache(type, attributeTypeToMatch, methodInfo);
                        }
                    }
                }
            }
        }
        
        private static void AddNewMethodToCache(Type ownerType, Type attributeType, MethodInfo methodInfo)
        {
            if(!MethodAttributeTypes.ContainsKey(methodInfo))
                MethodAttributeTypes.Add(methodInfo, new List<Type> {attributeType});
            
            if (!MethodOwnerTypes.Contains(ownerType))
            {
                MethodOwnerTypes.Add(ownerType);
                TypeMethodInfos.Add(ownerType, new List<MethodInfo> {methodInfo});
            }
            else
            {
                if(!TypeMethodInfos[ownerType].Contains(methodInfo))
                    TypeMethodInfos[ownerType].Add(methodInfo);
                if (!MethodAttributeTypes[methodInfo].Contains(attributeType))
                    MethodAttributeTypes[methodInfo].Add(attributeType);
            }
        }

        #endregion
        
        private static void ProcessScene()
        {
            ProcessFields();
            ProcessMethods();
        }

        #region Field Processing

        private static void ProcessFields()
        {
            ClearScreenInfos();
            ProcessStaticFields();
            ProcessInstanceFields();
        }

        private static void ProcessInstanceFields()
        {
            foreach (var ownerType in OwnerTypes)
            {
                var sceneComponents = Object.FindObjectsOfType(ownerType).Cast<Component>().ToArray();
                ProcessComponentsFields(sceneComponents, ownerType);
            }
        }

        private static void ProcessComponentsFields(IEnumerable<Component> sceneComponents, Type ownerType)
        {
            foreach (var ownerComponent in sceneComponents)
            {
                foreach (var fieldInfo in TypeFieldInfos[ownerType])
                {
                    if(fieldInfo.IsStatic) continue;
                    
                    var fieldType = fieldInfo.FieldType;
                    var isUnityObject = fieldType.IsSubclassOf(typeof(Object)) || fieldType == typeof(Object);
                    var isString = fieldType.IsSubclassOf(typeof(string)) || fieldType == typeof(string);
                    var isNative = !(isUnityObject || isString);
                    var isEnumerable = IsFieldTypeEnumerable(fieldType);

                    foreach (var attributeType in FieldAttributeTypes[fieldInfo])
                    {
                        var val = fieldInfo.GetValue(ownerComponent);
                        var isValid = !isEnumerable // Enumerable is always invalid - to get new objects 
                                      && (isUnityObject && val as Object != null) // Cast and check
                                      || (isString && !string.IsNullOrWhiteSpace(val as string)) // Invalid for whitespace
                                      || (isNative && !isEnumerable && val is not null); // Invalid if null

                        // Process Field by AttributeType
                        if (!isValid) ProcessInvalidField(attributeType, fieldInfo, ownerComponent);
                        ProcessField(attributeType, fieldInfo, ownerComponent);
                    }
                }
            }
        }

        private static void ProcessField(Type attributeType, FieldInfo fieldInfo, Component ownerComponent)
        {
            var fieldInfoName = fieldInfo.Name;
            
            if (ownerComponent && attributeType == typeof(RenameInHierarchyAttribute))
            {
                var val = fieldInfo.GetValue(ownerComponent);
                var comp = val as Component;
                if (comp) comp.name = fieldInfoName;
                
                var go = val as GameObject;
                if (go) go.name = fieldInfoName;
            }
            else if (attributeType == typeof(MonitorAttribute))
                ScreenInfos.Add(AttributeProcessors.ProcessMonitorAttribute(fieldInfo, ownerComponent, fieldInfoName));
        }

        private static void ProcessStaticFields()
        {
            foreach (var (methodInfo, attributes) in FieldAttributeTypes)
            {
                if (!methodInfo.IsStatic) continue;

                foreach (var attribute in attributes)
                {
                    ProcessField(attribute, methodInfo, null);
                }
            }
        }

        private static void ProcessInvalidField(Type attributeType, FieldInfo fieldInfo, Component ownerComponent)
        {
            if (attributeType == typeof(AutoFillSelfAttribute))
            {
                AttributeProcessors.ProcessAutoFillSelfAttribute(fieldInfo, ownerComponent);
            }
            else if (attributeType == typeof(AutoFillChildrenAttribute))
            {
                AttributeProcessors.ProcessAutoFillChildrenAttribute(fieldInfo, ownerComponent);
            }
            else if(attributeType == typeof(AutoFillSceneAttribute))
            {
                AttributeProcessors.ProcessAutoFillSceneAttribute(fieldInfo, ownerComponent);
            }
            else if (attributeType == typeof(RequiredAttribute) && !IsFieldTypeEnumerable(fieldInfo.FieldType))
            {
                LogValidationError(
                    $"Field {fieldInfo.Name} of type {fieldInfo.DeclaringType?.Name} is not set. ({ownerComponent.GetScenePath()})",
                    ownerComponent);
            }
        }

        private static void ClearScreenInfos()
        {
            foreach (var screenInfo in ScreenInfos)
            {
                screenInfo?.Delete();
            }

            ScreenInfos.Clear();
        }

        #endregion

        #region Method Processing

        private static void ProcessMethods()
        {
            ClearMethodButtons();
            ProcessStaticMethods();
            ProcessInstanceMethods();
        }

        private static void ProcessInstanceMethods()
        {
            foreach (var ownerType in MethodOwnerTypes)
            {
                var sceneComponents = Object.FindObjectsOfType(ownerType).Cast<Component>().ToArray();
                ProcessComponentsInstanceMethods(sceneComponents, ownerType);
            }
        }

        private static void ProcessStaticMethods()
        {
            foreach (var (methodInfo, attributes) in MethodAttributeTypes)
            {
                if (!methodInfo.IsStatic) continue;

                foreach (var attribute in attributes)
                {
                    if (attribute == typeof(ScreenButtonAttribute))
                    {
                        var buttonLabel = $"{methodInfo.DeclaringType?.Name}.{methodInfo.Name} ( )";

                        var button = new DynamicButton(buttonLabel,
                            () => { methodInfo.Invoke(null, Array.Empty<object>()); });

                        MethodScreenInfos.Add(button);
                        EditorDrawerSystem.RegisterDynamicButton(button);
                    }
                }
            }
        }

        private static void ProcessComponentsInstanceMethods(IEnumerable<Component> components, Type ownerType)
        {
            foreach (var ownerComponent in components)
            {
                foreach (var methodInfo in TypeMethodInfos[ownerType])
                {
                    foreach (var attributeType in MethodAttributeTypes[methodInfo])
                    {
                        ProcessMethod(attributeType, methodInfo, ownerComponent);
                    }
                }
            }
        }

        private static void ProcessMethod(Type attributeType, MethodInfo methodInfo, Component ownerComponent)
        {
            if (attributeType == typeof(ScreenButtonAttribute))
            {
                if (methodInfo.IsStatic) return;
                /*{
                    if(ButtonCreatedStaticMethods.Contains(methodInfo)) return;
                    
                    ButtonCreatedStaticMethods.Add(methodInfo);
                }*/

                var buttonLabel = methodInfo.IsStatic ? $"{methodInfo.Name} ( )" : $"{ownerComponent.name}.{methodInfo.Name} ( )";
                
                var button = new DynamicButton(buttonLabel,
                    () => { methodInfo.Invoke(ownerComponent, Array.Empty<object>()); });

                MethodScreenInfos.Add(button);
                EditorDrawerSystem.RegisterDynamicButton(button);

            }
        }

        private static void ClearMethodButtons()
        {
            foreach (var screenInfo in MethodScreenInfos)
                EditorDrawerSystem.UnregisterDynamicButton(screenInfo);
            MethodScreenInfos.Clear();
        }

        #endregion

        #region Helpers

        private static bool IsFieldTypeEnumerable(Type fieldType) => fieldType.IsArray || typeof(IEnumerable).IsAssignableFrom(fieldType);

        public static void LogValidationError(string msg, Object obj = null)
        {
            Debug.Log($"<color=red>[VALIDATION]</color> {msg}", obj);
        }

        #endregion
    }
}
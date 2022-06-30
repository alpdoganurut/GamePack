using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GamePack.CustomAttributes.Attributes;
using GamePack.Utilities;
using Shared.EditorDrawer;
using Shared.EditorDrawer.Buttons;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            typeof(HandleAttribute),
        };

        private static readonly Dictionary<Type, Type[]> AttributeValidComponents = new Dictionary<Type, Type[]>
        {
            {typeof(AutoFillSelfAttribute), new []{typeof(Component)}},
            {typeof(AutoFillChildrenAttribute), new []{typeof(Component), typeof(IEnumerable<Component>)}},
            {typeof(AutoFillSceneAttribute), new []{typeof(Component), typeof(IEnumerable<Component>)}},
            {typeof(RenameInHierarchyAttribute), new []{typeof(GameObject), typeof(Component)}},
            {typeof(RequiredAttribute), new []{typeof(object)}},
            {typeof(MonitorAttribute), new []{typeof(object)}},
            {typeof(HandleAttribute), new []{typeof(Vector3)}},
        };

        private static readonly Assembly[] Assemblies = new []{Assembly.Load("Assembly-CSharp"), Assembly.Load("com.alpdoganurut.gamepack.main")};

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
            ClearCache();
            
            CacheFields();
            CacheMethods();
            
            ProcessScene();
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
            EditorSceneManager.sceneOpened += EditorSceneManagerOnSceneOpened;
            SceneManager.sceneLoaded += EditorSceneManagerOnSceneLoaded;
            // EditorDrawerSystem.RegisterDynamicButton(new DynamicButton("Validate Scene", ProcessScene));
        }

        private static void EditorSceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1) => ProcessScene();

        private static void EditorSceneManagerOnSceneOpened(Scene scene, OpenSceneMode mode) => ProcessScene();

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
            
            foreach (var a in Assemblies)
            foreach (var type in a.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Component))) continue;

                foreach (var fieldInfo in type.GetFields(BindingFlags))
                foreach (var fieldAttribute in fieldInfo.GetCustomAttributes())
                foreach (var attributeType in AttributeTypesOrdered)
                    MatchFieldAttributeAndAdd(attributeType, fieldAttribute, type, fieldInfo);
            }

            // Sort attributes by order
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
                var isDeclaringTypeIsComponent = typeof(Component).IsAssignableFrom(type);
                foreach (var methodInfo in type.GetMethods(BindingFlags))
                foreach (var methodAttribute in methodInfo.GetCustomAttributes())
                foreach (var attributeTypeToMatch in MethodAttributeTypesOrdered)
                {
                    if (methodAttribute.GetType() != attributeTypeToMatch) continue;

                    // Hacky ScreenButton validation
                    if (!IsMethodValidForScreenButton(attributeTypeToMatch, methodInfo, isDeclaringTypeIsComponent)) continue;

                    AddNewMethodToCache(type, attributeTypeToMatch, methodInfo);
                }
            }
        }

        private static bool IsMethodValidForScreenButton(
            Type attributeTypeToMatch, 
            MethodInfo methodInfo,
            bool isDeclaringTypeIsComponent)
        {
            if (attributeTypeToMatch == typeof(ScreenButtonAttribute)
                && methodInfo.GetParameters().Length > 0)
            {
                Debug.LogError($"Can't use ScreenButton attribute for methods with parameters.");
                return false;
            }

            if (attributeTypeToMatch == typeof(ScreenButtonAttribute)
                && !isDeclaringTypeIsComponent
                && !methodInfo.IsStatic)
            {
                Debug.LogError(
                    $"{methodInfo.DeclaringType?.Name}.{methodInfo.Name} is not static. Declaring type must be derived from Component when using {nameof(ScreenButtonAttribute)} with non static methods.");
                return false;
            }

            return true;
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
        
        public static void ProcessScene()
        {
            ProcessFields();
            ProcessMethods();
            HandleDrawing.UpdateSelection();
        }

        #region Field Processing

        private static void ProcessFields()
        {
            HandleDrawing.Clear();
            ClearScreenInfos();
            ProcessStaticFields();
            ProcessInstanceFields();
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

        private static void ProcessInstanceFields()
        {
            foreach (var ownerType in OwnerTypes)
            {
                if(ownerType.IsGenericType) continue;
                var sceneComponents = Object.FindObjectsOfType(ownerType, true).Cast<Component>().ToArray();
                ProcessComponentsFieldsForType(sceneComponents, ownerType);
            }
        }

        private static void ProcessComponentsFieldsForType(IEnumerable<Component> sceneComponents, Type ownerType)
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
            {
                var monitorAttribute = (MonitorAttribute) fieldInfo.GetCustomAttribute(attributeType);
                switch (monitorAttribute.DrawType)
                {
                    case DrawType.GUI:
                        ScreenInfos.Add(AttributeProcessors.ProcessMonitorAttribute(fieldInfo, ownerComponent));
                        break;
                    case DrawType.WorldSelected:
                        HandleDrawing.AddInfo(fieldInfo, ownerComponent, SpaceType.Local);
                        break;
                    case DrawType.WorldAnyTime:
                        HandleDrawing.AddInfo(fieldInfo, ownerComponent, SpaceType.Local, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (attributeType == typeof(HandleAttribute))
            {
                var handleAttribute = (HandleAttribute) fieldInfo.GetCustomAttribute(attributeType);
                HandleDrawing.AddInfo(fieldInfo, ownerComponent,
                    handleAttribute.Space);
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
                if(!typeof(Component).IsAssignableFrom(ownerType)) continue;    // Don't handle if not Component
                
                var sceneComponents = Object.FindObjectsOfType(ownerType, true).Cast<Component>().ToArray();
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
                        var buttonLabel = $"{methodInfo.DeclaringType?.Name}.{methodInfo.Name}()";

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

                var buttonLabel = $"{ownerComponent.name}.{methodInfo.Name}()";
                
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
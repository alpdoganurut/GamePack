#region Development
#if UNITY_EDITOR
using UnityEditor; 
#endif
#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    public class PrefabContainer: MonoBehaviour
    {
        private const string EditorOnlyTag = "EditorOnly";
        
        [SerializeField, ReadOnly] private GameObject _Prefab;
        [ShowInInspector] private GameObject _instance;

        private void Awake()
        {
            
            #region Development
#if UNITY_EDITOR
            transform.GetChild(0).gameObject.SetActive(false);
#endif
            #endregion
            
            if(!_instance)
                CreateInstance();
        }

        private void CreateInstance()
        {
            _instance = Instantiate(_Prefab, transform.position, Quaternion.identity);
            _instance.transform.SetParent(transform);
        }

        public T GetInstance<T>() where T: Component
        {
            if(!_instance) CreateInstance();
            return _instance.GetComponent<T>();
        }

        public static IEnumerable<PrefabContainer> FindContainersForType<T>() where T: Component
        {
            var objs = FindObjectsOfType<T>();

            foreach (var component in objs)
            {
                if(!component.transform.parent) continue;
                var container = component.transform.parent.gameObject.GetComponent<PrefabContainer>();
                if (container)
                    yield return container;
            }
        }
        
        #region Development
#if UNITY_EDITOR
        [MenuItem("Utilities/Create Prefab Container")]
        public static void CreateContainer()
        {
            var sel = Selection.gameObjects;

            foreach (var obj in sel)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (!prefab)
                {
                    Debug.LogError($"{obj.name} is not connected to a prefab!");
                    continue;
                }
                
                var parent = new GameObject($"{obj.name} Container");
                parent.transform.SetParent(obj.transform.parent);
                
                parent.transform.position = obj.transform.position;
                parent.transform.rotation = obj.transform.rotation;
                
                obj.transform.SetParent(parent.transform, true);
                
                var container = parent.AddComponent<PrefabContainer>();
                container.Setup();
            }
        }

        [Button]
        private void Setup()
        {
            if(transform.childCount == 0) return;
            if(transform.childCount > 1)
            {
                Debug.LogError($"{nameof(PrefabContainer)} can't have more than 1 child!");
                return;
            }

            var instance = transform.GetChild(0);
            var isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(instance.gameObject);

            EditorUtility.SetDirty(gameObject);
            Undo.RecordObject(instance, $"{instance.name} Prefab Container Setup");
            
            if (!isPrefab)
            {
                Debug.LogError($"{instance.name} is not a prefab root!");
                return;
            }
            
            instance.gameObject.tag = EditorOnlyTag;

            _Prefab = PrefabUtility.GetCorrespondingObjectFromSource(instance.gameObject);
        } 
#endif
        #endregion
    }
}
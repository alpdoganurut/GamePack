using UnityEngine;

namespace GamePack.Utilities
{
    /// <summary>
    /// This is a basic Singleton implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        /// Override to create new instance automatically when Instance is accessed.
        protected virtual bool IsAutoCreate => false;
        
        public static bool HasInstance => !IsDestroyed;

        private static bool IsDestroyed => _instance == null;
        
        protected virtual void Awake()
        {
            _instance = this as T;
        }

        public static T Instance {
            get {
                Instantiate();
                return _instance;
            }
        }
    
        private static void Instantiate() {
            if (!_instance) {
                _instance = FindObjectOfType(typeof(T)) as T;
            }

            if (!_instance)
            {
                _instance = new GameObject($"{typeof(T)}").AddComponent<T>();
                if (_instance && _instance is Singleton<T> singleton && !singleton.IsAutoCreate)
                {
                    _instance = null;
                    Debug.LogError($"{typeof(T)} is not found and will not be instantiated because IsAutoCreate is false.");
                }
            }
        }

    }
}
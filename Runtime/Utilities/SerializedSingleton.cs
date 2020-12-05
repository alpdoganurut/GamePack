using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.UnityUtilities
{
    /// <summary>
    /// This is a basic Singleton implementation with OdinInspectors serialization capabilities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SerializedSingleton<T> : SerializedMonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
  
        protected virtual void Awake()
        {
            _instance = this as T;
        }

        // ReSharper disable once MemberCanBeProtected.Global
        public static T Instance {
            get {
                Instantiate();
                return _instance;
            }
        }

        private static void Instantiate() {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }
        }
    }
}

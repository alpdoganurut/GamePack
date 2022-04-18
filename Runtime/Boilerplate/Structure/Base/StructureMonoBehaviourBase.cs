using GamePack.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamePack.Boilerplate.Structure
{
    public abstract class StructureMonoBehaviourBase : MonoBehaviour
    {
        // private Transform _cachedTransform;
        private bool _isUpdating;
        
        #region Internal Access

        private protected Transform Internal_Transform => transform;
        
        private protected GameObject Internal_GameObject => base.gameObject;

        #endregion
        
        #region Access Restriction

        // ReSharper disable once InconsistentNaming
        public new GameObject gameObject
        {
            get
            {
                Debug.LogWarning($"Shouldn't access {nameof(gameObject)} of {nameof(ControllerBase)}");
                return base.gameObject;
            }
        }

        // ReSharper disable once InconsistentNaming
        public new Transform transform
        {
            get
            {
                
                Debug.LogWarning($"Shouldn't access {nameof(gameObject)} of {nameof(ControllerBase)}");
                return base.transform;
            }
        }

        // ReSharper disable once InconsistentNaming
        public new bool enabled
        {
            get
            {
                Debug.LogWarning($"Shouldn't access {nameof(enabled)} of {nameof(ControllerBase)}");
                return base.enabled;
            }
            set
            {
                Debug.LogWarning($"Shouldn't access {nameof(enabled)} of {nameof(ControllerBase)}");
                base.enabled = value;
            }
        }

        #endregion

        #region Logging Helper Methods

        protected void Log(object obj, ManagedLog.Type type = ManagedLog.Type.Default, Object context = null) => ManagedLog.Log(obj, type, context ? context : this);
        protected void LogMethod(object obj = null, ManagedLog.Type type = ManagedLog.Type.Default, Object context = null) => ManagedLog.LogMethod(obj: obj, type: type, context: context, stackOffset: 1);

        #endregion

        #region Object.Instantiate overrides to register new objects in StructureManager

        /// <summary>
        ///   <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <returns>
        ///   <para>The instantiated clone.</para>
        /// </returns>
        public new static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
        {
            var instantiate = MonoBehaviour.Instantiate(original, position, rotation);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        /// <summary>
        ///   <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>
        ///   <para>The instantiated clone.</para>
        /// </returns>
        public new static Object Instantiate(
            Object original,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
        {
            var instantiate = MonoBehaviour.Instantiate(
                original,
                position,
                rotation,
                parent);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        /// <summary>
        ///   <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <returns>
        ///   <para>The instantiated clone.</para>
        /// </returns>
        public new static Object Instantiate(Object original)
        {
            var instantiate = MonoBehaviour.Instantiate(
                original);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        /// <summary>
        ///   <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>
        ///   <para>The instantiated clone.</para>
        /// </returns>
        public new static Object Instantiate(Object original, Transform parent)
        {
            var instantiate = MonoBehaviour.Instantiate(original, parent);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        /// <summary>
        ///   <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <param name="instantiateInWorldSpace">When you assign a parent Object, pass true to position the new object directly in world space. Pass false to set the Object’s position relative to its new parent..</param>
        /// <returns>
        ///   <para>The instantiated clone.</para>
        /// </returns>
        public new static Object Instantiate(
            Object original,
            Transform parent,
            bool instantiateInWorldSpace)
        {
            var instantiate = MonoBehaviour.Instantiate(original, parent, instantiateInWorldSpace);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        public new static T Instantiate<T>(T original) where T : Object
        {
            var instantiate = MonoBehaviour.Instantiate(original);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        public new static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            var instantiate = MonoBehaviour.Instantiate(original, position, rotation);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        public new static T Instantiate<T>(
            T original,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : Object
        {
            var instantiate = MonoBehaviour.Instantiate(original, position, rotation, parent);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        public new static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            var instantiate = MonoBehaviour.Instantiate(original, parent);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        public new static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            var instantiate = MonoBehaviour.Instantiate(original, parent, worldPositionStays);
            StructureManager.RegisterViewOrController(instantiate);
            return instantiate;
        }

        #endregion
    }
}
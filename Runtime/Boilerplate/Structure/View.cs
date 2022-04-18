using System;
using GamePack.Logging;
using GamePack.Utilities;
using UnityEngine;

namespace GamePack.Boilerplate.Structure
{
    public class View: StructureMonoBehaviourBase
    {
        private bool _isInitiated;
        private Transform _transform;
        
        public bool IsVisible => Internal_GameObject.activeInHierarchy;

        public Transform Transform
        {
            get
            {
                if(!_isInitiated) Initiate();
                return _transform;
            }
        } 

        private void Initiate()
        {
            _transform = Internal_Transform;
            _isInitiated = true;
        }
        
        internal void Internal_OnLoad()
        {
            Initiate();
        }

        internal virtual void Internal_OnUpdate() { }
        
        public void DestroyView()
        {
            Destroy(Internal_GameObject);
        }

        public void SetIsVisible(bool isVisible)
        {
            Internal_GameObject.SetActive(isVisible);
            
#if UNITY_EDITOR
            if(!Internal_GameObject.activeInHierarchy)
            {
                var firstDisabledParent = FindDisabledParent(Internal_Transform);
                ManagedLog.LogError($"{gameObject.GetScenePath()} will not be visible because one of it's parents ({firstDisabledParent.gameObject.GetScenePath()} is disabled. ");
            }
            Transform FindDisabledParent(Transform t)
            {
                while (true)
                {
                    if (!t.parent.gameObject.activeSelf) return t.parent;
                    if (t.parent == null) return null;
                    t = t.parent;
                }
            }
#endif
        }

        public void LogInWorld(object msg)
        {
#if USING_SHAPES
            WorldLog.Log(msg, localTransform: transform);
#endif
        }
    }
}
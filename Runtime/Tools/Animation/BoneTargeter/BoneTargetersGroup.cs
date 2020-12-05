using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Animation
{
    public class BoneTargetersGroup: MonoBehaviour
    {
        [SerializeField, Required] private List<BoneTargeter> _Children = new List<BoneTargeter>();

        #region Development
#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var boneTargeter in _Children)
            {
                boneTargeter.Group = this;
            }
        } 
#endif
        #endregion

        public void SetIsActive(bool isActive)
        {
            foreach (var boneTargeter in _Children)
            {
                boneTargeter.SetIsActive(isActive);
            }
        }
        
        private void LateUpdate()
        {
            foreach (var animationRotater in _Children)
            {
                animationRotater.OrderedUpdate();
            }
        }

        public void SetTarget(Transform target, float delay)
        {
            LeanTween.cancel(gameObject);
            LeanTween.delayedCall(gameObject, delay, () =>
            {
                SetTarget(target);
            });
        }

        public void SetTarget(Transform target)
        {
            Debug.Log($"Target set to {target.name}");
            LeanTween.cancel(gameObject);
            foreach (var boneTargeter in _Children)
            {
                boneTargeter.Target = target;
            }
        }

    }
}
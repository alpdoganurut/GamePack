using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePack
{
    public class LeanTweenStates
    {
        private readonly Action<float, TransformInfo>[] _tweens;
        private readonly GameObject _tweenGameObject;
        private readonly float _duration;
        private readonly LeanTweenType _easing;
        private readonly GameObject _targetObject;

        public LeanTweenStates(GameObject targetObject,  Action<float, TransformInfo>[] tweens, float duration, LeanTweenType easing = LeanTweenType.easeOutSine)
        {
            _targetObject = targetObject;
            _tweens = tweens;
            _tweenGameObject = new GameObject {hideFlags = HideFlags.HideAndDontSave};
            _duration = duration;
            _easing = easing;
        }

        public void SetState(int index)
        {
            Assert.IsTrue(index < _tweens.Length);

            var transformInfo = new TransformInfo(_targetObject.transform);
            
            LeanTween.value(_tweenGameObject, 0, 1, _duration).setOnUpdate((float val) =>
            {
                _tweens[index].Invoke(val, transformInfo);
            }).setEase(_easing);

        }

        public struct TransformInfo
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 LocalScale;

            public TransformInfo(Transform transform)
            {
                Position = transform.position;
                Rotation = transform.rotation;
                LocalScale = transform.localScale;
            }
        }
    }
}
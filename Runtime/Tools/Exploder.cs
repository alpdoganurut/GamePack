using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePack
{
    public class Exploder : MonoBehaviour
    {
        [SerializeField] private float _ExplodeDistance = 5;
        [SerializeField] private float _ExplodeTime = .5f;
        [SerializeField] private float _ResetTime = .3f;
        [SerializeField] private LeanTweenType _ExplodeEasing = LeanTweenType.easeInOutSine;
        [SerializeField] private LeanTweenType _ResetEasing = LeanTweenType.easeInOutSine;
        [SerializeField, ReadOnly] private Transform[] _Children;
        [SerializeField, ReadOnly] private Vector3[] _ChildrenOriginalPositions;
        [SerializeField, ReadOnly] private Vector3[] _ChildrenOriginalScale;
        [SerializeField, Required] private Transform[] _Excluded;

        private void OnValidate()
        {
            _Children = GetComponentsInChildren<Transform>().Where(t => t != transform && !_Excluded.Contains(t)).ToArray();
            _ChildrenOriginalPositions = _Children.Select(rigidbody1 => rigidbody1.transform.localPosition).ToArray();
            _ChildrenOriginalScale = _Children.Select(rigidbody1 => rigidbody1.transform.localScale).ToArray();
        }

        [Button]
        public void ExplodeNow()
        {
            var childrenCurrentScale = _Children.Select(t => t.localScale).ToArray();
            var childrenCurrentPos = _Children.Select(t => t.localPosition).ToArray();
            var childrenExplodePos = _Children.Select(t =>
                (Random.onUnitSphere * _ExplodeDistance)).ToArray();

            foreach (var transform1 in _Excluded)
            {
                transform1.gameObject.SetActive(false);
            }

            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0, 1, _ExplodeTime).setOnUpdate(val =>
                {
                    for (var index = 0; index < _Children.Length; index++)
                    {
                        var child = _Children[index];

                        child.transform.localPosition =
                            Vector3.Lerp(childrenCurrentPos[index], childrenExplodePos[index], val);
                        child.transform.localScale = Vector3.Lerp(childrenCurrentScale[index], Vector3.zero, val);
                    }
                })
                .setEase(_ExplodeEasing)
                .setOnComplete(() =>
                {
                    foreach (var child in _Children)
                    {
                        child.gameObject.SetActive(false);
                    }
                });
        }

        [Button]
        public void ResetTarget(Action callback = null)
        {
            foreach (var child in _Children)
            {
                child.gameObject.SetActive(true);
            }

            var childrenCurrentPos = _Children.Select(rigidbody1 => rigidbody1.transform.localPosition).ToArray();
            var childrenCurrentScale = _Children.Select(rigidbody1 => rigidbody1.transform.localPosition).ToArray();

            // _Children.ForEach(rigidbody1 => rigidbody1.velocity = Vector3.zero);
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0, 1, _ResetTime)
                .setOnUpdate(val =>
                {
                    for (var index = 0; index < _Children.Length; index++)
                    {
                        var child = _Children[index];
                        child.transform.localPosition = Vector3.Lerp(childrenCurrentPos[index],
                            _ChildrenOriginalPositions[index], val);
                        child.transform.localScale = Vector3.Lerp(childrenCurrentScale[index],
                            _ChildrenOriginalScale[index], val);
                    }
                })
                .setEase(_ResetEasing)
                .setOnComplete(() =>
                {
                    foreach (var transform1 in _Excluded)
                    {
                        transform1.gameObject.SetActive(true);
                        callback?.Invoke();
                    }
                });
        }
    }
}
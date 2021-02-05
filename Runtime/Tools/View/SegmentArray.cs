using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GamePack
{
    public class SegmentArray: MonoBehaviour
    {
        [SerializeField, Required] private GameObject _SegmentPrefab;
        [SerializeField] private Vector3 _SegmentLength = new Vector3(0,0,5);

        [Title("Setup")]
        [SerializeField, HideInInspector] private float _Length;
        [SerializeField, ReadOnly] private List<GameObject> _Segments;
        
#if UNITY_EDITOR
        [ShowInInspector] private float Length
        {
            set
            {
                if(_SegmentLength.magnitude <= 0) return;
                
                _Length = value;
                
                UpdateSegments();
            }
            get => _Length;
        }

        private void UpdateSegments()
        {
            var segmentCount = Mathf.CeilToInt(_Length / _SegmentLength.magnitude);
            var iteration = Math.Max(segmentCount, _Segments.Count);

            for (var index = iteration - 1; index >= 0; index--)
            {
                var segment = index <= _Segments.Count - 1 ? _Segments[index] : null;

                if (segment && index > segmentCount)
                {
                    _Segments.Remove(segment);
                    DestroyImmediate(segment);
                }
                else if (!segment && index <= segmentCount)
                {
                    segment = PrefabUtility.InstantiatePrefab(_SegmentPrefab) as GameObject;
                    segment.transform.SetParent(transform);
                    _Segments.Add(segment);
                }

                if (segment)
                    // ReSharper disable once PossibleNullReferenceException
                    segment.transform.position = _SegmentLength * index;
            } 
        }
#endif

    }
}
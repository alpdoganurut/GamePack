using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Helper.Bezier
{
    [Serializable]
    public struct BezierCurveLut
    {
        private readonly List<float> _distances;
        private readonly List<float> _tValues;

        public float MaxDistance => _distances[^1];

        public BezierCurveLut(CubicBezierCurve curve, int pointCount)
        {
            var points = curve.GetCurvePoints(pointCount);
            _distances = new List<float>();
            _tValues = new List<float>();

            _distances.Add(0);
            _tValues.Add(0);
            for (var index = 1; index < points.Count; index++)
            {
                var point = points[index];
                var lastPoint = points[index - 1];
                
                _distances.Insert(index, _distances[index - 1] + (point - lastPoint).magnitude);
                // _distances[index] = _distances[index - 1] + (point - lastPoint).magnitude;
                _tValues.Insert(index, (float)index / (pointCount - 1));
                // _tValues[index] = (float)index / (pointCount - 1);
            }
        }

        [Button]
        public float GetT(float distance)
        {
            for (var index = 1; index < _distances.Count; index++)
            {
                var ptDistance = _distances[index];
                var lastDistance = _distances[index - 1];

                if (distance < ptDistance && distance >= lastDistance)
                {
                    var t = Mathf.InverseLerp(lastDistance, ptDistance, distance);
                    return Mathf.Lerp(_tValues[index - 1], _tValues[index], t);
                }
            }

            return 1;
        }
    }
}
using System.Collections.Generic;
using GamePack.CustomAttributes.Attributes;
using GamePack.DebugDrawSystem.DrawingMethods;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Helper.Bezier
{
    public class CubicBezierCurve: MonoBehaviour
    {
        private readonly struct BezierCurveLut
        {
            private readonly List<float> _distances;
            private readonly List<float> _tValues;

            public float MaxDistance => _distances[^1];

            private readonly bool _isInit;
            public bool IsNull => !_isInit;

            public BezierCurveLut(CubicBezierCurve curve, int pointCount)
            {
                var points = curve.GetRawCurvePoints(pointCount);
                _distances = new List<float>();
                _tValues = new List<float>();

                _distances.Add(0);
                _tValues.Add(0);
                for (var index = 1; index < points.Count; index++)
                {
                    var point = points[index];
                    var lastPoint = points[index - 1];
                
                    _distances.Insert(index, _distances[index - 1] + (point - lastPoint).magnitude);
                    _tValues.Insert(index, (float)index / (pointCount - 1));
                }

                _isInit = true;
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

        [SerializeField, Handle, OnValueChanged("RefreshLut")] private Vector3 _P0;
        [SerializeField, Handle, OnValueChanged("RefreshLut")] private Vector3 _P1;
        [SerializeField, Handle, OnValueChanged("RefreshLut")] private Vector3 _P2;
        [SerializeField, Handle, OnValueChanged("RefreshLut")] private Vector3 _P3;

        [FoldoutGroup("Static"), SerializeField] private List<Vector3> _CurvePoints = new();

        private BezierCurveLut _lut;

        // ReSharper disable once InconsistentNaming
        private BezierCurveLut LUT
        {
            get {
                if(_lut.IsNull)
                    _lut = new BezierCurveLut(this, 250);
                return _lut;
            }
        }

        private IEnumerable<Vector3> Points {
            get
            {
                yield return _P0;
                yield return _P1;
                yield return _P2;
                yield return _P3;
            }
        }

        public float MaxDistance => LUT.MaxDistance;

        public Vector3 GetPointByRatio(float ratio)
        {
            var dist = LUT.MaxDistance * ratio;
            return GetPointByDistance(dist);
        }
        
        public Vector3 GetPointByDistance(float distance) => GetRawPointByRatio(LUT.GetT(distance));

        public void SetKeyPoint(int index, Vector3 position)
        {
            switch (index)
            {
                case 0:
                    _P0 = position;
                    break;
                case 1:
                    _P1 = position;
                    break;
                case 2:
                    _P2 = position;
                    break;
                case 3:
                    _P3 = position;
                    break;
                default:
                    Debug.LogError("Index is invalid");
                    break;
            }

            RefreshLut();
        }

        private Vector3 GetRawPointByRatio(float ratio)
        {
            var a = Vector3.Lerp(_P0, _P1, ratio);
            var b = Vector3.Lerp(_P1, _P2, ratio);
            var c = Vector3.Lerp(_P2, _P3, ratio);

            var d = Vector3.Lerp(a, b, ratio);
            var e = Vector3.Lerp(b, c, ratio);

            var p = Vector3.Lerp(d, e, ratio);
            return p;
        }

        private List<Vector3> GetRawCurvePoints(int pointCount)
        {
            _CurvePoints.Clear();
            
            for(float ratio = 0; ratio <= 1; ratio += 1 / (float)pointCount)
            {
                var p = GetRawPointByRatio(ratio);

                _CurvePoints.Add(p);
            }
            
            _CurvePoints.Add(_P3);

            return _CurvePoints;
        }

        [Button]
        private void RefreshLut() => _lut = default;

        public static CubicBezierCurve Create(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var cubicBezierCurve = new GameObject(nameof(CubicBezierCurve)).AddComponent<CubicBezierCurve>();
            cubicBezierCurve._P0 = p0;
            cubicBezierCurve._P1 = p1;
            cubicBezierCurve._P2 = p2;
            cubicBezierCurve._P3 = p3;
            return cubicBezierCurve;
        }
        
        #region EDITOR

        private void OnDrawGizmosSelected()
        {
            foreach (var pt in GetRawCurvePoints(10))
            {
                Draw.Point(pt);
            }
            
            foreach (var curve in Points)
            {
                Draw.Point(curve, color: Color.red);
            }
        }

        [Button]
        public void TestLut()
        {
            const int duration = 5;
            
            var maxD = LUT.MaxDistance;
            var prevPoint = Vector3.zero;
            var pointCount = 100;
            for (var i = 0; i <= pointCount; i += 1)
            {
                var distance = ((float)i / (pointCount - 1)) * maxD;
                var point = GetRawPointByRatio(LUT.GetT(distance));
                Draw.Point(point, color: Color.red, duration:duration);
                
                if (i > 0)
                {
                    Draw.Arrow(prevPoint, point, color: Color.green, duration: duration);
                }
                prevPoint = point;
            }
        }

        #endregion
    }
}
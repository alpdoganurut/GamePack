using System.Collections.Generic;
using GamePack.CustomAttributes.Attributes;
using GamePack.DebugDrawSystem.DrawingMethods;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Helper.Bezier
{
    public class CubicBezierCurve: MonoBehaviour
    {
        [SerializeField, Handle] private Vector3 _p0;
        [SerializeField, Handle] private Vector3 _p1;
        [SerializeField, Handle] private Vector3 _p2;
        [SerializeField, Handle] private Vector3 _p3;
        
        [FoldoutGroup("Static"), SerializeField] private List<Vector3> _CurvePoints = new();

        public Vector3[] Points => new[] {_p0, _p1, _p2, _p3};

        public Vector3 GetPointByDistance(float distance)
        {
            var lut = CreateLut();
            return GetPoint(lut.GetT(distance));
        }
        
        public Vector3 GetPoint(float ratio)
        {
            var a = Vector3.Lerp(_p0, _p1, ratio);
            var b = Vector3.Lerp(_p1, _p2, ratio);
            var c = Vector3.Lerp(_p2, _p3, ratio);

            var d = Vector3.Lerp(a, b, ratio);
            var e = Vector3.Lerp(b, c, ratio);

            var p = Vector3.Lerp(d, e, ratio);
            return p;
        }

        public List<Vector3> GetCurvePoints(int pointCount)
        {
            _CurvePoints.Clear();
            
            for(float ratio = 0; ratio <= 1; ratio += 1 / (float)pointCount)
            {
                var p = GetPoint(ratio);

                _CurvePoints.Add(p);
            }
            
            _CurvePoints.Add(_p3);

            return _CurvePoints;
        }

        public BezierCurveLut CreateLut() => new(this, 250);

        [Button]
        public void TestLut()
        {
            const int duration = 5;
            
            var lut = CreateLut();
            var maxD = lut.MaxDistance;
            var prevPoint = Vector3.zero;
            var pointCount = 100;
            for (var i = 0; i <= pointCount; i += 1)
            {
                var distance = ((float)i / (pointCount - 1)) * maxD;
                var point = GetPoint(lut.GetT(distance));
                Draw.Point(point, color: Color.red, duration:duration);
                
                if (i > 0)
                {
                    Draw.Arrow(prevPoint, point, color: Color.green, duration: duration);
                }
                prevPoint = point;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            foreach (var pt in GetCurvePoints(10))
            {
                Draw.Point(pt);
            }
            
            foreach (var curve in Points)
            {
                Draw.Point(curve, color: Color.red);
            }
        }
    }
}
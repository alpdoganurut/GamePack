#if UNITY_EDITOR
using UnityEditor;
#endif

#if USING_SHAPES
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using GamePack.DebugDrawSystem.DrawingMethods;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.Tools.Helper
{
    [ExecuteAlways]
    public class PolyLinePath: MonoBehaviour
    {
        [FormerlySerializedAs("_PointsNew")] [SerializeField, Required] private List<PolyLinePathPoint> _Points;
        [FormerlySerializedAs("_DirectionRotation")] [SerializeField, Required] private Vector3 _LookDirectionRotation;

        [ShowInInspector, ReadOnly] private float[] _distances;
        [ShowInInspector, ReadOnly] private float _totalLength;
        
        public float TotalLength => _totalLength;
        
        private void Awake()
        {
            InitializeDistances();
        }

        private void InitializeDistances()
        {
            _distances = new float[_Points.Count - 1];

            for (var index = 0; index < _Points.Count - 1; index++)
            {
                var point = _Points[index].Position;
                var nextPoint = _Points[index + 1].Position;
                var distance = (point - nextPoint).magnitude;

                _distances[index] = distance;
            }

            _totalLength = _distances.Sum(f => f);
        }

        public Vector3 GetWorldPosAtPathPos(float distance, out Vector3 lookDirection)
        {
            if (_Points.Count <= 0)
            {
                lookDirection = Vector3.zero;
                return Vector3.zero;
            } 
            
            if (_Points.Count <= 1)
            {
                lookDirection = Vector3.zero;
                return _Points[0].Position;
            } 
            
            if (distance <= 0)
            {
                lookDirection = _Points[1].Position - _Points[0].Position;
                lookDirection = RotateLookDirection(lookDirection);
                return _Points[0].Position;
            }
            
            var total = 0f;
            for (var index = 0; index < _distances.Length; index++)
            {
                var d = _distances[index];
                var lastTotal = total;

                total += d;
                if (total > distance)
                {
                    var partialT = (distance - lastTotal) / (total - lastTotal);
                    var point = _Points[index].Position;
                    var nextPoint = _Points[index + 1].Position;
                    // direction = Vector3.Cross(nextPoint - point, Vector3.up);
                    lookDirection = nextPoint - point;
                    lookDirection = RotateLookDirection(lookDirection);
                    return Vector3.Lerp(point, nextPoint, partialT);
                }
            }
            lookDirection = (_Points[_Points.Count - 1].Position - _Points[_Points.Count - 2].Position).normalized;
            lookDirection = RotateLookDirection(lookDirection);
            return _Points[_Points.Count - 1].Position;
        }

        private Vector3 RotateLookDirection(Vector3 direction) => Quaternion.LookRotation(direction) *  Quaternion.Euler(_LookDirectionRotation) * Vector3.forward;  // This is functional but maybe not ideal (_LookDirectionRotation.z is not affecting)  

        #region Development
#if UNITY_EDITOR

        private void OnTransformChildrenChanged()
        {
            _Points.Clear();
            var childPoints = transform.GetComponentsInChildren<PolyLinePathPoint>();
            foreach (var polyLinePathPoint in childPoints)
            {
                _Points.Add(polyLinePathPoint);
            }
            
            RenamePoints();
            InitializeDistances();
        }
        
        [ShowInInspector] private float _testPos;

        [Button(ButtonSizes.Large)]
        private void AddPoint()
        {
            var np = new GameObject().AddComponent<PolyLinePathPoint>();
            np.transform.position = _Points.Count > 0 ? _Points.Last().Position : transform.position;
            np.transform.SetParent(transform);
            Selection.activeGameObject = np.gameObject;
        }

        [Button]
        private void Reverse()
        {
            _Points.Reverse();
            var dupPoints = _Points.ToArray();
            for (var index = 0; index < dupPoints.Length; index++)
            {
                var polyLinePathPoint = dupPoints[index];
                polyLinePathPoint.transform.SetSiblingIndex(index);
            }

            RenamePoints();
        }

        private void RenamePoints()
        {
            for (var index = 0; index < _Points.Count; index++)
            {
                var polyLinePathPoint = _Points[index];
                polyLinePathPoint.name = $"Point {index}";
            }
        }

        private void OnDrawGizmos()
        {
            var normalColor = new Color(0.26f, 0.49f, 1f);
            var pickedPointColor = new Color(1f, 0.36f, 0.23f);
            var selectedPathColor = new Color(0.22f, 1f, 0.29f);

            var isSelectedPath = Selection.gameObjects.Any(o => o == gameObject || o.transform.IsChildOf(transform));

            var lineColor = isSelectedPath ? selectedPathColor : normalColor;
            
            if(_Points == null) return;
            
            var hasMoreThanOnePoint = _Points.Count > 1;
            for (var index = 0; index < _Points.Count; index++)
            {
                var point = _Points[index];
                var pointPos = point.Position;
                var isSelectedPoint = Selection.gameObjects.Contains(point.gameObject);
                // Draw Line
                if(index < _Points.Count - 1 && hasMoreThanOnePoint)
                {
                    var nextPoint = _Points[index + 1].Position;
#if USING_SHAPES
                    Draw.Line(pointPos, nextPoint, lineColor);
#endif
                }
                // Draw Sphere
                var pointColor = isSelectedPoint ? pickedPointColor : normalColor;
                var firstPointColor = Colors.Aqua;
#if USING_SHAPES
                Draw.Sphere(pointPos, .1f, index == 0 ? firstPointColor : pointColor);
#endif
            }
            
            // Draw test pos
            var pos = GetWorldPosAtPathPos(_testPos, out var lookDirection);
#if USING_SHAPES
            Draw.Point(pos, color: Colors.LightYellow);
            Draw.ArrowRay(pos, lookDirection, Colors.CadetBlue);
#endif
        }

#endif
        #endregion
    }
}
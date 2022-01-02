using System.Collections.Generic;
using System.Linq;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.Tools.Helper
{
    public class PolyLinePath: MonoBehaviour
    {
        [FormerlySerializedAs("_PointsNew")] [SerializeField, Required] private List<PolyLinePathPoint> _Points;
        [SerializeField, Required] private Vector3 _DirectionRotation;

        private float[] _distances;
        private float _totalLength;
        
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

        public Vector3 GetWorldPosAtPathPos(float distance, out Vector3 direction)
        {
            if (_Points.Count <= 0)
            {
                direction = Vector3.zero;
                return Vector3.zero;
            } 
            
            if (_Points.Count <= 1)
            {
                direction = Vector3.zero;
                return _Points[0].Position;
            } 
            
            if (distance <= 0)
            {
                direction = _Points[1].Position - _Points[0].Position;
                direction = RotateDirection(direction);
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
                    direction = nextPoint - point;
                    direction = RotateDirection(direction);
                    return Vector3.Lerp(point, nextPoint, partialT);
                }
            }
            direction = _Points[_Points.Count - 1].Position - _Points[_Points.Count - 2].Position;
            direction = RotateDirection(direction);
            return _Points[_Points.Count - 1].Position;
        }

        private Vector3 RotateDirection(Vector3 direction) => Quaternion.LookRotation(direction) *  Quaternion.Euler(_DirectionRotation) * Vector3.forward ;

        #region Development
#if UNITY_EDITOR
        
        private float _testPos;

        [ShowInInspector, DisableInEditorMode]
        private float TestPosOnPath
        {
            get => _testPos;
            set
            {
                _testPos = value;
                var pos = GetWorldPosAtPathPos(_testPos, out _);
                DebugDraw.Pointer(pos, Color.red, 1, time: .1f);
            }
        }

        [Button(ButtonSizes.Large)]
        private void AddPoint()
        {
            var np = new GameObject($"Point {_Points.Count}").AddComponent<PolyLinePathPoint>();
            np.transform.position = _Points.Count > 0 ? _Points.Last().Position : transform.position;
            _Points.Add(np);
            np.transform.SetParent(transform);
        }

        [Button]
        private void Reverse() => _Points.Reverse();

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
                Gizmos.color = lineColor;
                if(index < _Points.Count - 1 && hasMoreThanOnePoint)
                {
                    var nextPoint = _Points[index + 1].Position;
                    Gizmos.DrawLine(pointPos, nextPoint);
                }
                // Draw Sphere
                Gizmos.color = isSelectedPoint ? pickedPointColor : normalColor;
                if (index == 0)
                    Gizmos.DrawCube(pointPos, Vector3.one * .2f);
                else
                    Gizmos.DrawSphere(pointPos, .2f);
            }
        }

        /*[Button]
        private void ValidatePoints()
        {
            foreach (var point in _Points)
            {
                var ptScript = point.GetComponents<PolyLinePathPoint>();
                if (!ptScript.Any())
                {
                    point.gameObject.AddComponent<PolyLinePathPoint>();
                }

                if (ptScript.Length > 1)
                {
                    for (var i = 1; i < ptScript.Length; i++)
                    {
                        var pt = ptScript[i];
                        DestroyImmediate(pt);
                    }
                }
            }

            _PointsNew = _Points.Select(transform1 => transform1.GetComponent<PolyLinePathPoint>()).ToList();
        }*/
#endif
        #endregion
    }
}
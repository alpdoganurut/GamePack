using System.Collections.Generic;
using System.Linq;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    public class PolyLinePath: MonoBehaviour
    {
        // [SerializeField, Required] private List<Transform> _Points;
        [SerializeField, Required] private List<PolyLinePathPoint> _PointsNew;
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
            _distances = new float[_PointsNew.Count - 1];

            for (var index = 0; index < _PointsNew.Count - 1; index++)
            {
                var point = _PointsNew[index].Position;
                var nextPoint = _PointsNew[index + 1].Position;
                var distance = (point - nextPoint).magnitude;

                _distances[index] = distance;
            }

            _totalLength = _distances.Sum(f => f);
        }

        public Vector3 GetWorldPosAtPathPos(float distance, out Vector3 direction)
        {
            if (_PointsNew.Count <= 0)
            {
                direction = Vector3.zero;
                return Vector3.zero;
            } 
            
            if (_PointsNew.Count <= 1)
            {
                direction = Vector3.zero;
                return _PointsNew[0].Position;
            } 
            
            if (distance <= 0)
            {
                direction = _PointsNew[1].Position - _PointsNew[0].Position;
                direction = RotateDirection(direction);
                return _PointsNew[0].Position;
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
                    var point = _PointsNew[index].Position;
                    var nextPoint = _PointsNew[index + 1].Position;
                    // direction = Vector3.Cross(nextPoint - point, Vector3.up);
                    direction = nextPoint - point;
                    direction = RotateDirection(direction);
                    return Vector3.Lerp(point, nextPoint, partialT);
                }
            }
            direction = _PointsNew[_PointsNew.Count - 1].Position - _PointsNew[_PointsNew.Count - 2].Position;
            direction = RotateDirection(direction);
            return _PointsNew[_PointsNew.Count - 1].Position;
        }

        private Vector3 RotateDirection(Vector3 direction) => Quaternion.Euler(_DirectionRotation) * direction;

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
            var np = new GameObject($"Point {_PointsNew.Count}").AddComponent<PolyLinePathPoint>();
            np.transform.position = _PointsNew.Count > 0 ? _PointsNew.Last().Position : transform.position;
            _PointsNew.Add(np);
            np.transform.SetParent(transform);
        }

        [Button]
        private void Reverse() => _PointsNew.Reverse();

        private void OnDrawGizmos()
        {
            var normalColor = new Color(0.26f, 0.49f, 1f);
            var pickedPointColor = new Color(1f, 0.36f, 0.23f);
            var selectedPathColor = new Color(0.22f, 1f, 0.29f);

            var isSelectedPath = Selection.gameObjects.Any(o => o == gameObject || o.transform.IsChildOf(transform));

            var lineColor = isSelectedPath ? selectedPathColor : normalColor;
            
            if(_PointsNew == null) return;
            
            var hasMoreThanOnePoint = _PointsNew.Count > 1;
            
            for (var index = 0; index < _PointsNew.Count; index++)
            {
                var point = _PointsNew[index];
                var pointPos = point.Position;
                var isSelectedPoint = Selection.gameObjects.Contains(point.gameObject);
                // Draw Line
                Gizmos.color = lineColor;
                if(index < _PointsNew.Count - 1 && hasMoreThanOnePoint)
                {
                    var nextPoint = _PointsNew[index + 1].Position;
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
using System.Collections.Generic;
using System.Linq;
using GamePack.UnityUtilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GamePack.Tools
{
    public class PolyLinePath: MonoBehaviour
    {
        [SerializeField, Required] private List<Transform> _Points;

        private float[] _distances;
        private float _totalDistance;
        
        public float TotalDistance => _totalDistance;

        #region Development
#if UNITY_EDITOR
        private float _testDistance;

        [ShowInInspector, DisableInEditorMode]
        private float TestDistance
        {
            get => _testDistance;
            set
            {
                _testDistance = value;
                var pos = GetWorldPosAtPathPos(_testDistance, out _);
                DebugDraw.Pointer(pos, Color.red, 1, time: .1f);
            }
        }

        [Button(ButtonSizes.Large)]
        private void AddPoint()
        {
            var np = new GameObject($"Point {_Points.Count}");
            np.transform.position = _Points.Last().position;
            _Points.Add(np.transform);
            np.transform.SetParent(transform);
        }

        [Button]
        private void Reverse()
        {
            _Points.Reverse();
        }
#endif
        #endregion
        
        private void Awake()
        {
            InitializeDistances();
        }

        private void InitializeDistances()
        {
            _distances = new float[_Points.Count - 1];

            for (var index = 0; index < _Points.Count - 1; index++)
            {
                var point = _Points[index].position;
                var nextPoint = _Points[index + 1].position;
                var distance = (point - nextPoint).magnitude;

                _distances[index] = distance;
            }

            _totalDistance = _distances.Sum(f => f);
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
                return _Points[0].position;
            } 
            
            if (distance <= 0)
            {
                direction = _Points[1].position - _Points[0].position;
                return _Points[0].position;
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
                    var point = _Points[index].position;
                    var nextPoint = _Points[index + 1].position;
                    // direction = Vector3.Cross(nextPoint - point, Vector3.up);
                    direction = nextPoint - point;
                    return Vector3.Lerp(point, nextPoint, partialT);
                }
            }
            // Debug.LogError($"Distance is out of bounds. Distance: {distance}, totalDistance: {TotalDistance}");
            direction = _Points[_Points.Count - 1].position - _Points[_Points.Count - 2].position;
            return _Points[_Points.Count - 1].position;
        }

        #region Development
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Selection.gameObjects.Any(o => o == gameObject || o.transform.IsChildOf(transform)) ? Color.red : Color.cyan;
            
            if(_Points == null || _Points.Count <= 1) return;
            
            for (var index = 0; index < _Points.Count; index++)
            {
                var point = _Points[index].transform.position;
                if(index < _Points.Count - 1)
                {
                    var nextPoint = _Points[index + 1].transform.position;
                    Gizmos.DrawLine(point, nextPoint);
                }
                Gizmos.DrawSphere(point, .2f);
            }
        } 
#endif
        #endregion
    }
}
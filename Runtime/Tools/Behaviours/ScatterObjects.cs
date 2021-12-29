// #define SCATTER_OBJECTS_MM_HAPTIC

using System;
using System.Linq;
using GamePack.Timer;
using GamePack.UnityUtilities.Vendor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrickyHands
{
    public class ScatterObjects: MonoBehaviour
    {
        private static ScatterObjects _instance;

        [SerializeField, Required] private float _Duration = 1;
        [SerializeField, Required] private float _Radius = 3;
        [SerializeField, Required] private float _MaxHeight = 5;
        [SerializeField, Required] private AnimationCurve _HeightCurve;
        [SerializeField, Required] private Vector2 _DelayRange;
        
        private void Awake()
        {
            _instance = this;
        }

        public static void Scatter<T>(T[] objectTransforms, Vector3 pos, Action<T> callback) where T: Component
        {
            if (!_instance)
            {
                Debug.LogError($"Create {nameof(ScatterObjects)} instance in scene before using! Not scattering.");
                return;
            }
            
            var positions = objectTransforms.Select(transform1 =>
            {
                var insideUnitCircle = (Vector3) Random.insideUnitCircle * _instance._Radius;
                insideUnitCircle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y) + pos;
                return insideUnitCircle;
            }).ToArray();

            for (var index = 0; index < objectTransforms.Length; index++)
            {
                var obj = objectTransforms[index];
                var startPos = obj.transform.position;
                var endPos = positions[index];
                new Operation(duration: _instance._Duration, delay: _instance._DelayRange.GetRandomValueAsRange(),
                    #if SCATTER_OBJECTS_MM_HAPTIC
                    action: () =>
                    {
                        MMVibrationManager.Haptic(HapticTypes.RigidImpact);  
                    },
                    #endif
                    updateAction: tVal =>
                    {
                        obj.transform.position = Vector3.Lerp(startPos, endPos, tVal) +
                                                 (Vector3.up * (_instance._HeightCurve.Evaluate(tVal) * _instance._MaxHeight));
                    },
                    endAction: () => callback?.Invoke(obj)).Start();
            }
        }

        #region Development
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, _Radius );
        } 
#endif
        #endregion
    }
}
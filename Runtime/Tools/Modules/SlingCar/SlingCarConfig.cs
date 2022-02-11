using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable InconsistentNaming

namespace GamePack
{
    [CreateAssetMenu(fileName = "SlingCarConfig", menuName = "SlingCarConfig", order = 0)]
    public class SlingCarConfig : ScriptableObject
    {
        public float _SideMaxAcceleration = 14;
        public float Deceleration = 20;
        [Space]
        public float _RotationLerpSpeed = 2;
        public float _SpeedToRotationLerpMultiplier = 1;
        public float _MinSpeedToRotation = 1;

        [Space] 
        public float _HardBreakMultiplier = 7;
        public float _SoftBreakMultiplier = 4;
        public float _BreakForecastTime = .15f;
        [Space] 
        [Obsolete, ReadOnly] public float _ImpulseForceMultiplier = 1;
        public float _ImpulseSleepDuration = 1f;
        public float _ControlSleepDuration = .4f;
        [Space]
        [InfoBox("This is the main curve to calculate acceleration.")] public AnimationCurve _AccelerationSpeedCurve;


        [Button]
        public float GetAccelerationForSpeed(float speed)
        {
            return _AccelerationSpeedCurve.Evaluate(speed);
        }

        #region Development
#if UNITY_EDITOR
        
        [Title("Acceleration Curve Calculation")]
        [ShowInInspector] public float MaxAcceleration
        {
            get => _MaxAcceleration;
            set
            {
                _MaxAcceleration = value;
                CalculateAccelerationSpeedCurve();
            }
        }

        [ShowInInspector] public float MaxSpeed
        {
            get => _MaxSpeed;
            set
            {
                _MaxSpeed = value;
                CalculateAccelerationSpeedCurve();
            }
        }


        [SerializeField, ReadOnly] public float _MaxAcceleration = 10;
        [SerializeField, ReadOnly] public float _MaxSpeed = 24;

        [SerializeField] private AnimationCurve _TimeSpeedCurve;
        [SerializeField] private AnimationCurve _AccelerationTimeCurve;

        [Button]
        private void CalculateAccelerationSpeedCurve(int steps = 50)
        {
            InvertTimeSpeedCurve(steps);
            
            var max = _TimeSpeedCurve.keys[_TimeSpeedCurve.keys.Length - 1].time;
            var newKeys = new Keyframe[steps + 1];

            for (var i = 0; i <= steps; i++)
            {
                var time = max * i / steps;
                var speed = _TimeSpeedCurve.Evaluate(time);
                var acc = _AccelerationTimeCurve.Evaluate(time);
                newKeys[i] = new Keyframe(speed * MaxSpeed, acc * MaxAcceleration);
            }

            _AccelerationSpeedCurve = SetKeys(newKeys);
            
            InvertTimeSpeedCurve(steps);
        }
        
        private void InvertTimeSpeedCurve(int steps = 50)
        {
            var max = _TimeSpeedCurve.keys[_TimeSpeedCurve.keys.Length - 1].time;
            var newKeys = new Keyframe[steps + 1];
            for (var i = 0; i <= steps; i++)
            {
                var time = max * i / steps;
                var val = _TimeSpeedCurve.Evaluate(time);
                newKeys[i] = new Keyframe(val, time);
            }

            _TimeSpeedCurve = SetKeys(newKeys);
        }

        private static AnimationCurve SetKeys(Keyframe[] newKeys)
        {
            var curve = new AnimationCurve {keys = newKeys};

            for (var index = 0; index < curve.keys.Length; index++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, index, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(curve, index, AnimationUtility.TangentMode.Linear);
            }

            return curve;
        } 
#endif
        #endregion
    }
}
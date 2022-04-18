using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Utilities
{
    [Serializable]
    public struct EaseCurve
    {
        
        [HideIf("HideEaseCondition")]public EasingFunction.Ease _Ease;
        [HideIf("HideCurveCondition")]public AnimationCurve _Curve;

        public bool IsCurve => _Ease == EasingFunction.Ease.None;
        
        public EaseCurve(EasingFunction.Ease ease) : this()
        {
            _Ease = ease;
            _Curve = null;
        }

        public EaseCurve(AnimationCurve curve) : this()
        {
            _Curve = curve;
            _Ease = EasingFunction.Ease.None;
        }

        public float Evaluate(float t) => 
            IsCurve ? _Curve.Evaluate(t) : EasingFunction.GetEasingFunction(_Ease)(0, 1, t);

        public static EaseCurve Linear => new(EasingFunction.Ease.Linear);
        
#if UNITY_EDITOR
        [SerializeField, Required, PropertyOrder(-1)] private bool _HasCustomCurve;
        
        private bool HideCurveCondition() => !_HasCustomCurve;

        private bool HideEaseCondition() => _HasCustomCurve;
#endif
    }
}
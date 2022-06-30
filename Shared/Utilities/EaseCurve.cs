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

        public static readonly EaseCurve Linear = new EaseCurve(EasingFunction.Ease.Linear);
        
#if UNITY_EDITOR
        [ShowInInspector, Required, PropertyOrder(-1)]
        private bool CustomCurve
        {
            get => IsCurve;
            set => _Ease = value ? EasingFunction.Ease.None : EasingFunction.Ease.Linear;
        }

        private bool HideCurveCondition() => !IsCurve;

        private bool HideEaseCondition() => IsCurve;
#endif
    }
}
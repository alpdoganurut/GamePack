using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.TweenAlphaSetActive
{
    public abstract class TweenAlphaSetActiveHandler : MonoBehaviour
    {
        [SerializeField] protected float Duration = .3f;
        [SerializeField] protected LeanTweenType Easing = LeanTweenType.easeInOutSine;
        [SerializeField] private bool _DisableOnStart;
        [ShowInInspector, ReadOnly] private bool _isActive;
        [ShowInInspector, ReadOnly] protected bool IsTransitioning;
    
        protected int CurrentTweenId = -1;

        protected Color DisabledColor
        {
            get
            {
                return new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 0);
            }
        }

        protected Color ActiveColor
        {
            get
            {
                return new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 1);
            }
        }

        protected abstract Color CurrentColor { get; }

        public bool IsActive
        {
            get { return _isActive; }
        }

        public float ThisDuration
        {
            get => Duration;
            set => Duration = value;
        }

        // Setting initial _isActive value on enable
        private void OnEnable()
        {
            _isActive = true;
        }

        private void Awake()
        {
            if (_DisableOnStart)
            {
                _isActive = false;
                gameObject.SetActive(false);
                SetIsActive(false);
            }
        }

        private void OnDisable()
        {
            _isActive = false;
        }

        public void SetIsActive(bool isActive)
        {
            /*
            if (!isAnimated)
            {
                _isActive = isActive;
                gameObject.SetActive(false);
            }
            */
        
            if (IsActive == isActive)
                return;
            _isActive = isActive;
        
            // Cancel current tween
            if (CurrentTweenId > 0)
                LeanTween.cancel(CurrentTweenId);
        
            DoTween(isActive);
        }

        protected abstract void DoTween(bool isActive);

        public void Toggle()
        {
//        if(IsTransitioning) return;
        
            SetIsActive(!IsActive);
        }
    }
}
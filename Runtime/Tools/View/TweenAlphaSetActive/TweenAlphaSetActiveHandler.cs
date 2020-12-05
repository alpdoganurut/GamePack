using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.TweenAlphaSetActive
{
    public abstract class TweenAlphaSetActiveHandler : MonoBehaviour
    {
        [SerializeField] protected float _Duration = .3f;
        [SerializeField] protected LeanTweenType _Easing = LeanTweenType.easeInOutSine;
        [SerializeField] private bool _DisableOnStart;
        [ShowInInspector, ReadOnly] protected bool IsTransitioning => CurrentTweenId > 0;
    
        protected int CurrentTweenId = -1;

        protected Color DisabledColor => new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 0);

        protected Color ActiveColor => new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 1);

        protected abstract Color CurrentColor { get; }

        [field: ShowInInspector]
        [field: ReadOnly]
        public bool IsActive { get; private set; }

        public float Duration
        {
            get => _Duration;
            set => _Duration = value;
        }

        // Setting initial _isActive value on enable
        private void OnEnable()
        {
            IsActive = true;
        }

        private void Awake()
        {
            if (_DisableOnStart)
            {
                IsActive = false;
                gameObject.SetActive(false);
                SetIsActive(false);
            }
        }

        private void OnDisable()
        {
            IsActive = false;
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
            IsActive = isActive;
        
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
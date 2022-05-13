using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Visual.ObjectFader
{
    public abstract class FaderBase : MonoBehaviour
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
        public bool IsActive { get; private set; } = true;

        public float Duration
        {
            get => _Duration;
            set => _Duration = value;
        }

        public bool DisableOnStart
        {
            get => _DisableOnStart;
            set => _DisableOnStart = value;
        }

        // Setting initial _isActive value on enable
        /*private void OnEnable()
        {
            IsActive = true;
        }*/

        private void Start()
        {
            if (_DisableOnStart)
            {
                SetIsActive(false, false);
            }
        }

        /*
        private void OnDisable()
        {
            IsActive = false;
        }
        */

        public void SetIsActive(bool isActive, bool isAnimated = true, bool ignoreTimescale = false)
        {
            if (IsActive == isActive)
                return;
            
            IsActive = isActive;
            
            if (!isAnimated)
            {
                gameObject.SetActive(isActive);
                return;
            }
        
            // Cancel current tween
            if (CurrentTweenId > 0)
                LeanTween.cancel(CurrentTweenId);
        
            DoTween(isActive, ignoreTimescale);
        }

        protected abstract void DoTween(bool isActive, bool ignoreTimescale);

        public void Toggle()
        {
            SetIsActive(!IsActive);
        }
    }
}
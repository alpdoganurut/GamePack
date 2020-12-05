using UnityEngine;

namespace GamePack.TweenAlphaSetActive
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupTweenAlphaSetActiveHandler : TweenAlphaSetActiveHandler
    {
        private CanvasGroup _canvasGroup;

        private CanvasGroup CanvasGroup
        {
            get
            {
                if (!_canvasGroup)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        protected override Color CurrentColor => new Color();

        protected override void DoTween(bool isActive)
        {
            if (isActive) gameObject.SetActive(true);

            if (!IsTransitioning)
            {
                CanvasGroup.alpha = isActive ? 0 : 1;
            }
            
            CurrentTweenId = LeanTween.alphaCanvas( CanvasGroup, isActive ? 1 : 0, Duration)
                .setEase(_Easing)
                .setOnComplete(() =>
                {
                    CurrentTweenId = -1;
                    if(!isActive) gameObject.SetActive(false);
                })
                .uniqueId;
        }
    }
}
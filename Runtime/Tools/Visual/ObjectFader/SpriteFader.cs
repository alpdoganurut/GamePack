using UnityEngine;

namespace GamePack.Visual.ObjectFader
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFader : FaderBase
    {
        private SpriteRenderer _renderer;

        public SpriteRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = GetComponent<SpriteRenderer>();
                }
                return _renderer;
            }
        }


        protected override Color CurrentColor
        {
            get { return Renderer.color; }
        }


        protected override void DoTween(bool isActive, bool ignoreTimescale)
        {
        
            CurrentTweenId = LeanTween.color(gameObject, isActive ? ActiveColor : DisabledColor, _Duration)
                .setEase(_Easing)
                .setOnComplete(() => { CurrentTweenId = -1; })
                .setIgnoreTimeScale(ignoreTimescale)
                .uniqueId;
        }
    }
}
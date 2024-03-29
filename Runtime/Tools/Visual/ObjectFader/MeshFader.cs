using UnityEngine;

namespace GamePack.Visual.ObjectFader
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshFader : FaderBase
    {
    
        private MeshRenderer _renderer;
    
        public MeshRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = GetComponent<MeshRenderer>();
                }
                return _renderer;
            }
        }
    
        protected override Color CurrentColor
        {
            get
            {
                return Renderer.material.color;
            }
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
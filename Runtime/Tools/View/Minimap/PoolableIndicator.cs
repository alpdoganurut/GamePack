using GamePack.Poolable;
using UnityEngine;

namespace GamePack.Minimap
{
    [RequireComponent(typeof(RectTransform))] 
    public class PoolableIndicator: PoolableGameObject
    {
        
        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

    }
}
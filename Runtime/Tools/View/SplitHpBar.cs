using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace GamePack
{
    public class SplitHpBar:MonoBehaviour
    {
        [SerializeField, Required] private Canvas _Canvas;
        [SerializeField, Required] private RectTransform _Container;
        [SerializeField, Required] private List<RectTransform> _BarParts;
        [SerializeField, Required] private float _Gap = .2f;
        [SerializeField, Required] private float _Margin = .2f;
        
        private RectTransform KeyBarPart => _BarParts[0];

        [Button]
        private void Initialize(int count)
        {
            for (var index = 0; index < _BarParts.Count; index++)
            {
                var rectTransform = _BarParts[index];
                if (index > 0) DestroyImmediate(rectTransform.gameObject);
            }

            _BarParts = new List<RectTransform>{_BarParts[0]};

            // Reset
            // _Container.pivot = Vector2.zero;
            // _Container.anchorMax = Vector2.zero;
            // _Container.anchorMin = Vector2.zero;
            
            KeyBarPart.pivot = Vector2.zero;
            KeyBarPart.anchorMax = Vector2.zero;
            KeyBarPart.anchorMin = Vector2.zero;
            
            // var containerWidth = _Container.sizeDelta.x;
            // var containerHeight = _Container.sizeDelta.y;
            var containerSize = GetRtSize(_Container, _Canvas);
            var containerWidth = containerSize.x;
            var containerHeight = containerSize.y;

            var partWidth = containerWidth - (_Margin * 2);
            var partHeight = (containerHeight - ((count - 1) * _Gap) - (2 * _Margin)) / count;

            for (var i = 0; i < count; i++)
            {
                var part = i == 0 ? KeyBarPart : Instantiate(KeyBarPart);
                part.SetParent(_Container);
                
                if(i > 0)
                    _BarParts.Add(part);

                var totalVerticalGap = i * _Gap;
                var yPosition = _Margin + totalVerticalGap + (i * partHeight);
                
                part.sizeDelta = new Vector2(partWidth, partHeight);
                part.anchoredPosition = new Vector2(_Margin, yPosition);
            }


        }
        
        
        public Vector2 GetRtSize(RectTransform trans, Canvas canvas)
        {
            var v = new Vector3[4];
            trans.GetWorldCorners(v);
            //method one
            //return new Vector2(v[3].x - v[0].x, v[1].y - v[0].y);

            //method two
            return RectTransformUtility.PixelAdjustRect(trans, canvas).size;
        }

    }
}
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.Tools.Helper
{
    [ExecuteInEditMode]
    public class MakeRelative : MonoBehaviour
    {
        [FormerlySerializedAs("manualRefresh")]
        public bool _ManualRefresh = true;

        [FormerlySerializedAs("anchorRect")] public Rect _AnchorRect;
        [FormerlySerializedAs("anchorVector")] public Vector2 _AnchorVector;
        private Rect _anchorRectOld;
        private Vector2 _anchorVectorOld;
        private RectTransform _ownRectTransform;
        private RectTransform _parentRectTransform;
        private Vector2 _pivotOld;
        private Vector2 _offsetMinOld;
        private Vector2 _offsetMaxOld;

        private void Update()
        {
#if UNITY_EDITOR
            _ownRectTransform = gameObject.GetComponent<RectTransform>();
            _parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
            if (_ownRectTransform.offsetMin != _offsetMinOld || _ownRectTransform.offsetMax != _offsetMaxOld)
            {
                CalculateCurrentWh();
                CalculateCurrentXy();
            }

            if (_ownRectTransform.pivot != _pivotOld || _AnchorVector != _anchorVectorOld)
            {
                CalculateCurrentXy();
                _pivotOld = _ownRectTransform.pivot;
                _anchorVectorOld = _AnchorVector;
            }

            if (_AnchorRect != _anchorRectOld)
            {
                AnchorsToCorners();
                _anchorRectOld = _AnchorRect;
            }

            if (_ManualRefresh)
            {
                _ManualRefresh = false;
                CalculateCurrentWh();
                CalculateCurrentXy();
                AnchorsToCorners();
            }

            DestroyImmediate(this);
#endif
        }

        public void StopDrag()
        {
            CalculateCurrentWh();
            CalculateCurrentXy();
            AnchorsToCorners();
        }

        private void CalculateCurrentXy()
        {
            var pivot = _ownRectTransform.pivot;
            var pivotX = _AnchorRect.width * pivot.x;
            var pivotY = _AnchorRect.height * (1 - pivot.y);
            var rect = _parentRectTransform.rect;
            var newXy = new Vector2(
                _ownRectTransform.anchorMin.x * rect.width + _ownRectTransform.offsetMin.x + pivotX -
                rect.width * _AnchorVector.x,
                -(1 - _ownRectTransform.anchorMax.y) * rect.height + _ownRectTransform.offsetMax.y - pivotY +
                rect.height * (1 - _AnchorVector.y));
            _AnchorRect.x = newXy.x;
            _AnchorRect.y = newXy.y;
            _anchorRectOld = _AnchorRect;
        }

        private void CalculateCurrentWh()
        {
            var rect = _ownRectTransform.rect;
            _AnchorRect.width = rect.width;
            _AnchorRect.height = rect.height;
            _anchorRectOld = _AnchorRect;
        }

        private void AnchorsToCorners()
        {
            var pivot = _ownRectTransform.pivot;
            var pivotX = _AnchorRect.width * pivot.x;
            var pivotY = _AnchorRect.height * (1 - pivot.y);
            _ownRectTransform.anchorMin = new Vector2(0f, 1f);
            _ownRectTransform.anchorMax = new Vector2(0f, 1f);
            var localScale = _ownRectTransform.localScale;
            _ownRectTransform.offsetMin = new Vector2(_AnchorRect.x / localScale.x,
                _AnchorRect.y / localScale.y - _AnchorRect.height);
            _ownRectTransform.offsetMax = new Vector2(_AnchorRect.x / localScale.x + _AnchorRect.width,
                _AnchorRect.y / localScale.y);
            var rect = _parentRectTransform.rect;
            _ownRectTransform.anchorMin = new Vector2(
                _ownRectTransform.anchorMin.x + _AnchorVector.x +
                (_ownRectTransform.offsetMin.x - pivotX) / rect.width * localScale.x,
                _ownRectTransform.anchorMin.y - (1 - _AnchorVector.y) +
                (_ownRectTransform.offsetMin.y + pivotY) / rect.height * localScale.y);
            _ownRectTransform.anchorMax = new Vector2(
                _ownRectTransform.anchorMax.x + _AnchorVector.x +
                (_ownRectTransform.offsetMax.x - pivotX) / rect.width * localScale.x,
                _ownRectTransform.anchorMax.y - (1 - _AnchorVector.y) +
                (_ownRectTransform.offsetMax.y + pivotY) / rect.height * localScale.y);
            var pivot1 = _ownRectTransform.pivot;
            _ownRectTransform.offsetMin = new Vector2((0 - pivot1.x) * _AnchorRect.width * (1 - localScale.x),
                (0 - pivot1.y) * _AnchorRect.height * (1 - localScale.y));
            var pivot2 = _ownRectTransform.pivot;
            _ownRectTransform.offsetMax = new Vector2((1 - pivot2.x) * _AnchorRect.width * (1 - localScale.x),
                (1 - pivot2.y) * _AnchorRect.height * (1 - localScale.y));

            _offsetMinOld = _ownRectTransform.offsetMin;
            _offsetMaxOld = _ownRectTransform.offsetMax;
        }
    }

//X and Y set the position of the Pivot relative to the parent Rect
//Anchor X and Y set where on the parent Rect the Pivot is relative to
//Where (0, 0) is the bottom left corner of parent Rect and (1, 1) the top right
}
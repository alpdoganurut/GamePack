using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.TweenAlphaSetActive
{
    public class DistanceTweenAlphaActiveSetter<T> : MonoBehaviour where T : TweenAlphaSetActiveHandler
    {
        [SerializeField] private bool _isShowWhenFar = true;
        [DisableInPlayMode, SerializeField] private float _distanceThreshold = 200;

        private T _component;
        private T Component => _component ? _component : (_component = GetComponent<T>());

        private float _minDistanceToShowSqr = -1f;

        private float MinDistanceToShowSqr
        {
            get
            {
                if (_minDistanceToShowSqr < 0)
                {
                    _minDistanceToShowSqr = DistanceThreshold * DistanceThreshold;
                }

                return _minDistanceToShowSqr;
            }
        }

        public bool IsShowWhenFar
        {
            get { return _isShowWhenFar; }
            set { _isShowWhenFar = value; }
        }

        public float DistanceThreshold
        {
            get { return _distanceThreshold; }
            set
            {
                _distanceThreshold = value;
                _minDistanceToShowSqr = value * value;
            }
        }

        private void Awake()
        {
            if (GetComponent<TweenAlphaSetActiveHandler>() == null)
            {
                Debug.LogError("DistanceTweenAlphaActiveSetter requires TweenAlphaSetActiveHandler component!");
            }
        }

        private void Update()
        {
            // TODO: Optimize Camera.main
            var distanceSqr = (Camera.main.transform.position - transform.position).sqrMagnitude;
            var isActive = IsShowWhenFar ? 
                distanceSqr > MinDistanceToShowSqr : 
                distanceSqr < MinDistanceToShowSqr;
            Component.SetIsActive(isActive);
        }
    }
}
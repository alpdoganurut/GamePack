using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    public class FollowObject: MonoBehaviour
    {
        [SerializeField] private GameObject _ObjectToFollow;
        [SerializeField] private Vector3 _Offset;

        [SerializeField, Required]
        private float _LerpSpeed = 5;

        private void LateUpdate()
        {
            if(_ObjectToFollow)
                transform.position = Vector3.Lerp(transform.position, _ObjectToFollow.transform.position + _Offset, _LerpSpeed * Time.deltaTime);
        }

        public void SetObjectToFollow(GameObject toFollow, Vector3? offset = null)
        {
            if (offset.HasValue) _Offset = offset.Value;
            else _Offset = Vector3.zero;
            
            _ObjectToFollow = toFollow;
        }
        
        #if UNITY_EDITOR
        [Button]
        private void GetCurrentOffset()
        {
            _Offset = _ObjectToFollow.transform.position - transform.position;
        }
        #endif
    }
}
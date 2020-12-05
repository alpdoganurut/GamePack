using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Animation
{
    public class BoneTargeter: MonoBehaviour
    {
        [SerializeField, Required] private Vector3 _Direction = Vector3.forward;
        [SerializeField, Required] private Transform _Target;
        [SerializeField, Range(0, 1)] private float _RotAmount = 1;
        [SerializeField, Required] private int _Priority;
        [SerializeField, ReadOnly] private BoneTargetersGroup _BoneTargetersGroup;
        [SerializeField] private float _LerpSpeed = 5;

        private Transform _transform;
        private Quaternion _rotation;
        [ShowInInspector] private bool _isActive;
        private Quaternion _directionRot;

        public int Priority
        {
            get => _Priority;
            set => _Priority = value;
        }

        public BoneTargetersGroup Group
        {
            get => _BoneTargetersGroup;
            
            #region Development
#if UNITY_EDITOR
            set => _BoneTargetersGroup = value; 
#endif
            #endregion
        }

        public Transform Target
        {
            get => _Target;
            set => _Target = value;
        }

        private void Awake()
        {
            _transform = transform;
            _directionRot = Quaternion.LookRotation(_Direction);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var transform1 = transform;
            Gizmos.DrawRay(transform1.position, transform1.rotation * _Direction * 5);
        }

        public void SetIsActive(bool isActive)
        {
            _isActive = isActive;
            if (isActive) _rotation = _transform.rotation;
        }

        public void OrderedUpdate()
        {
            if(Target && _isActive)
            {

                var targetDirection = _Target.position - _transform.position;
                var realRotation = _transform.rotation;

                
                // var directionToTargetRot = Quaternion.FromToRotation(_Direction, targetDirection);
                var directionToTargetRot = Quaternion.LookRotation(targetDirection) * Quaternion.Inverse(_directionRot);
                var targetRotation = Quaternion.Slerp(realRotation,
                    directionToTargetRot, _RotAmount);

                _rotation = Quaternion.Slerp(_rotation, targetRotation, _LerpSpeed * Time.deltaTime);
            }
            else
            {
                _rotation = Quaternion.Slerp(_rotation, _transform.rotation, _LerpSpeed * Time.deltaTime);
            }

            _transform.rotation = _rotation;
        }
    }
}
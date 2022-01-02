using GamePack.Timer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TrickyHands
{
    public class StaggeringForwardMovement : MonoBehaviour
    {
        [SerializeField, Required] private float _StaggerDuration = 1;
        [SerializeField, Required] private AnimationCurve _StaggerSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        private float _baseSpeed;
        private float _speed;
        private OperationTreeDescription? _currentStagger;

        public float BaseSpeed
        {
            get => _baseSpeed;
            set
            {
                _baseSpeed = value;
                _speed = _baseSpeed;
            }
        }

        private void Update()
        {
            transform.position += Vector3.forward * (_speed * Time.deltaTime);
        }

        [Button, DisableInEditorMode]
        public void Stagger()
        {
            _currentStagger?.Cancel();

            _currentStagger = new Operation("Staggering", duration: _StaggerDuration, easeCurve: _StaggerSpeedCurve,
                updateAction: (t) =>
                {
                    _speed = _baseSpeed * t;
                }, 
                endAction: () =>
                {
                    _speed = _baseSpeed;
                }).Start();
        }
    }
}

using GamePack.TimerSystem;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrickyHands
{
    public class StaggeringForwardMovement : MonoBehaviour
    {
        [SerializeField, Required] private float _StaggerDuration = 1;
        [SerializeField, Required] private AnimationCurve _StaggerSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField, Required] private bool _UseUnscaledTime;
        
        private float _baseSpeed;
        private float _speed;
        private OperationTreeDescription? _currentStagger;

        private float DeltaTime => _UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        
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
            transform.position += Vector3.forward * (_speed * DeltaTime);
        }

        [Button, DisableInEditorMode]
        public void Stagger()
        {
            _currentStagger?.Cancel();

            _currentStagger = new Operation("Staggering", duration: _StaggerDuration, ease: new EaseCurve(_StaggerSpeedCurve),
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

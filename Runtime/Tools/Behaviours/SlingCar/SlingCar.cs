using System;
using GamePack.Tools.Helper;
using GamePack.Tools.Helper.CollisionDetection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack
{
    public partial class SlingCar : MonoBehaviour
    {
        #region Development
#if UNITY_EDITOR
        /// <summary>
        /// This is initialized at Awake by checking MeshRenderer material color of the GameObject or its children. 
        /// </summary>
        private Color _debugColor;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = _debugColor;
            Gizmos.DrawSphere(new Vector3(SidePosition, 0, ForwardPosition), .33f);

        }

#endif
        #endregion
        
        private const float ClampPositionDistance = 20;

        public event Action<Vector3, Vector3, SlingCar> Collided;
        
        #region Config
        [SerializeField, Required] private SlingCarConfig _Config;

        private float Acceleration => _Config.GetAccelerationForSpeed(_speed);
        private float RotationLerpSpeed => _Config._RotationLerpSpeed;
        private float SpeedToRotationLerpMultiplier => _Config._SpeedToRotationLerpMultiplier;
        private float SideMaxAcceleration => _Config._SideMaxAcceleration;
        private float HardBreakMultiplier => _Config._HardBreakMultiplier;
        private float SoftBreakMultiplier => _Config._SoftBreakMultiplier;
        private float BreakForecastTime => _Config._BreakForecastTime;
        private float ImpulseMultiplier => _Config._ImpulseForceMultiplier;
        private float ImpulseSleepDuration => _Config._ImpulseSleepDuration;
        private float ControlSleepDuration => _Config._ControlSleepDuration;
        
        #endregion
        
        [SerializeField, Required] private SlingCarControllerBase _Controller;
        [SerializeField, Required] private Rigidbody _Rigidbody;
        [FormerlySerializedAs("_ColliderEvent")] [SerializeField, Required] private EventCollider _EventCollider;
        [Space] 
        [SerializeField] private float _TargetSpeed = 12;
        [Space]
        [SerializeField] private bool _IsClampPos = true;


        [ShowInInspector, ReadOnly] private float _sideSpeed;
        [ShowInInspector, ReadOnly] private float _sideAcceleration;
        [ShowInInspector, ReadOnly] private float _speed;

        [ShowInInspector, ReadOnly] private bool _isControlling = true;
        
        private readonly Sleep _impulseSleep = new Sleep();
        private readonly Sleep _controlSleep = new Sleep();


        [Title("Computed")]
        private float SidePosition => _Rigidbody.position.x;

        private float ForwardPosition
        {
            get => _Rigidbody.position.z;
            set
            {
                
                var position = _Rigidbody.position;
                _Rigidbody.MovePosition(new Vector3(position.x, position.y, value));
                // _Rigidbody.position = new Vector3(position.x, position.y, value);
            }
        }
        
        private Quaternion Rotation
        {
            get => _Rigidbody.rotation;
            set => _Rigidbody.MoveRotation(value);
        }

        [ShowInInspector] private float _targetSidePos;
        
        [ShowInInspector] private Vector3 _velocity;

        public Rigidbody Rigidbody => _Rigidbody;

        public float TargetSpeed
        {
            get => _TargetSpeed;
            set => _TargetSpeed = value;
        }

        public float Speed => _speed;

        public bool IsControlling
        {
            get => _isControlling;
            set
            {
                SetRigidbodyConstraints(value);
                _isControlling = value;
            }
        }

        [NonSerialized] private bool _isConstraintsFree = false;

        public float TargetSidePos
        {
            get => _targetSidePos;
            set => _targetSidePos = value;
        }

        /*public SlingCarControllerBase Controller
        {
            get => _Controller;
            set => _Controller = value;
        }*/

        public float SideSpeed
        {
            get => _sideSpeed;
            set => _sideSpeed = value;
        }

        public bool IsConstraintsFree
        {
            get => _isConstraintsFree;
            set
            {
                _isConstraintsFree = value;
                SetRigidbodyConstraints(IsControlling);
            }
        }

        private void Awake()
        {
            #region Development
#if UNITY_EDITOR
            _debugColor = _Rigidbody.GetComponentInChildren<MeshRenderer>().material.color; 
#endif
            #endregion
            
            // _neutralPos = _NeutralPosRef.position.x;
            // _activePos = _ActivePosRef.position.x;

            _EventCollider.Enter += OnCollision;

            /*
            var @delegate = _Rigidbody.gameObject.AddComponent<SlingCarDelegate>();
            @delegate.SlingCar = this;
            */

            _impulseSleep.SleepFunc += OnImpulseSleepChange;
        }

        private void FixedUpdate()
        {
            if (_isControlling)
                PositionUpdate();
            
            RotateUpdate();

            ClearHandledCollisions();

            if (_IsClampPos)
                ClampPosition();
            
        }

        private void PositionUpdate()
        {
            if(TargetSpeed == 0 && Mathf.Abs(_speed - _TargetSpeed) <= .1f) return;
            
            if (_controlSleep.IsSleep) return;
            
            var deltaTime = Time.fixedDeltaTime;
            // Get direction
            var dir = TargetSidePos - SidePosition;
            dir = Mathf.Clamp(dir, -1, 1);

            /*var maxSpeed = Mathf.Sign(_speed) * _Rigidbody.velocity.magnitude;
            if (Mathf.Abs(_speed) > Mathf.Abs(maxSpeed))
                _speed = maxSpeed;*/

            // Accelerate forward
            if (Mathf.Abs(_TargetSpeed - _speed) > 0.05f)
            {
                var acceleration = Mathf.Abs(TargetSpeed) > Mathf.Abs(_speed) ? Acceleration : _Config.Deceleration;
                _speed += Mathf.Sign(_TargetSpeed - _speed) * acceleration * deltaTime;
            }

            // Break if target and speed is opposite or close to target pos
            var forecastPos = SidePosition + (SideSpeed * BreakForecastTime);
            var shouldBreakForecast = Mathf.Abs(forecastPos - SidePosition) > Mathf.Abs(dir);
            var shouldBreakReverseSpeed = Mathf.Sign(dir * SideSpeed) < 0;
            if (shouldBreakReverseSpeed || shouldBreakForecast)
            {
                var breakMultiplier = (shouldBreakReverseSpeed ? HardBreakMultiplier : SoftBreakMultiplier);
                var breakFriction = breakMultiplier * SideSpeed * deltaTime;
                // Clamp break friction to speed
                if (Mathf.Abs(breakFriction) > Mathf.Abs(SideSpeed))
                    breakFriction = SideSpeed;

                _sideSpeed -= breakFriction;
                Debug.DrawRay(new Vector3(SidePosition, 0, ForwardPosition), Vector3.right * (-breakFriction * 10),
                    Color.green);
            }

            // Update side movement
            _sideAcceleration = dir * SideMaxAcceleration;
            _sideSpeed += _sideAcceleration * deltaTime;
            
            // Draw
            var rotatedSpeed = (Rotation * Vector3.forward).z * Mathf.Abs(_speed);
            _velocity = new Vector3(SideSpeed, 0, rotatedSpeed);
            var worldPos = new Vector3(SidePosition, 0, ForwardPosition);
            Debug.DrawRay(worldPos, _velocity.normalized * 3, Color.red);
            Debug.DrawRay(worldPos, Rotation * Vector3.forward * 3, Color.blue);
            
            // Affect rigidbody
            if (_Rigidbody.isKinematic)
            {
                _Rigidbody.position += _velocity * deltaTime;
            }
            else
            {
                _Rigidbody.velocity = _velocity;
                // _Rigidbody.MovePosition(_Rigidbody.position + _velocity * Time.fixedDeltaTime);
            }
            
            // RotateUpdate();
        }

        private void RotateUpdate()
        {
            if(_speed <= .5f) return;
            
            // Update rotation 
            var targetRotation = Quaternion.LookRotation(new Vector3(SideSpeed, 0, _speed), Vector3.up);
            Rotation = Quaternion.Slerp(Rotation, targetRotation,
                RotationLerpSpeed * Mathf.Max(_Rigidbody.velocity.magnitude, _Config._MinSpeedToRotation) *
                SpeedToRotationLerpMultiplier *
                Time.fixedDeltaTime);

            if (_Rigidbody.isKinematic)
            {
                _Rigidbody.rotation = Rotation;
            }
            else
            {
                _Rigidbody.MoveRotation(Rotation);
            }
        }

        private void OnImpulseSleepChange(bool isSleep)
        {
            SetRigidbodyConstraints(!isSleep);
        }

        private void SetRigidbodyConstraints(bool isActive)
        {
            const RigidbodyConstraints passiveConstraints = RigidbodyConstraints.FreezePositionY |
                                                            RigidbodyConstraints.FreezeRotationZ |
                                                            RigidbodyConstraints.FreezeRotationX;
            const RigidbodyConstraints activeConstraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            const RigidbodyConstraints hitConstraints = RigidbodyConstraints.None;

            var controlling = isActive && _isControlling;
            
            _Rigidbody.constraints = 
                IsConstraintsFree ?
                    hitConstraints :
                    controlling
                        ? activeConstraints
                        : passiveConstraints;

            _Rigidbody.drag = controlling ? 0 : 1;
            _Rigidbody.angularDrag = controlling ? 0 : 1;
        }

        private void OnCollision(Collision collision)
        {
            var relativeVelocity = collision.relativeVelocity;
            // var relativeVelocity = collision.rigidbody.velocity - _Rigidbody.velocity;
            if (!IsCollisionHandled(collision))
            {
                // var force = relativeVelocity * ImpulseMultiplier;
                var collisionPoint = collision.GetContact(0).point;
                
                var otherCar = collision.gameObject.GetComponent<SlingCar>();
                
                if(otherCar)
                {
                    CollideWithCar(relativeVelocity, collisionPoint, otherCar);
                    otherCar.CollideWithCar(-relativeVelocity, collisionPoint, this);
                }
                
                CollisionHandled(collision);
            }
        }

        private void ClampPosition()
        {
            if (ForwardPosition > ClampPositionDistance)
            {
                ForwardPosition -= ClampPositionDistance;
            }
            else if (ForwardPosition < 0)
            {
                ForwardPosition += ClampPositionDistance;
            }
        }

        private void CollideWithCar(Vector3 relativeVelocity, Vector3 point, SlingCar otherCar)
        {
            if(!_isControlling) return;
            if (_impulseSleep.IsSleep) return;
            
            var force = relativeVelocity * ImpulseMultiplier;

            #region Development
#if UNITY_EDITOR
            Debug.Log($" {name} ApplySideForce: {force}");
            Debug.DrawRay(_Rigidbody.position, force, _debugColor, 1); 
#endif
            #endregion
            
            _impulseSleep.SetSleep(ImpulseSleepDuration);
            _controlSleep.SetSleep(ControlSleepDuration);
            
            Collided?.Invoke(force, point, otherCar);

            _Rigidbody.velocity = _velocity;

            // _Rigidbody.AddForce(force, ForceMode.Impulse);
            // _Rigidbody.AddForceAtPosition(force, point, ForceMode.Impulse);
            
            // _sideSpeed += force.x;
            // _speed += force.z;
        }

        public void ResetCar()
        {
            _speed = 0;
            transform.rotation = Quaternion.LookRotation(TargetSpeed < 0 ? Vector3.back : Vector3.forward);
            // Rotation = Quaternion.LookRotation(TargetSpeed < 0 ? Vector3.forward : Vector3.back);
        }

        public void SleepControls(float duration)
        {
            _controlSleep.SetSleep(duration);
        }

        private void OnDestroy()
        {
            _impulseSleep.SleepFunc -= OnImpulseSleepChange;
        }

        #region Development

#if UNITY_EDITOR
        [Button]
        private void ResetPos()
        {
            ForwardPosition = 0;
        }
#endif

        #endregion
    }
    
    /*internal class SlingCarDelegate : MonoBehaviour
    {
        public SlingCar SlingCar { get; set; }
    }*/
}
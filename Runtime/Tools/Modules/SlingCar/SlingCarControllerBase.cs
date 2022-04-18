using System;
using Sirenix.OdinInspector;
using UnityEngine;
// ReSharper disable NotAccessedField.Local

namespace GamePack
{
    public abstract class SlingCarControllerBase : MonoBehaviour
    {
        [SerializeField, Required] protected SlingCar _SlingCar;
        [SerializeField, Required] protected ForwardRoad _Road;
        // [SerializeField, Required] private float _DetectionSideSpeedLimit = 0.3f;
        [SerializeField, Required] private float _FindFrontCarTime = 2f;
        [SerializeField, Required] private LayerMask _LayerMask;
        [SerializeField, ReadOnly] private float _CarLength;
        [SerializeField] private Collider _Collider;

        private bool _isActive = true;
        
        protected virtual float TargetSpeed
        {
            get => _SlingCar.TargetSpeed;
            set => _SlingCar.TargetSpeed = value;
        }

        private float FindFrontCarDistance => SlingCar.Speed * _FindFrontCarTime;
        public SlingCar SlingCar => _SlingCar;
        private Collider Collider => _Collider;
        private Vector3 Center => Collider ? _Collider.bounds.center : transform.position;

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        private void FixedUpdate()
        {
            Debug.DrawRay(Center, Vector3.forward * (FindFrontCarDistance + (_CarLength / 2)), Color.magenta);
            // if (Mathf.Abs(SlingCar.SideSpeed) > _DetectionSideSpeedLimit)
            // {
                // SlingCar.TargetSpeed = TargetSpeed;
            // }
            if (Physics.Raycast(Center, Vector3.forward,
                out var hitInfo,
                FindFrontCarDistance + (_CarLength / 2),
                _LayerMask))
            {
                var frontCar = hitInfo.transform.GetComponent<SlingCar>();
                
                if(frontCar && frontCar != _SlingCar)
                    FoundCar(frontCar);
            }
            // else
            // {
            
                SlingCar.TargetSpeed = TargetSpeed;
                
            // }
        }

        protected virtual void FoundCar(SlingCar slingCar)
        {
            
        }

        public virtual void SetRoad(ForwardRoad road)
        {
            _Road = road;
        }
    }
}
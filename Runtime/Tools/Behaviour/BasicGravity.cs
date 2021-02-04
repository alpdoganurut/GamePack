using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.GamePack
{
    public class BasicGravity : MonoBehaviour
    {
        [SerializeField] private float _Mass = 1;
        [SerializeField, Required] private Vector3 _Gravity = new Vector3(0, -9.81f, 0);

        private Vector3 _velocity;
        private Vector3 _position;

        private void Awake()
        {
            _position = transform.position;
        }

        private void FixedUpdate()
        {
            _velocity += _Gravity * Time.fixedDeltaTime;
            _position += _velocity * Time.fixedDeltaTime;
        }

        private void Update()
        {
            transform.position = _position;
        }

        public void AddForce(Vector3 force)
        {
            _velocity += force / _Mass;
        }
    }
}
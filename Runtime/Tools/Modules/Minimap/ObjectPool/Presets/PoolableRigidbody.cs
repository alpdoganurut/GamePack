using GamePack.CustomAttribute.Attributes;
using UnityEngine;

namespace GamePack.PoolingSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class PoolableRigidbody: PoolableBase
    {
        [SerializeField, AutoFillSelf] private Rigidbody _Rigidbody;
        
        internal override void OnStart()
        {
            gameObject.SetActive(true);
        }

        internal override void OnStop()
        {
            gameObject.SetActive(false);
            _Rigidbody.velocity = Vector3.zero;
            _Rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
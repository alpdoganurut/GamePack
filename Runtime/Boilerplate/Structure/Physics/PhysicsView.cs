using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    [RequireComponent(typeof(PhysicsObject))]
    public class PhysicsView: View
    {
        [SerializeField, ReadOnly] private PhysicsObject _PhysicsObject;

        private void OnValidate()
        {
            if (!_PhysicsObject)
                _PhysicsObject = GetComponent<PhysicsObject>();
        }
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Boilerplate.Structure.Physics
{
    [RequireComponent(typeof(PhysicsObject))]
    public class PhysicsView: View
    {
        [SerializeField, ReadOnly] private PhysicsObject _PhysicsObject;

        protected delegate bool LifeTimeCheck(PhysicsView view);

        protected delegate bool Destruction(PhysicsView view);

        protected virtual LifeTimeCheck LifeTimeCheckCondition => null;
        protected virtual Destruction DestructionAction => null;

        private void OnValidate()
        {
            if (!_PhysicsObject)
                _PhysicsObject = GetComponent<PhysicsObject>();
        }

        internal override void Internal_OnUpdate()
        {
            base.Internal_OnUpdate();
            if (LifeTimeCheckCondition?.Invoke(this) ?? false)
            {
                if (DestructionAction != null)
                    DestructionAction.Invoke(this);
                else
                    Destroy(Internal_GameObject);
            }
        }
    }
}
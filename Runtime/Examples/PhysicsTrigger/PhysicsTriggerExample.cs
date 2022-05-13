using GamePack.Boilerplate.Structure;
using GamePack.Boilerplate.Structure.Physics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples
{
    public class PhysicsTriggerExample: StructureMonoBehaviourBase
    {
        [SerializeField, Required] private PhysicsTrigger _SomeTrigger;

        private void Awake()
        {
            _SomeTrigger.ListenFor<MeshRenderer>(DidTriggerWith, PhysicsEventPhase.Enter);
        }

        private void DidTriggerWith(MeshRenderer meshRenderer, Collider other) => LogMethod(meshRenderer.name);
    }
}
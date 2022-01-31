using UnityEngine;

namespace Boilerplate.Structure
{
    public class View: StructureMonoBehaviourBase
    {
        public void DestroyView()
        {
            Destroy(InternalGameObject);
        }

        public void SetIsVisible(bool isVisible)
        {
            InternalGameObject.SetActive(isVisible);
        }
    }
}
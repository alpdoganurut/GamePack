using UnityEngine;

namespace Boilerplate.Structure
{
    public class View: StructureMonoBehaviour
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
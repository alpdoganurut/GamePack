using GamePack.Logging;

namespace GamePack.Boilerplate.Structure
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

        public void LogInWorld(object msg)
        {
            WorldLog.Log(msg, transform);
        }
    }
}
using GamePack.Logging;

namespace GamePack.Boilerplate.Structure
{
    public class View: StructureMonoBehaviourBase
    {
        public bool IsVisible => Internal_GameObject.activeInHierarchy;

        internal void Internal_OnLoad()
        {
            // TODO: Utilize this if necessary
        }

        internal virtual void Internal_OnUpdate()
        {
            // TODO: Utilize this if necessary
        }
        
        public void DestroyView()
        {
            Destroy(Internal_GameObject);
        }

        public void SetIsVisible(bool isVisible)
        {
            Internal_GameObject.SetActive(isVisible);
        }

#if USING_SHAPES
        public void LogInWorld(object msg)
        {
            WorldLog.Log(msg, transform);
        }
#endif
    }
}
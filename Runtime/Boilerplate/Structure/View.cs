using GamePack.Logging;
using GamePack.Timer;
using GamePack.Utilities;
using Shapes;
using UnityEngine;
using Draw = GamePack.Utilities.DebugDrawSystem.DrawingMethods.Draw;

namespace GamePack.Boilerplate.Structure
{
    public class View: StructureMonoBehaviourBase
    {
        public bool IsVisible => InternalGameObject.activeInHierarchy;
        
        internal void InternalOnLoad()
        {
            // TODO: Utilize this if necessary
        }
        
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

        public void InternalUpdate()
        {
            if(StructureManager.ShowViewAxes)
                Draw.Text(Vector3.zero, $"{this.GetScenePath()} ({GetType().Name})", color: IsVisible ? Color.white : Colors.DimGray, textAlign: TextAlign.Bottom, fontSize: .5f, localTransform: transform);
            if(StructureManager.ShowViewNames)
               Draw.Axis(Vector3.zero, transform);
        }
    }
}
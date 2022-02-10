using System;
using GamePack.Logging;
using GamePack.Utilities;
using Shapes;
using UnityEngine;
using Draw = GamePack.Utilities.DebugDrawSystem.DrawingMethods.Draw;

namespace GamePack.Boilerplate.Structure
{
    public class View: StructureMonoBehaviourBase
    {
        public bool IsVisible => InternalGameObject.activeInHierarchy;

        internal void Internal_OnLoad()
        {
            // TODO: Utilize this if necessary
        }

        public void Internal_OnUpdate()
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
    }
}
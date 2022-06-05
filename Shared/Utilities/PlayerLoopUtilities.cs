using System;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace GamePack.Utilities
{
    public class PlayerLoopUtilities
    {
        public static void AppendToPlayerLoop<TSystemToAdd>(Type type, PlayerLoopSystem.UpdateFunction action)
        {
            var defaultSystems = PlayerLoop.GetCurrentPlayerLoop();
            var customLateUpdate = new PlayerLoopSystem()
            {
                updateDelegate = action,
                type = type
            };
            AddToSystem<TSystemToAdd>(ref defaultSystems, customLateUpdate);
            PlayerLoop.SetPlayerLoop(defaultSystems);
        }
        
        private static bool AddToSystem<TSystemToAdd>(ref PlayerLoopSystem system, PlayerLoopSystem addition)
        {
            if (system.type == typeof(TSystemToAdd))
            {
                var oldList = system.subSystemList;
                var listLength = oldList == null ? 1 : oldList.Length + 1;
                var newList =  new PlayerLoopSystem[listLength];
                oldList?.CopyTo(newList, 0);
                newList[listLength - 1] = addition;
                system.subSystemList = newList;
                return true;
            }
            
            if (system.subSystemList != null)
            {
                for (var i = 0; i < system.subSystemList.Length; i++)
                {
                    if (AddToSystem<TSystemToAdd>(ref system.subSystemList[i], addition))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
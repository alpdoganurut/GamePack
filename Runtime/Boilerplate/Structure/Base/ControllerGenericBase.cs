using GamePack.Boilerplate.Main;
using GamePack.Logging;
using GamePack.Utilities;

namespace GamePack.Boilerplate.Structure
{
    public abstract class ControllerGenericBase<TLevelInitData> : ControllerBase where TLevelInitData: LevelInitDataBase
    {
        internal void InternalOnLevelStart(LevelInitDataBase levelData)
        {
            ManagedLog.Log($"{GetType().Name}.{nameof(InternalOnLevelStart)} ({this.GetScenePath()})", ManagedLog.Type.Verbose);
            OnLevelDidStart(levelData as TLevelInitData);
        }
        
        internal void InternalOnLevelStop()
        {
            ManagedLog.Log($"{GetType().Name}.{nameof(InternalOnLevelStop)} ({this.GetScenePath()})", ManagedLog.Type.Verbose);
            OnLevelDidStop();
        }

        protected virtual void OnLevelDidStart(TLevelInitData levelInitData) { }
        protected virtual void OnLevelDidStop() { }
    }
}
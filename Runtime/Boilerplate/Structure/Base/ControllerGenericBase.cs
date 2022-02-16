using GamePack.Boilerplate.Main;
using GamePack.Logging;
using GamePack.Utilities;

namespace GamePack.Boilerplate.Structure
{
    public abstract class ControllerGenericBase<TLevelInitData, TLevelHelper> : ControllerBase
        where TLevelInitData : LevelInitDataBase where TLevelHelper: LevelHelperBase

    {
    internal void InternalOnLevelStart(LevelInitDataBase levelData, TLevelHelper levelHelper)
    {
        ManagedLog.Log($"{GetType().Name}.{nameof(InternalOnLevelStart)} ({this.GetScenePath()})",
            ManagedLog.Type.Verbose);
        OnLevelDidStart(levelData as TLevelInitData, levelHelper);
    }

    internal void InternalOnLevelStop()
    {
        ManagedLog.Log($"{GetType().Name}.{nameof(InternalOnLevelStop)} ({this.GetScenePath()})",
            ManagedLog.Type.Verbose);
        OnLevelDidStop();
    }

    protected virtual void OnLevelDidStart(TLevelInitData levelInitData, TLevelHelper levelHelper)
    {
    }

    protected virtual void OnLevelDidStop()
    {
    }
    }
}
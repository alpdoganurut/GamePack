using GamePack.Boilerplate.Main;
using GamePack.Logging;
using GamePack.Utilities;

namespace GamePack.Boilerplate.Structure
{
    public abstract class ControllerGenericBase<TMainSceneRefBase, TLevelSceneRefBase> : ControllerBase
        where TMainSceneRefBase : MainSceneRefBase where TLevelSceneRefBase: LevelSceneRefBase

    {
    internal void InternalOnLevelStart(MainSceneRefBase levelData, TLevelSceneRefBase levelSceneRef)
    {
        ManagedLog.Log($"{GetType().Name}.{nameof(InternalOnLevelStart)} ({this.GetScenePath()})",
            ManagedLog.Type.Verbose);
        OnLevelDidStart(levelData as TMainSceneRefBase, levelSceneRef);
    }

    internal void InternalOnLevelStop()
    {
        ManagedLog.Log($"{GetType().Name}.{nameof(InternalOnLevelStop)} ({this.GetScenePath()})",
            ManagedLog.Type.Verbose);
        OnLevelDidStop();
    }

    protected virtual void OnLevelDidStart(TMainSceneRefBase mainSceneRef, TLevelSceneRefBase levelSceneRef)
    {
    }

    protected virtual void OnLevelDidStop()
    {
    }
    }
}
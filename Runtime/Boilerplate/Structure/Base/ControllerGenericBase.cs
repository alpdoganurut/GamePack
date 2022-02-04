using GamePack.Boilerplate.Main;

namespace GamePack.Boilerplate.Structure
{
    public abstract class ControllerGenericBase<TLevelInitData> : ControllerBase where TLevelInitData: LevelInitDataBase
    {
        internal void InternalOnLevelStart(LevelInitDataBase levelData)
        {
            OnLevelDidStart(levelData as TLevelInitData);
        }

        protected virtual void OnLevelDidStart(TLevelInitData levelInitData) { }
        protected virtual void OnLevelDidStop(TLevelInitData levelInitData) { }
    }
}
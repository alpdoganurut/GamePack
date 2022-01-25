using HexGames;

namespace Boilerplate.Structure
{
    public abstract class ControllerGenericBase<TLevelInitData> : ControllerBase where TLevelInitData: LevelInitDataBase
    {
        internal void InternalOnLevelStart(LevelInitDataBase levelData)
        {
            OnLevelStart(levelData as TLevelInitData);
        }

        protected virtual void OnLevelStart(TLevelInitData levelInitData) { }
    }
}
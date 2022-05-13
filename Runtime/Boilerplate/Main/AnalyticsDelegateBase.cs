namespace GamePack.Boilerplate.Main
{
    public abstract class AnalyticsDelegateBase
    {
        public abstract void Initialize();
        public abstract void GameDidStart(int levelIndex);
        public abstract void GameDidStop(bool isSuccess, int levelIndex);
    }
}
namespace GamePack.Boilerplate.Main
{
    public abstract class GameSessionDelegateBase<TLevelSceneRefBase, TMainSceneRefBase> where TLevelSceneRefBase: LevelSceneRefBase where TMainSceneRefBase: MainSceneRefBase
    {
        public virtual void InitiateMainScene(TMainSceneRefBase mainSceneRef){}
        
        public virtual void WillStartLevel(TMainSceneRefBase mainSceneRef){}

        public virtual void DidStartLevel(TMainSceneRefBase mainSceneRef, TLevelSceneRefBase levelSceneRef){}

        public virtual void WillStopLevel(TMainSceneRefBase mainSceneRef, TLevelSceneRefBase levelSceneRef, bool isSuccess){}

        public virtual void DidStopLevel(TMainSceneRefBase mainSceneRef, bool isSuccess){}
        public GameBase Game {get; internal set; }
    }
}
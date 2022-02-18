using GamePack.Boilerplate.Structure;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Boilerplate.Main
{
    public abstract class GameBase : StructureMonoBehaviourBase
    {
        public static Scene? LoadedScene => SceneLevelManager.LoadedScene;
        
        // ReSharper disable once UnusedMember.Global - Used by GameWindow inspector
        public abstract bool IsPlaying { get; }

        public void StartGame() => InternalStartGame();

        public void StopGame(bool isSuccess) => InternalStopGame(isSuccess);
        
        private protected abstract void InternalStartGame();

        private protected abstract void InternalStopGame(bool isSuccess);
    }
}
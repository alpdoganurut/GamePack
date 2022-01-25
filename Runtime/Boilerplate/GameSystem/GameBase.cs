using GamePack;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexGames
{
    public abstract class GameBase : MonoBehaviour
    {
        public static Scene? LoadedScene => SceneLevelManager.LoadedScene;
        
        // ReSharper disable once UnusedMember.Global - Used by GameWindow inspector
        public abstract bool IsPlaying { get; }

        public void StartGame()
        {
            OnStartGame();
        }

        private protected abstract void OnStartGame();

        public void StopGame(bool isSuccess)
        {
            OnStopGame(isSuccess);
        }

        private protected abstract void OnStopGame(bool isSuccess);
    }
}
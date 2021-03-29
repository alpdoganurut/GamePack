using UnityEngine;

namespace HexGames
{
    public abstract class GameBase : MonoBehaviour
    {
        public abstract bool IsPlaying { get; }
        public abstract void StartGame();
        public abstract void StopGame(bool isSuccess);
    }
}
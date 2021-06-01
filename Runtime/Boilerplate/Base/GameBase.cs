using UnityEngine;

namespace HexGames
{
    public abstract class GameBase : MonoBehaviour
    {
        [SerializeField, HideInInspector] public string WorkingTitle;
        public abstract bool IsPlaying { get; }
        public abstract void StartGame();
        public abstract void StopGame(bool isSuccess);
    }
}
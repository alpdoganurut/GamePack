using UnityEngine;
using UnityEngine.Serialization;

namespace HexGames
{
    public abstract class GameBase : MonoBehaviour
    {
        [FormerlySerializedAs("WorkingTitle")] [SerializeField, HideInInspector] public string Identifier;
        public abstract bool IsPlaying { get; }
        public abstract void StartGame();
        public abstract void StopGame(bool isSuccess);
    }
}
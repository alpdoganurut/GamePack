using GamePack;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace HexGames
{
    public abstract class GameBase : MonoBehaviour
    {
        public static Scene? LoadedScene => SceneLevelManager.LoadedScene;
        
        // ReSharper disable once UnusedMember.Global - Used by GameWindow inspector
        public abstract bool IsPlaying { get; } 
        
        public abstract void StartGame();
        public abstract void StopGame(bool isSuccess);
    }
}
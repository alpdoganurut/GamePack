using GamePack;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace HexGames
{
    public abstract class GameBase : MonoBehaviour
    {
        [FormerlySerializedAs("WorkingTitle")] [SerializeField, HideInInspector] public string Identifier;
        
        public static Scene? LoadedScene => SceneLevelManager.LoadedScene;
        
        public abstract bool IsPlaying { get; }
        public abstract void StartGame();
        public abstract void StopGame(bool isSuccess);
    }
}
using System.Linq;
using GamePack.Boilerplate.Main;
using GamePack.Logging;
using GamePack.TimerSystem;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Boilerplate
{
    public static class LevelSceneEnterPlayModeHandler
    {
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod() => 
            SceneManager.sceneLoaded += OnSceneLoaded;

        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) => 
            CheckLevelEnterPlayModeForLoadingMainScene();

        private static AsyncOperation _asyncOperation;
        
        private static void CheckLevelEnterPlayModeForLoadingMainScene()
        {
            if(!ProjectConfig.Instance.AutoEnterMainScene) return;
            
            // No game exists
            var game = FindAllObjects.InScene<GameBase>().FirstOrDefault();
            if (game) return;
            
            // Found level helper
            var sceneRef = FindAllObjects.InScene<LevelSceneRefBase>();
            if(sceneRef.Count <= 0) return;
            
            ManagedLog.Log($"Loading main scene to test {SceneManager.GetActiveScene().name}");

            var testingScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            GameWindow.TestLevel = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            
            _asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode(GameWindow.MainSceneAssetPath,
                new LoadSceneParameters(LoadSceneMode.Single));

            _asyncOperation.completed += OnloadComplete;
            
            void OnloadComplete(AsyncOperation obj)
            {
                game = FindAllObjects.InScene<GameBase>().FirstOrDefault();
                _asyncOperation.completed -= OnloadComplete;
                Assert.IsNotNull(game);

#if USING_SHAPES
                WorldLog.OnScreen($"Testing {testingScene.name}");
#endif
                new Operation(delay: 1f, action: () =>
                {
                    game.StartGame();
                }).Start();
            }
        }
    }
}
using System.Linq;
using GamePack.Boilerplate;
using GamePack.Boilerplate.Main;
using GamePack.Logging;
using GamePack.TimerSystem;
using GamePack.UnityUtilities;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        private static AsyncOperation _asyncOperation;

        private static void CheckLevelEnterPlayModeForLoadingMainScene()
        {
            // No game exists
            var game = FindAllObjects.InScene<GameBase>().FirstOrDefault();
            if (game) return;
            
            // Found level helper
            var levelHelper = FindAllObjects.InScene<LevelHelperBase>();
            if(levelHelper.Count <= 0) return;
            
            ManagedLog.Log($"Loading main scene to test {SceneManager.GetActiveScene().name}");

            var testingScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            TestLevel = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            
            _asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode(MainSceneAssetPath,
                new LoadSceneParameters(LoadSceneMode.Single));

            _asyncOperation.completed += OnloadComplete;
            
            void OnloadComplete(AsyncOperation obj)
            {
                game = FindAllObjects.InScene<GameBase>().FirstOrDefault();
                _asyncOperation.completed -= OnloadComplete;
                Assert.IsNotNull(game);

#if USING_SHAPES
                WorldLog.OnScreen($"Testing {testingScene}");
#endif
                new Operation(delay: 1f, action: () =>
                {
                    game.StartGame();
                }).Start();
            }
        }

    }
}
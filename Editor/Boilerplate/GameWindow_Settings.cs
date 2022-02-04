using System;
using System.IO;
using System.Linq;
using GamePack.Logging;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        private const string EnableAnalyticsDefineSymbol = "ENABLE_ANALYTICS";
        private const string LoggingDefineSymbol = "GAME_WINDOW_LOGGING";

        private const string VersionFileName = "version.txt";
        
        private const string PackagesLockJsonName = "packages-lock.json";
        private const string PackagesDirectoryName = "Packages";

        [TabGroup("Settings")]
        [ShowInInspector, PropertyOrder(GameWindow.OrderTabsBottom)] private string BoilerplateVersion
        {
            get
            {
                // Backwards compatibility
                try
                {
                    return File.ReadAllText(Application.dataPath + "/" + VersionFileName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                }

                try
                {
                    return File.ReadAllText(Application.dataPath + "/." + VersionFileName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // throw;
                }
                return null;
            }
        }

        [TabGroup("Settings")]
        [Button]
        private void UpdateGamePack()
        {
            if (!EditorUtility.DisplayDialog("Update Packages", "This will delete packages.lock. Continue?", "Update",
                "Cancel")) return;

            var directoryInfo = Directory.GetParent(Application.dataPath);
            
            if(directoryInfo == null)
            {
                Debug.LogError("Failed to find packages.lock");
                return;
            }

            var path = Path.Combine(directoryInfo.FullName, PackagesDirectoryName, PackagesLockJsonName) ;
            File.Delete(path);
            UnityEditor.PackageManager.Client.Resolve();
        }

        [TabGroup("Settings")]
        [Button]
        private void RefreshWindow()
        {
            Init();
        }
        
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool DisableReloadDomain
        {
            get =>
                EditorSettings.enterPlayModeOptionsEnabled &&
                EditorSettings.enterPlayModeOptions == EnterPlayModeOptions.DisableDomainReload;
            set
            {
                EditorSettings.enterPlayModeOptionsEnabled = value;
                EditorSettings.enterPlayModeOptions =  value ? EnterPlayModeOptions.DisableDomainReload : EnterPlayModeOptions.None;
            }
        }
        
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool Analytics
        {
            get =>
                IsDefineSymbolEnabled(EnableAnalyticsDefineSymbol);
            set => SetDefineSymbol(value, EnableAnalyticsDefineSymbol);
        }
        
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool GameWindowLogging
        {
            get =>
                IsDefineSymbolEnabled(LoggingDefineSymbol);
            set => SetDefineSymbol(value, LoggingDefineSymbol);
        }
        
        [TabGroup("Settings"), ShowInInspector, ShowIf("@_game != null")]
        private bool GameVisible
        {
            get =>
                _game && _game.hideFlags == HideFlags.None;
            set
            {
                if(value)
                {
                    _game.gameObject.hideFlags = HideFlags.None;
                    EditorApplication.RepaintHierarchyWindow();
                    EditorApplication.DirtyHierarchyWindowSorting();
                }
                else
                {
                    _game.gameObject.hideFlags = HideFlags.HideInHierarchy;
                    EditorApplication.RepaintHierarchyWindow();
                    EditorApplication.DirtyHierarchyWindowSorting();
                }

                if(!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(_scene);
            }
        }

        [ShowInInspector, TabGroup("Settings")]
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private ManagedLogConfig ManagedLogConfig
        {
            get => ManagedLog.Config;
            set
            {
            }
        }

        private static bool IsDefineSymbolEnabled(string symbol)
        {
            return PlayerSettings
                .GetScriptingDefineSymbolsForGroup(
                    BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget)).Contains(symbol);
        }

        private static void SetDefineSymbol(bool value, string symbol)
        {
            var settings =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(
                    BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

            var containsSymbol = settings.Contains(symbol);
            if (!value && containsSymbol)
            {
                var splitSettings = settings.Split(';').ToList();
                splitSettings.Remove(symbol);

                settings = splitSettings.Aggregate((s, s1) => s + ";" + s1);
            }

            if (value && !containsSymbol)
            {
                settings += ";" + symbol;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), settings);
        }
    }
}
using System;
using System.IO;
using System.Linq;
using GamePack.Logging;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace GamePack.Editor.Boilerplate
{
    public partial class GameWindow
    {
        private const string EnableAnalyticsDefineSymbol = "ENABLE_ANALYTICS";
        private const string LoggingDefineSymbol = "GAME_WINDOW_LOGGING";
        private const string TimerEngineDefineSymbol = "TIMER_ENABLE_LOG";
        private const string UsingShapesDefineSymbol = "USING_SHAPES";

        private const string VersionFileName = "version.txt";
        
        private const string PackagesLockJsonName = "packages-lock.json";
        private const string PackagesDirectoryName = "Packages";

        #region Version Strings

        [TabGroup("Settings")]
        [ShowInInspector, PropertyOrder(OrderTabsBottom + OrderTop)] 
        private string BoilerplateVersion
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
        [ShowInInspector, PropertyOrder(OrderTabsBottom + OrderTop)] 
        private string GamePackVersion
        {
            get
            {
                try
                {
                    var info = PackageInfo.FindForAssembly(typeof(GameWindow).Assembly);
                    return info.version;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return "";
            }
        }
        [PropertySpace]

        #endregion
        
        [Title("Project Settings")]
        
        [PropertyOrder(OrderTabsBottom)]
        [TabGroup("Settings"), ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private ProjectEditorConfig ProjectEditorConfig
        {
            get => ProjectEditorConfig.Instance;
            // ReSharper disable once ValueParameterNotUsed
            set{}
        }

        [Title("Development")]
        
        [PropertyOrder(OrderTabsBottom)]
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
        
        [PropertyOrder(OrderTabsBottom)]
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool Analytics
        {
            get =>
                IsDefineSymbolEnabled(EnableAnalyticsDefineSymbol);
            set => SetDefineSymbol(value, EnableAnalyticsDefineSymbol);
        }
        
        [PropertyOrder(OrderTabsBottom)]
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool DebugDraw
        {
            get =>
                IsDefineSymbolEnabled(UsingShapesDefineSymbol);
            set => SetDefineSymbol(value, UsingShapesDefineSymbol);
        }
        
        [PropertyOrder(OrderTabsBottom)]
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

        [Title("Debug")]
        
        [PropertyOrder(OrderTabsBottom)]
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool GameWindowLogging
        {
            get =>
                IsDefineSymbolEnabled(LoggingDefineSymbol);
            set => SetDefineSymbol(value, LoggingDefineSymbol);
        }
        
        [PropertyOrder(OrderTabsBottom)]
        [TabGroup("Settings"), ShowInInspector, HideInPlayMode]
        private bool TimerEngineLogging
        {
            get =>
                IsDefineSymbolEnabled(TimerEngineDefineSymbol);
            set => SetDefineSymbol(value, TimerEngineDefineSymbol);
        }
        
        [Title("Logging (ManagedLog)")]

        [PropertyOrder(OrderTabsBottom)]
        [ShowInInspector, TabGroup("Settings")]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private ManagedLogConfig ManagedLogConfig
        {
            get => ManagedLog.Config;
            // ReSharper disable once ValueParameterNotUsed - Need to change inner values of field
            set {}
        }
        
        // Buttons
        [PropertyOrder(OrderTabsBottom + OrderBottom)]
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

        [PropertyOrder(OrderTabsBottom + OrderBottom)]
        [TabGroup("Settings")]
        [Button]
        private void RefreshWindow()
        {
            Init();
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
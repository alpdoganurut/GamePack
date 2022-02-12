#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable once RedundantUsingDirective
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Collections.Generic;
using GamePack.Logging;
using GamePack.Utilities;



namespace GamePack.BasicStateMachineSystem
{
    public class BasicStateMachineEngine
    {
        private static readonly List<BasicStateMachineBase> StateMachines = new List<BasicStateMachineBase>();

        
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoadMethod()
        {
            InitializeOnLoadMethod();
        }
#endif
        
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.Log($"{nameof(BasicStateMachineEngine)}.{nameof(InitializeOnLoadMethod)}", ManagedLog.Type.Structure);

            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(BasicStateMachineEngine), Update);
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnEditorApplicationOnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            ManagedLog.Log($"{nameof(BasicStateMachineEngine)}.{nameof(InitializeOnEnterPlayMode)}", ManagedLog.Type.Structure);
            StateMachines.Clear();
        }
#endif

#if UNITY_EDITOR
        private static void OnEditorApplicationOnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingPlayMode) return;
            ManagedLog.Log($"{nameof(BasicStateMachineEngine)}.{nameof(OnEditorApplicationOnPlayModeStateChanged)} {PlayModeStateChange.ExitingPlayMode}", ManagedLog.Type.Structure);
            StateMachines.Clear();
        }
#endif
        
        private static void Update()
        {
            // if(!Application.isPlaying) return;
            foreach (var stateMachine in StateMachines)
            {
                stateMachine.Update();
            }
        }

        public static void AddStateMachine(BasicStateMachineBase stateMachine)
        {
            StateMachines.Add(stateMachine);
        }
    }
}
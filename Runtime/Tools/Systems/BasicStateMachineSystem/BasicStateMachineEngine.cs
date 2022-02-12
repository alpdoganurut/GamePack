using System.Collections.Generic;
using GamePack.Logging;
using GamePack.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GamePack.Tools.BasicStateMachineSystem
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
        
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.Log($"{nameof(BasicStateMachineEngine)}.{nameof(InitializeOnLoadMethod)}", ManagedLog.Type.Structure);

            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(BasicStateMachineEngine), Update);
            EditorApplication.playModeStateChanged += OnEditorApplicationOnPlayModeStateChanged;
        }

        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            ManagedLog.Log($"{nameof(BasicStateMachineEngine)}.{nameof(InitializeOnEnterPlayMode)}", ManagedLog.Type.Structure);
            StateMachines.Clear();
        }
        
        private static void OnEditorApplicationOnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingPlayMode) return;
            ManagedLog.Log($"{nameof(BasicStateMachineEngine)}.{nameof(OnEditorApplicationOnPlayModeStateChanged)} {PlayModeStateChange.ExitingPlayMode}", ManagedLog.Type.Structure);
            StateMachines.Clear();
        }
        
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
using System;
using GamePack.Logging;
using GamePack.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GamePack.Editor.Utilities
{
    public partial class EditorUtilities
    {
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure);

            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(ManagedLog), LateUpdate);
        }

        private static void LateUpdate()
        {
            TimeScaleUpdate();
        }
    }
}
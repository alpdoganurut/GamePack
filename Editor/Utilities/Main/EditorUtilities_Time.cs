using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Utilities
{
    public partial class EditorUtilities
    {
        [MenuItem("Utilities/Time/Decrease Time Scale %&Z")]
        private static void DecreaseTimeScale()
        {
            Time.timeScale /= 2;
        }
        [MenuItem("Utilities/Time/Increase Time Scale %&C")]
        private static void IncreaseTimeScale()
        {
            Time.timeScale *= 2;
        }

        [MenuItem("Utilities/Time/Reset Time Scale %&X")]
        private static void ResetTimeScale()
        {
            Time.timeScale = 1;
        }
    }
}
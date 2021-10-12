using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Utilities
{
    public partial class EditorUtilities
    {
        [MenuItem("Utilities/Time/Slow Time Down %&T")]
        private static void SlowTimeDown()
        {
            Time.timeScale /= 2;
        }

        [MenuItem("Utilities/Time/Reset Slow Time Down %&R")]
        private static void ResetSlowTimeDown()
        {
            Time.timeScale = 1;
        }
    }
}
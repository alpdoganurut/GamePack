using System.Diagnostics;
using GamePack.Utilities;
using UnityEngine;

namespace Shared.EditorDrawer.BuildReplacement
{
    public static class ShowScreenInfo
    {
        [Conditional("UNITY_EDITOR")]
        public static void Message(MessageDelegate messageEvent, GameObject boundGameObject = null)
        {
#if UNITY_EDITOR
            new ScreenInfo(messageEvent, boundGameObject);
#endif
        }
    }
}
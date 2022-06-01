using GamePack.TimerSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GamePack.Editor.Boilerplate
{
    public static class ShowCursor
    {
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if((int)arg1 == 4)  // This is the initial load mode
                CreateCursor();
        }

        private static void CreateCursor()
        {
            if (!ProjectEditorConfig.Instance.ShowCursor) return;

            var canvas = new GameObject("Cursor Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();

            canvasScaler.matchWidthOrHeight = 0;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);

            var cursor = Object.Instantiate(ProjectEditorConfig.Instance.CursorPrefab, canvas.transform, false);
            cursor.rectTransform.anchorMax = Vector2.zero;
            cursor.rectTransform.anchorMin = Vector2.zero;

            new Operation(updateAction: _ => cursor.rectTransform.anchoredPosition = Input.mousePosition / canvas.transform.localScale.x).Start();
        }
    }
}
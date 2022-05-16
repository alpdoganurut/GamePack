#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GamePack.LevelDesign
{
    [InitializeOnLoad]
    public class ChainableHierarchyDrawer
    {
        private const float LineHeight = 16;
        private const float IconSize = 10;
        private const float Margin = (LineHeight - IconSize) / 2;

        private static readonly Texture2D Icon = Resources.Load<Texture2D>("chain-icon");
        private static readonly Texture2D ParentIcon = Resources.Load<Texture2D>("chain-parent-icon");

        static ChainableHierarchyDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
        }

        private static void HierarchyWindowItemCallback(int instanceId, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (!gameObject) return;
            
            var chainable = gameObject.GetComponent<Chainable>();
            if (!chainable) return;

            var textureRect = new Rect(selectionRect.max.x - IconSize, selectionRect.y + Margin, IconSize, IconSize);
            var icon = chainable.IsParent ? ParentIcon : Icon;
            GUI.DrawTexture(textureRect, icon);
        }
    }
}
#endif
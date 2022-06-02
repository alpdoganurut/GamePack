using System;
using System.Collections.Generic;
using System.Linq;
using GamePack.Tools.Helper;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GamePack.Editor.Tools
{
    [Serializable]
    public class Replace
    {
        #region Replace

        [Button, TabGroup("Replace")]
        private void ReplaceTmpTextString(string searchFor, string replaceWith)
        {
            var textMeshProUguis = FindAllObjects.InScene<TMP_Text>();
            // Undo.RecordObjects(textMeshProUguis.Select(ugui => ugui.gameObject).ToArray(), "ReplaceTmpTextString");

            var changed = new List<TMP_Text>();
            foreach (var textMeshProUgui in textMeshProUguis.Where(ugui => ugui.text.Contains(searchFor)))
            {
                textMeshProUgui.text = textMeshProUgui.text.Replace(searchFor, replaceWith);
                changed.Add(textMeshProUgui);
                EditorUtility.SetDirty(textMeshProUgui);
            }

            Debug.Log($"Replaced {changed.Count} names!");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Selection.objects = changed.Select(ugui => ugui.gameObject).ToArray();
        }

        [Button, TabGroup("Replace")]
        private void ReplaceTmpUguiTextString(string searchFor, string replaceWith)
        {
            // var textMeshProUguis = FindAllObjects.InScene<TextMeshProUGUI>();
            var textMeshProUguis = FindAllObjects.InScene<TextMeshProUGUI>();
            Undo.RecordObjects(textMeshProUguis.Select(ugui => ugui.gameObject).ToArray(), "ReplaceTmpTextString");

            var changed = new List<TextMeshProUGUI>();
            foreach (var textMeshProUgui in textMeshProUguis.Where(ugui => ugui.text.Contains(searchFor)))
            {
                textMeshProUgui.text = textMeshProUgui.text.Replace(searchFor, replaceWith);
                changed.Add(textMeshProUgui);
            }

            Debug.Log($"Replaced {changed.Count} names!");
            Selection.objects = changed.Select(ugui => ugui.gameObject).ToArray();
        }

        [Button, TabGroup("Replace")]
        private void ReplaceNameString(string searchFor, string replaceWith)
        {
            var gameObjects = FindAllObjects.InScene<GameObject>();
            Undo.RecordObjects(gameObjects, "ReplaceNameString");

            var changed = new List<GameObject>();
            foreach (var image in gameObjects.Where(go => go.name.Contains(searchFor)))
            {
                image.name = image.name.Replace(searchFor, replaceWith);
                changed.Add(image);
            }

            Debug.Log($"Replaced {changed.Count} names!");
            Selection.objects = changed.ToArray();
        }

        [Button, TabGroup("Replace")]
        private void ReplaceImageSprite(Sprite searchFor, Sprite replaceWith)
        {
            var images = FindAllObjects.InScene<Image>();
            Undo.RecordObjects(images.ToArray(), "Replace Sprites");

            var changedImages = new List<Image>();
            foreach (var image in images.Where(spriteRenderer => spriteRenderer.sprite == searchFor))
            {
                image.sprite = replaceWith;
                Debug.Log($"Replacing {image.name} sprite");
                changedImages.Add(image);
            }

            Debug.Log($"Replaced {changedImages.Count} sprites!");
            Selection.objects = changedImages.ToArray();
        }

        [Button, TabGroup("Replace")]
        private void ReplaceImageColors(Color searchFor, Color replaceWith)
        {
            var images = FindAllObjects.InScene<Image>();
            Undo.RecordObjects(images.ToArray(), "Replace Colors");

            var changed = new List<Image>();
            foreach (var image in images.Where(image => image.color == searchFor))
            {
                image.color = replaceWith;
                Debug.Log($"Replacing {image.name} sprite");
                changed.Add(image);
            }

            Debug.Log($"Replaced {changed.Count} sprites!");
        }

        [Button, TabGroup("Replace")]
        private void ReplaceTmpTextColors(Color searchFor, Color replaceWith)
        {
            var texts = FindAllObjects.InScene<TextMeshProUGUI>();
            Undo.RecordObjects(texts.ToArray(), "Replace TMP Text Colors");

            var changed = new List<TextMeshProUGUI>();
            foreach (var text in texts.Where(text => text.color == searchFor))
            {
                text.color = replaceWith;
                Debug.Log($"Replacing {text.name} text");
                changed.Add(text);
            }

            Debug.Log($"Replaced {changed.Count} texts!");
        }

        [Button, TabGroup("Replace")]
        private void ReplaceUnityTextColors(Color searchFor, Color replaceWith)
        {
            var texts = FindAllObjects.InScene<Text>();
            Undo.RecordObjects(texts, "Replace Colors");

            var changed = new List<Text>();
            foreach (var text in texts.Where(text => text.color == searchFor))
            {
                text.color = replaceWith;
                Debug.Log($"Replacing {text.name} color");
                changed.Add(text);
            }

            Debug.Log($"Replaced {changed.Count} texts!");
        }

        #endregion

        #region Replace GameObject

        [SerializeField, Required, FoldoutGroup("Replace Selected GameObjects")] private bool _DeleteOld = true;
        [SerializeField, Required, AssetsOnly, FoldoutGroup("Replace Selected GameObjects")] private GameObject _Target;
        
        [Button, FoldoutGroup("Replace Selected GameObjects")]
        private void ReplaceSelectionWithTarget()
        {
            var selection = Selection.gameObjects;
            foreach (var gameObject in selection)
            {
                var newPrefab = (GameObject) PrefabUtility.InstantiatePrefab(_Target);
                Undo.RegisterCreatedObjectUndo(newPrefab, "Replace GameObject");
                newPrefab.transform.SetParent(gameObject.transform.parent);
                
                var info = new TransformInfo(gameObject.transform);
                info.ApplyLocal(newPrefab.transform);
                
                if(_DeleteOld) Undo.DestroyObjectImmediate(gameObject);
            }
        }

        #endregion
    }
}
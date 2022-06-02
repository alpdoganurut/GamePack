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
// ReSharper disable CoVariantArrayConversion

namespace GamePack.Editor.Tools
{
    [Serializable]
    public class Replace
    {
        #region Replace

        [Title("Replace In Scene")]
        [Button]
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

        [Button]
        private void ReplaceImageColors(Color searchFor, Color replaceWith)
        {
            var images = FindAllObjects.InScene<Image>();
            Undo.RecordObjects(images, "Replace Colors");

            var changed = new List<Image>();
            foreach (var image in images.Where(image => image.color == searchFor))
            {
                image.color = replaceWith;
                Debug.Log($"Replacing {image.name} sprite");
                changed.Add(image);
            }

            Debug.Log($"Replaced {changed.Count} sprites!");
        }

        [Button]
        private void ReplaceImageSprite(Sprite searchFor, Sprite replaceWith)
        {
            var images = FindAllObjects.InScene<Image>();
            Undo.RecordObjects(images, "Replace Sprites");

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

        [Button]
        private void ReplaceTmpTextString(string searchFor, string replaceWith)
        {
            var textMeshProUGuis = FindAllObjects.InScene<TMP_Text>();

            var changed = new List<TMP_Text>();
            foreach (var textMeshProUgui in textMeshProUGuis.Where(ugui => ugui.text.Contains(searchFor)))
            {
                textMeshProUgui.text = textMeshProUgui.text.Replace(searchFor, replaceWith);
                changed.Add(textMeshProUgui);
                EditorUtility.SetDirty(textMeshProUgui);
            }

            Debug.Log($"Replaced {changed.Count} names!");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Selection.objects = changed.Select(ugui => ugui.gameObject).ToArray();
        }

        [Button]
        private void ReplaceTmpTextColors(Color searchFor, Color replaceWith)
        {
            var texts = FindAllObjects.InScene<TextMeshProUGUI>();
            Undo.RecordObjects(texts, "Replace TMP Text Colors");

            var changed = new List<TextMeshProUGUI>();
            foreach (var text in texts.Where(text => text.color == searchFor))
            {
                text.color = replaceWith;
                Debug.Log($"Replacing {text.name} text");
                changed.Add(text);
            }

            Debug.Log($"Replaced {changed.Count} texts!");
        }

        #endregion

        #region Replace GameObject

        [Title("Replace With Target Object")]
        [SerializeField, AssetsOnly, HorizontalGroup("Selection", Order = -2), LabelWidth(50)] private GameObject _Target;
        [Title("")]
        [SerializeField, HorizontalGroup("Selection"), LabelWidth(70)] private bool _DeleteOld = true;

        [PropertySpace(SpaceAfter = 20)]
        [Button, PropertyOrder(-1)]
        private void ReplaceSelectionWithTarget()
        {
            if(!_Target)
            {
                Debug.LogError("No target specified");
                return;
            }
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
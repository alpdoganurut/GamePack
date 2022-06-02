using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GamePack.Editor.Tools
{
    [Serializable]
    public class Helper
    {
        [Button]
        private static void CenterMeshInParent()
        {
            var sel = Selection.gameObjects;
            if (sel.Length < 1)
            {
                Debug.LogError("Select one or more object to center.");
                return;
            }

            foreach (var obj in sel)
            {
                if (!obj.transform.parent)
                {
                    Debug.LogError($"{obj} has no parent.");
                    continue;
                }

                var bounds = obj.GetComponent<MeshRenderer>().bounds;
                obj.transform.localPosition = -bounds.center;
            }
        }

        [Button]
        private static void RandomYRotation()
        {
            foreach (var o in Selection.gameObjects)
            {
                o.transform.Rotate(Vector3.up, Random.Range(0, 360f), Space.World);
            }
        }
        
        [Button]
        private static void CreateWrapper()
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;
            var firstOrDefault = sel.FirstOrDefault();
            if (!firstOrDefault) return;

            var firstParent = firstOrDefault ? firstOrDefault.transform.parent : null;
            var center = Vector3.zero;
            sel.ForEach(o => { center += o.transform.position / sel.Length; });

            var wrapperGo = new GameObject("Wrapper");
            wrapperGo.transform.position = center;
            wrapperGo.transform.SetParent(firstParent);

            sel.ForEach(o => { o.transform.SetParent(wrapperGo.transform); });
        }
        
        [Button, Tooltip("Create one parent gameobject for each selected gameobject.Selected gameobject will be centered by position or by its bounds if it has a MeshRenderer component")]
        private static void CreateIndividualWrappers()
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;

            foreach (var o in sel)
            {
                var parent = o.transform.parent;
                var meshRenderer = o.GetComponent<MeshRenderer>();
                var wrapperPos = meshRenderer ? meshRenderer.bounds.center : o.transform.position;

                var wrapperGo = new GameObject($"{o.name} Wrapper");
                wrapperGo.transform.position = wrapperPos;
                
                if(parent)
                    wrapperGo.transform.SetParent(parent);
                
                o.transform.SetParent(wrapperGo.transform, true);
            }
        }

        [Button]
        private static void PlaceItems(float placementYOffset = 0, float projectionOffset = 1)
        {
            var sel = Selection.gameObjects;
            if (sel.Length <= 0) return;

            Undo.RecordObjects(sel.ToArray<Object>(), "Place Items");

            // ReSharper disable once PossibleNullReferenceException
            
            sel.ForEach(o =>
            {
                o.SetActive(false);
                
                var pos = o.transform.position;
                var origin = pos + (Vector3.up * projectionOffset);
                Debug.DrawRay(origin, Vector3.down * 100, Color.red, 5);
                if (!Physics.Raycast(origin, Vector3.down, out var hit)) return;
                // if(hit.collider.gameObject == o) return;

                Debug.DrawLine(pos, hit.point, Color.green, 5);
                o.transform.position = hit.point + new Vector3(0, placementYOffset, 0);
                
                o.SetActive(true);
            });
        }
        
        [Button]
        private static void RandomizeScale(float min = .5f, float max = 1)
        {
            var sel = Selection.gameObjects;
            Undo.RecordObjects(sel.ToArray<Object>(), "RandomizeScale");

            foreach (var gameObject in sel)
            {
                gameObject.transform.localScale *= Random.Range(min, max);
            }
        }

        
        [Button]
        private static void ArrangeItemsWithSpacing(float spacing = 1)
        {
            var sel = Selection.gameObjects;


            for (var index = 0; index < sel.Length; index++)
            {
                var obj = sel[index];
                obj.transform.position = new Vector3(index * spacing, 0, 0);
            }
        }
    }
}
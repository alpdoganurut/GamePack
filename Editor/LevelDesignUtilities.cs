using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

[CreateAssetMenu(fileName = "Editor Utilities", menuName = "Level Design", order = 0)]
public class LevelDesignUtilities : ScriptableObject
{
    /* Not working properly for rotated objects
     [MenuItem("Lvl Design/Center Mesh Renderer")]
    private static void CenterInParentMeshRenderer()
    {
        var gameObject = Selection.gameObjects[0];
        
        Undo.RecordObject(gameObject, "Lvl Design/Center Mesh Renderer");
        
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        var bounds = meshRenderer.bounds;
        var center = bounds.center;
        var localCenter = gameObject.transform.InverseTransformPoint(center);
        meshRenderer.transform.localPosition = -localCenter;
    }*/
    
    [MenuItem("Lvl Design/Create Wrapper")]
    private static void CreateWrapper()
    {
        var sel = Selection.gameObjects;
        if(sel.Length <= 0) return;
        var firstOrDefault = sel.FirstOrDefault();
        if(!firstOrDefault) return;
        
        var firstParent = firstOrDefault ? firstOrDefault.transform.parent : null;
        var center = Vector3.zero;
        sel.ForEach(o =>
        {
            center += o.transform.position / sel.Length;
        });
        
        var wrapperGo = new GameObject("Wrapper");
        wrapperGo.transform.position = center;
        wrapperGo.transform.SetParent(firstParent);

        sel.ForEach(o =>
        {
            o.transform.SetParent(wrapperGo.transform);
        });
    }
    
    [Button]
    // [MenuItem("Utilities/Place Items")]
    private static void PlaceItems(float verticalOffset)
    {
        var sel = Selection.gameObjects;
        if(sel.Length <= 0) return;

        Undo.RecordObjects(sel.ToArray<Object>(), "Place Items");
        
        // ReSharper disable once PossibleNullReferenceException
        
        // var selMeshRenderer = sel.FirstOrDefault().GetComponent<MeshRenderer>();
        // var verticalOffset = selMeshRenderer ? selMeshRenderer.bounds.min.y : 0;
        // const int verticalOffset = off;
        sel.ForEach(o =>
        {
            var origin = o.transform.position + (Vector3.up * 10);
            Debug.DrawRay(origin, Vector3.down * 100, Color.red, 5);
            if (!Physics.Raycast(origin, Vector3.down, out var hit)) return;
            
            Debug.DrawLine(origin, hit.point, Color.green, 5);
            o.transform.position = hit.point + new Vector3(0, verticalOffset, 0);
        });
    }
    
    [MenuItem("Lvl Design/Select Siblings")]
    private static void SelectSiblings()
    {
        var sel = Selection.gameObjects.ToList();
        if(sel.Count <= 0) return;

        foreach (Transform sibling in sel.FirstOrDefault().transform.parent)
        {
            sel.Add(sibling.gameObject);
        }

        Selection.objects = sel.ToArray();
    }

    [Button]
    private static void RandomizeScale(float min, float max)
    {
        var sel = Selection.gameObjects;
        Undo.RecordObjects(sel.ToArray<Object>(), "RandomizeScale");

        foreach (var gameObject in sel)
        {
            gameObject.transform.localScale *= UnityEngine.Random.Range(min, max);
        }
    }
}

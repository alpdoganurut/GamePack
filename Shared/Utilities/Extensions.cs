using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace GamePack.Utilities
{
    public static class Extensions
    {
        // Transform
        public static void SetGlobalScale (this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            var lossyScale = transform.lossyScale;
            
            if(transform.lossyScale == Vector3.zero)
            {
                transform.localScale = Vector3.zero;
                return;
            }
                
            transform.localScale = new Vector3 (globalScale.x/lossyScale.x, globalScale.y/lossyScale.y, globalScale.z/lossyScale.z);
        }
        
        // Range (Vector2)
        public static float GetRandomValueAsRange(this Vector2 v2)
        {
            return Random.Range(v2.x, v2.y);
        }
        
        // Collection
        public static void ForEach<T>(this IEnumerable<T> coll, Action<T> func)
        {
            foreach (var item in coll)
            {
                func(item);
            }
        }
        
        public static int GetIndexWithMaxValue<T>(this IEnumerable<T> coll, Func<T, float> func)
        {
            var greatestVal = float.MinValue;
            var greatestIndex = 0;
            var i = 0;
            foreach (var item in coll)
            {
                var val = func(item);
                
                if (val > greatestVal)
                {
                    greatestIndex = i;
                    greatestVal = val;
                }

                i++;
            }

            return greatestIndex;
        }
        
        public static int GetIndexWithMinValue<T>(this IEnumerable<T> coll, Func<T, float> func)
        {
            var smallestVal = float.MaxValue;
            var smallestIndex = 0;
            var i = 0;
            foreach (var item in coll)
            {
                var val = func(item);
                
                if (val < smallestVal)
                {
                    smallestIndex = i;
                    smallestVal = val;
                }

                i++;
            }

            return smallestIndex;
        }

        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        
        public static int FindFirstIndex<T>(this T[] array, Func<T, bool> condition, bool reverseSearch = false)
        {
            if(reverseSearch)
            {
                for (var index = array.Length - 1; index >= 0; index--)
                {
                    var item = array[index];
                    if (condition(item)) return index;
                }

                return -1;
            }
            
            for (var index = 0; index < array.Length; index++)
            {
                var item = array[index];
                if (condition(item)) return index;
            }

            return -1;
        }

        // Unity Object Info
        public static string GetScenePath(this UnityEngine.Object component)
        {
            return GetScenePath(component as Component);
        }
        
        public static string GetScenePath(this Component component)
        {
            return GetGameObjectPath(component.gameObject);
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            if (!obj) return "NULL";
            
            var s = "/";
            var path = s + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = s + obj.name + path;
            }
            return path;
        }
    }
}
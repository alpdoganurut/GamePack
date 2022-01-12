using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePack.UnityUtilities.Vendor
{
    public static class Extensions
    {
        // Transform
        public static void SetGlobalScale (this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3 (globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
        }
        
        // Range (Vector2)
        public static float GetRandomValueAsRange(this Vector2 v2)
        {
            return Random.Range(v2.x, v2.y);
        }
        
        // Collection
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
    }
}
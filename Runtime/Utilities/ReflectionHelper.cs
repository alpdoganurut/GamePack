using System.Reflection;
using UnityEngine;

namespace GamePack.UnityUtilities
{
    public static class ReflectionHelper
    {
        private static readonly BindingFlags BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        public static void SetFieldWithReflection(object @object, string field, object value)
        {
            SetPropOrField(@object, field, value);
            
            /* var fieldInfo = @object.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(fieldInfo);

            fieldInfo.SetValue(@object, value); */
        }

        public static bool IsValueConformsField(object obj, string field, object value)
        {
            var fieldType = obj.GetType().GetField(field, BindingFlags)?.FieldType;

            return fieldType != null && value.GetType()
                .IsSubclassOf(fieldType); 
             
        }

        public static Object GetFieldAsUnityObject(object obj, string field)
        {
            return GetPropOrField(obj, field) as Object;
            // return obj.GetType().GetField(field, BindingFlags)?.GetValue(obj) as Object;
        }

        
        private static void SetPropOrField(object obj, string field, object value)
        {
            var fieldInfo = obj.GetType().GetField(field, BindingFlags);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
                return;
            }

            var propInfo = obj.GetType().GetProperty(field, BindingFlags);
            if (propInfo != null)
            {
                propInfo.SetValue(obj, value);
            }
        }
        
        public static object GetPropOrField(object obj, string field)
        {
            var fieldInfo = obj.GetType().GetField(field, BindingFlags);
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }

            var propInfo = obj.GetType().GetProperty(field, BindingFlags);
            if (propInfo != null)
            {
                return propInfo.GetValue(obj);
            }

            return null;
        }
        
        
    }
}
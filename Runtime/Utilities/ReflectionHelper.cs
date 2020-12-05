using System.Reflection;
using UnityEngine.Assertions;

namespace GamePack.UnityUtilities.Common
{
    public static class ReflectionHelper
    {
        public static void SetFieldWithReflection(object @object, string field, object value)
        {
            var fieldInfo = @object.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(fieldInfo);

            fieldInfo.SetValue(@object, value);
        }
    }
}
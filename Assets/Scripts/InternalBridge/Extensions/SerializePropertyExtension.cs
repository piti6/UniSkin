using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UniSkin
{
    public static class SerializePropertyExtension
    {
        public static object GetObject(this SerializedProperty property)
        {
            var path = property.propertyPath.Replace(".Array.data", string.Empty);

            var value = property.serializedObject.targetObject as object;

            var elements = path.Split('.');

            foreach (var element in elements)
            {
                var arrayStringStartIndex = element.IndexOf("[");
                if (arrayStringStartIndex != -1)
                {
                    var splittedElementName = element.Split('[');
                    var elementName = splittedElementName[0];
                    var index = Convert.ToInt32(splittedElementName[1].Replace("]", string.Empty));

                    var propertyValue = GetPropertyValue(value, elementName);
                    var enumerable = propertyValue as IEnumerable<object>;

                    value = enumerable.ElementAt(index);
                }
                else
                {
                    value = GetPropertyValue(value, element);
                }
            }

            return value;
        }

        private static object GetPropertyValue(object source, string name)
        {
            var type = source.GetType();
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            return field.GetValue(source);
        }
    }
}

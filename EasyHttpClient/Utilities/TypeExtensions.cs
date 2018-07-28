using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Utilities
{
    internal static class TypeExtensions
    {
        private static readonly Type[] builtInTypes = new Type[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(char), typeof(bool), typeof(DateTime), typeof(TimeSpan), typeof(string), typeof(decimal) };

        private static readonly Type[] builtInNullableTypes = new Type[] { typeof(byte?), typeof(sbyte?), typeof(int?), typeof(uint?), typeof(short?), typeof(ushort?), typeof(long?), typeof(ulong?), typeof(float?), typeof(double?), typeof(char?), typeof(bool?), typeof(DateTime?), typeof(TimeSpan?), typeof(decimal?) };

        public static bool IsEnumerableType(this Type type)
        {
            return type.IsArray || type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);
        }
        public static bool IsBulitInType(this Type type)
        {
            return builtInTypes.Contains(type) || builtInNullableTypes.Contains(type);
        }
    }
}

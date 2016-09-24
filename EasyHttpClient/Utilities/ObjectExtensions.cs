using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Utilities
{
    internal static class ObjectExtensions
    {
        public static object ChangeType(this object obj, Type type)
        {
            return type == typeof(object) ? obj : Convert.ChangeType(obj, type);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Utilities
{
    internal static class StringExtensions
    {
        public static string Replace(this string str, string oldValue, string @newValue, StringComparison comparisonType)
        {
            @newValue = @newValue ?? string.Empty;
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(oldValue) || oldValue.Equals(@newValue, comparisonType))
            {
                return str;
            }
            int foundAt;
            while ((foundAt = str.IndexOf(oldValue, 0, comparisonType)) != -1)
            {
                str = str.Remove(foundAt, oldValue.Length).Insert(foundAt, @newValue);
            }
            return str;
        }
    }
}

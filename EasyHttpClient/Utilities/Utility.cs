using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EasyHttpClient.Utilities
{
    internal class Utility
    {
        public static IEnumerable<KeyValuePair<string, string>> ExtractUrlParameter(string name, object value, int deep)
        {
            if (deep < 0)
                return Enumerable.Empty<KeyValuePair<string, string>>();


            Func<object, string> autoEncodeParameter=(val)=>{
                return Convert.ToString(val, CultureInfo.InvariantCulture);
                //return urlEncode?HttpUtility.UrlEncode(valStr):valStr;
            };

            var kps = new List<KeyValuePair<string, string>>();

            var paramType = value.GetType();
            if (paramType.IsEnum)
            {
                kps.Add(new KeyValuePair<string, string>(name, autoEncodeParameter((int)value)));
            }
            else if (paramType.IsBulitInType())
            {
                kps.Add(new KeyValuePair<string, string>(name, autoEncodeParameter(value)));
            }
            else if (paramType.IsEnumerableType())
            {
                kps.Add(new KeyValuePair<string, string>(name, autoEncodeParameter((string.Join(",", value)))));
            }
            else
            {
                foreach (var p in paramType.GetProperties())
                {
                    var propertyValue = p.GetValue(value);
                    if (propertyValue != null)
                    {
                        kps.AddRange(ExtractUrlParameter(p.Name, propertyValue, deep--));
                    }
                }
            }
            return kps;
        }

        public static Uri BuildPath(Uri baseUri, params string[] paths)
        {
            return BuildPath(baseUri, paths as IEnumerable<string>);
        }

        public static Uri BuildPath(Uri baseUri, IEnumerable<string> paths)
        {
            if (paths != null && paths.Any())
            {
                return BuildPath(new Uri(baseUri, paths.FirstOrDefault()), paths.Skip(1));
            }
            else
            {
                return baseUri;
            }
        }
    }
}

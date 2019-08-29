using EasyHttpClient.Attributes.Parameter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Reflection;
using System.Collections;
using EasyHttpClient.Attributes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using MimeMapping;

namespace EasyHttpClient.Utilities
{
    internal class Utility
    {

        public static IEnumerable<KeyValuePair<string, string>> ExtractUrlParameter(string name, object value, int deep)
        {
            return ExtractUrlParameter(name, value, new StringFormatAttribute(""), deep);
        }

        public static IEnumerable<KeyValuePair<string, string>> ExtractUrlParameter(string name, object value, StringFormatAttribute formatter, int deep)
        {
            if (deep < 0 || value == null)
                return Enumerable.Empty<KeyValuePair<string, string>>();


            Func<object, string> autoEncodeParameter = (val) =>
            {
                return formatter.Format(val, CultureInfo.InvariantCulture);
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
                var vals = ((IEnumerable)value).Cast<object>();
                foreach (var val in vals)
                    kps.Add(new KeyValuePair<string, string>(name, autoEncodeParameter(val)));
                //kps.Add(new KeyValuePair<string, string>(name, autoEncodeParameter((string.Join(",", value)))));
            }
            else
            {
                foreach (var p in paramType.GetProperties())
                {
                    if (p.IsDefined(typeof(HttpIgnoreAttribute)))
                    {
                        continue;
                    }
                    var pName = p.Name;
                    var pNameAttr = p.GetCustomAttribute<HttpAliasAttribute>();
                    var pFormatAttr = p.GetCustomAttributes().FirstOrDefault(a => a is StringFormatAttribute) as StringFormatAttribute;

                    if (pNameAttr != null && !string.IsNullOrWhiteSpace(pNameAttr.Name))
                    {
                        pName = pNameAttr.Name;
                    }
                    var propertyValue = p.GetValue(value);
                    if (propertyValue != null)
                    {
                        if (pFormatAttr != null)
                        {
                            kps.Add(new KeyValuePair<string, string>(pName, pFormatAttr.Format(propertyValue, CultureInfo.InvariantCulture)));
                        }
                        else
                        {
                            kps.AddRange(ExtractUrlParameter(pName, propertyValue, formatter, deep - 1));
                        }

                    }
                }
            }
            return kps;
        }

        public static IEnumerable<HttpContent> ExtractMultipleFormContent(string name, object value, StringFormatAttribute formatter, int deep, string defaultContentDisposition = MultiPartType.FormData)
        {
            if (deep < 0 || value == null)
                return Enumerable.Empty<HttpContent>();

            var quotedName = "\"" + name + "\"";

            Func<object, string> autoEncodeParameter = (val) =>
            {
                return formatter.Format(val, CultureInfo.InvariantCulture);
                //return urlEncode?HttpUtility.UrlEncode(valStr):valStr;
            };

            var kps = new List<HttpContent>();

            var paramType = value.GetType();
            if (value is MultipartFileData)
            {
                var f = value as MultipartFileData;
                var content = new StreamContent(File.OpenRead(f.LocalFileName));
                if (f.Headers != null)
                {
                    foreach (var h in f.Headers)
                    {
                        content.Headers.TryAddWithoutValidation(h.Key, h.Value);
                    }
                }
                if (content.Headers.ContentDisposition == null)
                {
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);

                }
                if (string.IsNullOrWhiteSpace(content.Headers.ContentDisposition.Name))
                {
                    content.Headers.ContentDisposition.Name = quotedName;
                }


                if (string.IsNullOrWhiteSpace(content.Headers.ContentDisposition.FileName))
                {
                    content.Headers.ContentDisposition.FileName = Path.GetFileName(f.LocalFileName);
                }

                content.Headers.ContentType = new MediaTypeHeaderValue(MimeUtility.GetMimeMapping(f.LocalFileName));
                kps.Add(content);
            }
            else if (value is FileInfo)
            {
                var f = value as FileInfo;
                var content = new StreamContent(f.OpenRead());
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);
                content.Headers.ContentDisposition.FileName = f.Name;
                content.Headers.ContentDisposition.Name = quotedName;
                content.Headers.ContentType = new MediaTypeHeaderValue(MimeUtility.GetMimeMapping(f.Name));
                kps.Add(content);
            }
            else if (value is FileStream)
            {
                var s = value as FileStream;
                var content = new StreamContent(s);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);
                content.Headers.ContentDisposition.FileName = s.Name;
                content.Headers.ContentDisposition.Name = quotedName;
                content.Headers.ContentType = new MediaTypeHeaderValue(MimeUtility.GetMimeMapping(s.Name));
                kps.Add(content);
            }
            else if (value is Stream)
            {
                var s = value as Stream;
                var content = new StreamContent(s);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);
                content.Headers.ContentDisposition.Name = quotedName;
                kps.Add(content);
            }
            else if (value is IEnumerable<byte>)
            {
                var content = new ByteArrayContent((value as IEnumerable<byte>).ToArray());
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);
                content.Headers.ContentDisposition.Name = quotedName;
                kps.Add(content);
            }
            else if (paramType.IsEnum)
            {
                var content = new StringContent(Convert.ToString(autoEncodeParameter((int)value)), Encoding.UTF8);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);
                content.Headers.ContentDisposition.Name = quotedName;
                kps.Add(content);
            }
            else if (paramType.IsBulitInType())
            {
                var content = new StringContent(Convert.ToString(autoEncodeParameter(value)), Encoding.UTF8);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);
                content.Headers.ContentDisposition.Name = quotedName;
                kps.Add(content);
            }
            else if (paramType.IsEnumerableType())
            {
                var vals = ((IEnumerable)value).Cast<object>();
                foreach (var val in vals)
                {

                    var content = new StringContent(autoEncodeParameter(val), Encoding.UTF8);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue(defaultContentDisposition);
                    content.Headers.ContentDisposition.Name = quotedName;
                    kps.Add(content);

                }
                //kps.Add(new KeyValuePair<string, string>(name, autoEncodeParameter((string.Join(",", value)))));
            }
            else
            {
                foreach (var p in paramType.GetProperties())
                {
                    if (p.IsDefined(typeof(HttpIgnoreAttribute)))
                    {
                        continue;
                    }
                    var pName = p.Name;
                    var pNameAttr = p.GetCustomAttribute<HttpAliasAttribute>();
                    var pFormatAttr = p.GetCustomAttribute<StringFormatAttribute>();
                    if (pNameAttr != null && !string.IsNullOrWhiteSpace(pNameAttr.Name))
                    {
                        pName = pNameAttr.Name;
                    }
                    var propertyValue = p.GetValue(value);
                    if (propertyValue != null)
                    {
                        kps.AddRange(ExtractMultipleFormContent(pName, propertyValue, pFormatAttr ?? formatter, deep - 1, defaultContentDisposition));
                    }
                }
            }
            return kps;
        }

        public static Uri CombinePaths(Uri baseUri, params string[] paths)
        {
            return CombinePaths(baseUri, paths as IEnumerable<string>);
        }

        public static Uri CombinePaths(Uri baseUri, IEnumerable<string> paths)
        {
            if (paths != null && paths.Any())
            {
                return CombinePaths(new Uri(baseUri, paths.FirstOrDefault()), paths.Skip(1));
            }
            else
            {
                return baseUri;
            }
        }
    }
}

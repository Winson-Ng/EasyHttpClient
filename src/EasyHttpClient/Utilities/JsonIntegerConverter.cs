using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Utilities
{
    public class JsonIntegerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (
                objectType == typeof(int)
                || objectType == typeof(int?)
                || objectType == typeof(long)
                || objectType == typeof(long?)
                );
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(int))
            {
                return Convert.ToInt32(reader.Value);
            }
            else if (objectType == typeof(int?))
            {
                return reader.Value == null ? null : (int?)Convert.ToInt32(reader.Value);
            }
            else if (objectType == typeof(long))
            {
                return Convert.ToInt64(reader.Value);
            }
            else if (objectType == typeof(long?))
            {
                return reader.Value == null ? null : (long?)Convert.ToInt64(reader.Value);
            }
            else
            {
                return reader.Value;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value == null ? null : (long?)Convert.ToInt64(value));
        }
    }
}

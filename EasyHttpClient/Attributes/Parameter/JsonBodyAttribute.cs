using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyHttpClient.Utilities;

namespace EasyHttpClient.Attributes
{
    public enum JTokenType
    {
        JObject, JArray
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JsonBodyAttribute : Attribute, IParameterScopeAttribute
    {
        public JsonBodyAttribute()
            : this(null)
        {

        }
        public JsonBodyAttribute(string name)
        {
            this.Name = name;
            this.JTokenType = JTokenType.JObject;
        }

        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.Json;
            }
        }
        public string Name
        {
            get;
            set;
        }

        public JTokenType JTokenType
        {
            get;
            set;
        }
        private static readonly JsonMergeSettings JsonMergeSettings = new JsonMergeSettings()
        {
            MergeArrayHandling = MergeArrayHandling.Merge
        };

        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            switch (this.JTokenType)
            {
                case JTokenType.JArray:
                    this.ProcessAsJArray(requestBuilder, parameterInfo, parameterValue);
                    break;
                case JTokenType.JObject:
                    this.ProcessAsJObject(requestBuilder, parameterInfo, parameterValue);
                    break;
            }
        }

        void ProcessAsJObject(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var jObj = requestBuilder.JsonBody;
            var objectType = parameterValue != null ? parameterValue.GetType() : parameterInfo.ParameterType;
            var val = parameterValue;

            if (objectType.IsBulitInType() || objectType.IsEnumerableType())
            {
                val = new Dictionary<string, object>{
                {this.Name ?? parameterInfo.Name, parameterValue}
               };
            }

            if (val != null)
            {
                var jsonVal = JObject.FromObject(val, requestBuilder.JsonSerializer); ;
                if (jObj == null)
                {
                    jObj = jsonVal;
                }
                else
                {
                    if (!(jObj is JObject))
                    {
                        throw new NotSupportedException("Not support merge JObject and JArray");
                    }
                    jObj = mergeJsonObjects((JObject)jObj, jsonVal);
                }
            }
            requestBuilder.JsonBody = jObj;
        }


        void ProcessAsJArray(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var jObj = requestBuilder.JsonBody;
            var objectType = parameterValue != null ? parameterValue.GetType() : parameterInfo.ParameterType;

            if (!objectType.IsEnumerableType())
            {
                throw new NotSupportedException("JsonArrayAttribute for IEnumerable parameter only");
            }
            var val = parameterValue;

            if (val != null)
            {
                var jsonVal = JArray.FromObject(val, requestBuilder.JsonSerializer); ;
                jObj = jsonVal;
            }
            requestBuilder.JsonBody = jObj;
        }

        private static JObject mergeJsonObjects(params JObject[] jObjects)
        {
            Dictionary<string, JToken> json = new Dictionary<string, JToken>();
            foreach (var jObject in jObjects)
            {
                foreach (var property in jObject)
                {
                    string name = property.Key;
                    JToken value = property.Value;

                    json[property.Key] = property.Value;
                }
            }
            var newJObject = new JObject();
            foreach (var j in json)
            {
                newJObject.Add(j.Key, j.Value);
            }
            return newJObject;
        }
    }
}

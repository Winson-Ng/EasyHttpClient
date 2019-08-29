using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public interface IHttpResult<T> : IHttpResult
    {
        /// <summary>
        /// Gets or sets the content of a HTTP response message. (For HTTP Success only)
        /// 
        /// Returns Deserialized <TofObject> from http response body
        /// </summary>
        new T Content { get; set; }
    }

    public interface IHttpResult
    {
        /// <summary>
        /// Deserialize the error message as an Object, but please ensure the response message is a Json format. 
        /// </summary>
        /// <typeparam name="TofError"></typeparam>
        /// <returns></returns>
        /// <exception cref="Newtonsoft.Json.JsonException"></exception>
        TofError ErrorMessageAs<TofError>();

        /// <summary>
        /// Deserialize the error message as an Object, but please ensure the response message is a Json format. 
        /// </summary>
        /// <typeparam name="TofError"></typeparam>
        /// <returns></returns>
        /// <exception cref="Newtonsoft.Json.JsonException"></exception>
        TofError ErrorMessageAs<TofError>(JsonSerializerSettings jsonSerializerSettings);
        /// <summary>
        /// Deserialize the error message as an Object
        /// </summary>
        /// <typeparam name="TofError"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="Newtonsoft.Json.JsonException"></exception>
        bool TryConvertErrorMessage<TofError>(out TofError result);

        /// <summary>
        /// Deserialize the error message as an Object
        /// </summary>
        /// <typeparam name="TofError"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="Newtonsoft.Json.JsonException"></exception>
        bool TryConvertErrorMessage<TofError>(JsonSerializerSettings jsonSerializerSettings, out TofError result);

        /// Gets or sets the content of a HTTP response message. (For HTTP Success only)
        /// </summary>
        /// <remarks>
        /// Returns Deserialized Object from http response body
        /// </remarks>
        object Content { get; set; }

        /// <summary>
        ///    Gets or sets the Http response body for fail case
        /// </summary>
        string ErrorMessage { get; set; }
        /// <summary>
        /// They are:
        /// AcceptRanges, Age, ETag, Location, ProxyAuthenticate, RetryAfter, Server, Vary, WwwAuthenticate, CacheControl, Connection, ConnectionClose, Date, Pragma, Trailer, TransferEncoding, TransferEncodingChunked, Upgrade, Via, Warning
        /// </summary>
        HttpResponseHeaders Headers { get; set; }
        /// <summary>
        /// They are: 
        /// Allow, Content-Disposition, Content-Encoding, Content-Language, Content-Length, Content-Location, Content-MD5, Content-Range, Content-Type, Expires, LastModified
        /// </summary>
        HttpContentHeaders ContentHeaders { get; set; }

        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// 
        /// Returns System.Boolean.A value that indicates if the HTTP response was successful.
        /// true if System.Net.Http.HttpResponseMessage.StatusCode was in the range 200-299;
        /// otherwise false.
        /// </summary>
        bool IsSuccessStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the reason phrase which typically is sent by servers together
        ///     with the status code.
        ///
        /// Returns System.String.The reason phrase sent by the server.
        /// </summary>
        string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets or sets the status code of the HTTP response.
        /// 
        /// Returns System.Net.HttpStatusCode.The status code of the HTTP response.
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the HTTP message version.
        /// 
        /// Returns System.Version.The HTTP message version. The default is 1.1.
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        /// Will use for functions ErrorMessageAs<TofError>() and TryConvertErrorMessage<TofError>(out TofError result). 
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; set; }


        HttpRequestMessage RequestMessage { get; set; }
    }

    public class HttpResult<T> : IHttpResult<T>, IConvertible
    {
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public HttpResult()
            : this(new JsonSerializerSettings())
        {

        }

        public HttpResult(JsonSerializerSettings jsonSetting)
        {
            this.JsonSerializerSettings = jsonSetting;
        }

        public HttpResponseHeaders Headers
        {
            get;
            set;
        }

        public HttpContentHeaders ContentHeaders
        {
            get;
            set;
        }

        public bool IsSuccessStatusCode
        {
            get;
            set;
        }

        public string ReasonPhrase
        {
            get;
            set;
        }

        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        public Version Version
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        private object _content;
        public T Content
        {
            get
            {
                return (T)_content;
            }
            set
            {
                _content = value;
            }
        }

        object IHttpResult.Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        #region IConvertable
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return this.Content == null ? null : this.Content.ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (!typeof(IHttpResult).IsAssignableFrom(conversionType))
            {
                throw new NotSupportedException("Not support type conversion!");
            }
            return this as IHttpResult<T>;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        #endregion

        public TofError ErrorMessageAs<TofError>()
        {
            return this.ErrorMessageAs<TofError>(this.JsonSerializerSettings);
        }

        public TofError ErrorMessageAs<TofError>(JsonSerializerSettings _jsonSerializerSettings)
        {
            return JsonConvert.DeserializeObject<TofError>(this.ErrorMessage, _jsonSerializerSettings);
        }


        public bool TryConvertErrorMessage<TofError>(out TofError result)
        {
            return TryConvertErrorMessage<TofError>(this.JsonSerializerSettings, out result);
        }

        public bool TryConvertErrorMessage<TofError>(JsonSerializerSettings _jsonSerializerSettings, out TofError result)
        {
            result = default(TofError);

            try
            {
                result = JsonConvert.DeserializeObject<TofError>(this.ErrorMessage, _jsonSerializerSettings);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public HttpRequestMessage RequestMessage
        {
            get;
            set;
        }
    }
}

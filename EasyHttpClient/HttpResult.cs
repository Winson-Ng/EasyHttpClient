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
        new T Content { get; set; }
    }

    public interface IHttpResult
    {
        TofError ErrorMessageAs<TofError>();

        // Summary:
        //     Gets or sets the content of a HTTP response message.
        //
        // Returns:
        //     Returns System.Net.Http.HttpContent.The content of the HTTP response message.
        object Content { get; set; }
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
        //
        // Summary:
        //     Gets a value that indicates if the HTTP response was successful.
        //
        // Returns:
        //     Returns System.Boolean.A value that indicates if the HTTP response was successful.
        //     true if System.Net.Http.HttpResponseMessage.StatusCode was in the range 200-299;
        //     otherwise false.
        bool IsSuccessStatusCode { get; set; }
        //
        // Summary:
        //     Gets or sets the reason phrase which typically is sent by servers together
        //     with the status code.
        //
        // Returns:
        //     Returns System.String.The reason phrase sent by the server.
        string ReasonPhrase { get; set; }
        //
        // Summary:
        //     Gets or sets the status code of the HTTP response.
        //
        // Returns:
        //     Returns System.Net.HttpStatusCode.The status code of the HTTP response.
        HttpStatusCode StatusCode { get; set; }
        //
        // Summary:
        //     Gets or sets the HTTP message version.
        //
        // Returns:
        //     Returns System.Version.The HTTP message version. The default is 1.1.
        Version Version { get; set; }
    }

    internal class HttpResult<T> : IHttpResult<T>, IConvertible
    {
        private JsonSerializerSettings _jsonSetting;

        public HttpResult(JsonSerializerSettings jsonSetting)
        {
            this._jsonSetting = jsonSetting;
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
            return this.Content.ToString();
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
            return this.ErrorMessageAs<TofError>(this._jsonSetting);
        }
        public TofError ErrorMessageAs<TofError>(JsonSerializerSettings _jsonSerializerSettings)
        {
            return JsonConvert.DeserializeObject<TofError>(this.ErrorMessage, _jsonSerializerSettings);
        }
    }
}

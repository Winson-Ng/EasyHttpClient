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
        object Content { get; set; }
        string ErrorMessage { get; set; }
        HttpResponseHeaders Headers { get; set; }
        bool IsSuccessStatusCode { get; set; }
        string ReasonPhrase { get; set; }
        HttpStatusCode StatusCode { get; set; }
        Version Version { get; set; }
    }

    internal class HttpResult<T> : IHttpResult<T>, IConvertible
    {
        public HttpResponseHeaders Headers
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
    }
}

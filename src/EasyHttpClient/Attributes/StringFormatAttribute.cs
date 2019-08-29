using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
    public class StringFormatAttribute : Attribute
    {
        private string format;

        private bool isSimpleFormat;

        /// <summary>
        /// Format a value to string
        /// </summary>
        /// <param name="format">
        /// "yyyy-MM-dd", "#,###.00" e.g.
        /// </param>
        /// <example>
        /// [StringFormat("yyyy-MM-dd")]
        /// </example>
        public StringFormatAttribute(string format)
            : this(format, true)
        {

        }
        /// <summary>
        /// Format a value to string
        /// </summary>
        /// <param name="format">
        /// "yyyy-MM-dd", "#,###.00" e.g.
        /// </param>
        /// <param name="isSimpleFormat">
        /// Default true, When isSimpleFormat=false, the "{0:yyyy-MM-dd}", "{0:#,###.00}" e.g.
        /// </param>
        /// <example>
        /// [StringFormat("Today is {0:yyyy-MM-dd}", false)]
        /// </example>
        public StringFormatAttribute(string format, bool isSimpleFormat)
        {
            this.format = string.IsNullOrWhiteSpace(format) ? "{0}"
                : (isSimpleFormat ? ("{0:" + format + "}") : format);
        }

        public virtual string Format(object arg, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, this.format, arg);
        }
    }
}

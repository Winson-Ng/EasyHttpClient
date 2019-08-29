using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public class EmptyMethod : MethodBase {
        internal static readonly object[] emptyArray = new object[0];
        internal static readonly ParameterInfo[] emptyParameterInfo = new ParameterInfo[0];
        internal EmptyMethod(){
        }
        public override MethodAttributes Attributes
        {
            get { return MethodAttributes.PrivateScope; }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.Managed;
        }

        public override ParameterInfo[] GetParameters()
        {
            return emptyParameterInfo;
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { return new RuntimeMethodHandle(); }
        }

        public override Type DeclaringType
        {
            get { return typeof(object); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return emptyArray;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return emptyArray;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        public override MemberTypes MemberType
        {
            get { return MemberTypes.Method; }
        }

        public override string Name
        {
            get { return typeof(EmptyMethod).Name; }
        }

        public override Type ReflectedType
        {
            get { return typeof(object); }
        }
    }
}

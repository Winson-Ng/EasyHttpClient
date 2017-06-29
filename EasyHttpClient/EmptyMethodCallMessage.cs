using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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

    public class EmptyMethodCallMessage : IMethodCallMessage
    {
        private static readonly EmptyMethod emptyMethod=new EmptyMethod();
        //private static readonly LogicalCallContext emptyLogicalCallContext = new LogicalCallContext();
        private static readonly IDictionary emptyDictionary = new Dictionary<object, object>();
        internal EmptyMethodCallMessage() {
        }

        public object GetInArg(int argNum)
        {
            return null;
        }

        public string GetInArgName(int index)
        {
            return null;
        }

        public int InArgCount
        {
            get { return 0; }
        }

        public object[] InArgs
        {
            get { return EmptyMethod.emptyArray; }
        }

        public int ArgCount
        {
            get { return 0; }
        }

        public object[] Args
        {
            get { return EmptyMethod.emptyArray; }
        }

        public object GetArg(int argNum)
        {
            return null;
        }

        public string GetArgName(int index)
        {
            return null;
        }

        public bool HasVarArgs
        {
            get { return false; }
        }

        public LogicalCallContext LogicalCallContext
        {
            get { return null; }
        }

        public System.Reflection.MethodBase MethodBase
        {
            get { return emptyMethod; }
        }

        public string MethodName
        {
            get { return emptyMethod.Name; }
        }

        public object MethodSignature
        {
            get { return typeof(void); }
        }

        public string TypeName
        {
            get { return typeof(EmptyMethodCallMessage).Name; }
        }

        public string Uri
        {
            get { return typeof(EmptyMethodCallMessage).FullName; }
        }

        public System.Collections.IDictionary Properties
        {
            get { return emptyDictionary; }
        }
    }
}

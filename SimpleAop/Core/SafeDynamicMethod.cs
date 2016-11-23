using System;
using System.Collections;
using System.Reflection;
using AopIntercept.Interface;

namespace AopIntercept.Core
{
    public class SafeDynamicMethod : IDynamicMethod
    {
        public delegate object FunctionDelegate(object target, params object[] args);

        private static readonly Hashtable stateCache = new IdentityTable();
        private readonly MethodInfo methodInfo;
        private readonly SafeMethodState state;

        public SafeDynamicMethod(MethodInfo methodInfo)
        {
            state = (SafeMethodState)stateCache[methodInfo];
            if (state == null)
            {
                state = new SafeMethodState(DynamicReflectionManager.CreateMethod(methodInfo),
                    new object[methodInfo.GetParameters().Length]);
                stateCache[methodInfo] = state;
            }
            this.methodInfo = methodInfo;
        }

        public Type DeclaringType
        {
            get { return methodInfo.DeclaringType; }
        }

        public object Invoke(object target, params object[] arguments)
        {
            var nullArguments = state.nullArguments;
            if (arguments == null && nullArguments.Length == 1) arguments = nullArguments;
            var arglen = (arguments == null ? 0 : arguments.Length);
            if (nullArguments.Length != arglen)
            {
                throw new ArgumentException(
                    string.Format("Invalid number of arguments passed into method {0} - expected {1}, but was {2}",
                        methodInfo.Name, nullArguments.Length, arglen));
            }

            return state.method(target, arguments);
        }

        private class IdentityTable : Hashtable
        {
            protected override int GetHash(object key)
            {
                return key.GetHashCode();
            }

            protected override bool KeyEquals(object item, object key)
            {
                return ReferenceEquals(item, key);
            }
        }

        private class SafeMethodState
        {
            public readonly FunctionDelegate method;
            public readonly object[] nullArguments;

            public SafeMethodState(FunctionDelegate method, object[] nullArguments)
            {
                this.method = method;
                this.nullArguments = nullArguments;
            }
        }
    }
}
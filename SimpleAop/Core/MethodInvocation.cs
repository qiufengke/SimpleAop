using System;
using System.Reflection;
using AopIntercept.Interface;

namespace AopIntercept.Core
{
    [Serializable]
    public class MethodInvocation : IMethodInvocation
    {
        public MethodInvocation(MethodInfo methodInfo, object target, object[] args)
        {
            MethodInfo = methodInfo;
            Target = target;
            Arguments = args;
        }

        public object[] Arguments { get; }
        public MethodInfo MethodInfo { get; }
        public object Target { get; }

        public object Proceed()
        {
            IDynamicMethod targetMethod = new SafeDynamicMethod(MethodInfo);
            return targetMethod.Invoke(Target, Arguments);
        }
    }
}
using System.Reflection;
using SimpleAop.Interface;

namespace SimpleAop.Core
{
    public class MethodInvocation : IMethodInvocation
    {
        protected object[] arguments;
        protected MethodInfo methodInfo;
        protected object target;

        public MethodInvocation(MethodInfo _methodInfo, object _target, object[] _args)
        {
            methodInfo = _methodInfo;
            target = _target;
            arguments = _args;
        }

        public object Proceed()
        {
            return methodInfo.Invoke(target, arguments);
        }
    }
}
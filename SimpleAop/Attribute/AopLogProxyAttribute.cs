using System;
using System.Runtime.Remoting.Proxies;
using SimpleAop.Interface;
using SimpleAop.Proxy;

namespace SimpleAop.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AopLogProxyAttribute : ProxyAttribute
    {
        private readonly bool _enableAfterInterception; // 是否启用方法执行后拦截
        private readonly bool _enablePreInterception; // 是否启用方法执行前拦截
        private IInterception _interception;

        public AopLogProxyAttribute(Type interceptionType, bool enablePreInterception = false,
            bool enableAfterInterception = false)
        {
            Interception = interceptionType;
            _enablePreInterception = enablePreInterception;
            _enableAfterInterception = enableAfterInterception;
        }

        public Type Interception
        {
            get { return _interception.GetType(); }
            set
            {
                var interception = Activator.CreateInstance(value) as IInterception;
                _interception = interception;
            }
        }

        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            var target = base.CreateInstance(serverType);
            var aopRealProxy = new LogProxy(serverType, target);
            aopRealProxy.InjectInterception(_interception, _enablePreInterception, _enableAfterInterception);
            return aopRealProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
using System;
using System.Runtime.Remoting.Proxies;
using AopIntercept.Interface;
using AopIntercept.Proxy;

namespace AopIntercept.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    [Serializable]
    public class AopProxyAttribute : ProxyAttribute
    {
        private readonly bool _enableAfterInterception; // 是否启用方法执行后拦截
        private readonly bool _enableArroundInterception;
        private readonly bool _enablePreInterception; // 是否启用方法执行前拦截
        private IInterception _interception;

        /// <summary>
        /// 构造拦截器类型
        /// </summary>
        /// <param name="interceptionType">拦截器类型</param>
        /// <param name="enablePreInterception">是否启用方法执行后拦截</param>
        /// <param name="enableAfterInterception">是否启用方法执行后拦截</param>
        /// <param name="enableAroundInterception">是否启用 around 拦截</param>
        public AopProxyAttribute(Type interceptionType, bool enablePreInterception = false,
            bool enableAfterInterception = false, bool enableAroundInterception = false)
        {
            Interception = interceptionType;
            _enablePreInterception = enablePreInterception;
            _enableAfterInterception = enableAfterInterception;
            _enableArroundInterception = enableAroundInterception;
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
            var aopRealProxy = new AopProxy(serverType, target);
            aopRealProxy.InjectInterception(_interception, _enablePreInterception, _enableAfterInterception,
                _enableArroundInterception);
            return aopRealProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
using System;
using AopIntercept.Interface;

namespace AopIntercept.Proxy
{
    public class ProxyFactory
    {
        public bool EnableAfterInterception = false;
        public bool EnablePreInterception = false;
        public bool EnableAroundInterception = false;

        public ProxyFactory(bool enableAfter = false, bool enablePre = false, bool enableAround = false)
        {
            EnablePreInterception = enablePre;
            EnableAroundInterception = enableAround;
            EnableAfterInterception = enableAfter;
        }

        public T CreateProxyInstance<T>(IInterception interception) where T : class
        {
            var serverType = typeof(T);
            var target = Activator.CreateInstance(serverType) as MarshalByRefObject;
            var aopRealProxy = new AopProxy(serverType, target);
            aopRealProxy.InjectInterception(interception, EnableAfterInterception, EnablePreInterception, EnableAroundInterception);
            return (T)aopRealProxy.GetTransparentProxy();
        }
    }
}
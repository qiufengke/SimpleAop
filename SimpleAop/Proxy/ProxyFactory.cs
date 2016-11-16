using System;
using SimpleAop.Interface;

namespace SimpleAop.Proxy
{
    public class ProxyFactory
    {
        public static bool EnableAfterInterception = false;
        public static bool EnablePreInterception = false;

        public static T CreateProxyInstance<T>(IInterception interception) where T : new()
        {
            var serverType = typeof(T);
            var target = Activator.CreateInstance(serverType) as MarshalByRefObject;
            var aopRealProxy = new LogProxy(serverType, target);
            aopRealProxy.InjectInterception(interception, EnableAfterInterception, EnablePreInterception);
            return (T)aopRealProxy.GetTransparentProxy();
        }
    }
}
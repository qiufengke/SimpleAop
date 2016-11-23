using System;
using AopIntercept.Interface;

namespace AopIntercept.Proxy
{
    public class ProxyFactory
    {
        public static bool EnableAfterInterception = false;
        public static bool EnablePreInterception = false;
        public static bool EnableAroundInterception = false;

        public static T CreateProxyInstance<T>(IInterception interception) where T : new()
        {
            var serverType = typeof(T);
            var target = Activator.CreateInstance(serverType) as MarshalByRefObject;
            var aopRealProxy = new AopProxy(serverType, target);
            aopRealProxy.InjectInterception(interception, EnableAfterInterception, EnablePreInterception, EnableAroundInterception);
            return (T)aopRealProxy.GetTransparentProxy();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAop.Interface;

namespace SimpleAop.Proxy
{
    public class ProxyFactory
    {
        public static T CreateProxyInstance<T>(IInterception interception) where T : new()
        {
            Type serverType = typeof(T);
            MarshalByRefObject target = Activator.CreateInstance(serverType) as MarshalByRefObject;
            LogProxy aopRealProxy = new LogProxy(serverType, target);
            aopRealProxy.InjectInterception(interception);
            return (T)aopRealProxy.GetTransparentProxy();
        }
    }
}

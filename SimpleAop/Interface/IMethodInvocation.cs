using System.Reflection;

namespace SimpleAop.Interface
{
    public interface IMethodInvocation
    {
        object[] Arguments { get; }
        MethodInfo MethodInfo { get; }
        object Target { get; }
        object Proceed();
    }
}
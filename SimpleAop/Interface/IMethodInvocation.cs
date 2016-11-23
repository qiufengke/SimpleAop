using System.Reflection;

namespace AopIntercept.Interface
{
    public interface IMethodInvocation
    {
        object[] Arguments { get; }
        MethodInfo MethodInfo { get; }
        object Target { get; }
        object Proceed();
    }
}
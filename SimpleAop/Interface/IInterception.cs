using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAop.Interface
{
    public interface IInterception
    {
        /// <summary>
        /// execute before the method invoke.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="target"></param>
        void PreInvoke(MethodInfo method, object[] args, object target);

        /// <summary>
        /// execute the method invoke.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="target"></param>
        void AfterInvoke(MethodInfo method, object[] args, object target);

        /// <summary>
        /// around the method invoke.
        /// </summary>
        /// <param name="methodInvocation"></param>
        /// <returns></returns>
        object ArroundInvoke(IMethodInvocation methodInvocation);

        /// <summary>
        /// Handling the exception which occurs when the method is invoked.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="target"></param>
        /// <param name="ex"></param>
        void ExceptionHandle(MethodInfo method, object[] args, object target, Exception ex);
    }
}

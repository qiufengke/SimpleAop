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
        /// 
        /// execute before the method invoke.
        /// 
        void PreInvoke(MethodInfo method, object[] args, object target);

        /// 
        /// execute the method invoke.
        /// 
        void AfterInvoke(MethodInfo method, object[] args, object target);

        /// 
        /// Handling the exception which occurs when the method is invoked.
        /// 
        void ExceptionHandle(MethodInfo method, object[] args, object target, Exception ex);
    }
}

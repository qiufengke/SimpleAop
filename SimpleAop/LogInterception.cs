using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleAop.Interface;

namespace SimpleAop
{
    public class LogInterception : IInterception
    {
        public void PreInvoke(MethodInfo method, object[] args, object target)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("方法调用之前");
        }

        public void AfterInvoke(MethodInfo method, object[] args, object target)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("方法调用之后");
        }

        public void ExceptionHandle(MethodInfo method, object[] args, object target, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"抛出异常:{ex.ToString()}");
        }
    }
}

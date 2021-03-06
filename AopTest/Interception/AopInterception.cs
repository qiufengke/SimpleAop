﻿using System;
using System.Reflection;
using AopIntercept.Interface;

namespace AopTest.Interception
{
    public class AopInterception : IInterception
    {
        public string[] InterceptMethod
        {
            get { return new string[] { "*Excute" }; }
        }

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

        public object ArroundInvoke(IMethodInvocation methodInvocation)
        {
            try
            {
                Console.WriteLine("ArroundInvoke 拦截器");
                var result = methodInvocation.Proceed();
                Console.WriteLine("ArroundInvoke 拦截器调用结束");
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ExceptionHandle(MethodInfo method, object[] args, object target, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"抛出异常:\n{ex}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
using System;
using AopTest.Interception;
using SimpleAop.Attribute;

namespace AopTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            #region Attribute 方式

            var t = new TestAop();

            t.Excute();

            Console.WriteLine(" end.");

            //Console.WriteLine($"返回值：{result}");

            #endregion

            #region 代理模式

            //IInterception interception = new LogInterception();
            //ProxyFactory.EnableAfterInterception = true;
            //ProxyFactory.EnablePreInterception = true;
            //var t2 = ProxyFactory.CreateProxyInstance<TestAop2>(interception);
            //t2.Excute(); 

            #endregion

            Console.Read();
        }
    }

    public class TestAop : BaseAop
    {
        public void Excute()
        {
            Console.WriteLine(" execute method.");
            throw new Exception("66666666");
            Console.WriteLine(" end method.");
            //try
            //{
            //    Console.WriteLine(" execute method.");
            //    throw new Exception("66666666");
            //    Console.WriteLine(" end method.");
            //}
            //catch (Exception)
            //{
            //}
        }
    }

    public class TestAop2 : ContextBoundObject
    {
        public void Excute()
        {
            Console.WriteLine("execute method.");
        }
    }

    [AopProxy(typeof(AopInterception), false, false, false)]
    public abstract class BaseAop : ContextBoundObject
    {
    }
}
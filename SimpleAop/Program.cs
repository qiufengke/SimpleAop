using System;
using SimpleAop.Attribute;
using SimpleAop.Interception;
using SimpleAop.Interface;
using SimpleAop.Proxy;

namespace SimpleAop
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Attribute 方式
            //TestAop t = new TestAop();
            //t.Excute();

            // 代理模式
            IInterception interception = new LogInterception();
            ProxyFactory.EnableAfterInterception = true;
            ProxyFactory.EnablePreInterception = true;
            var t2 = ProxyFactory.CreateProxyInstance<TestAop2>(interception);
            t2.Excute();

            Console.Read();
        }
    }

    public class TestAop : BaseAop
    {
        public void Excute()
        {
            Console.WriteLine(" excute method.");
            throw new Exception("55");
        }
    }

    public class TestAop2 : ContextBoundObject
    {
        public void Excute()
        {
            Console.WriteLine("excute method.");
        }
    }

    [AopLogProxy(typeof(LogInterception), true, true)]
    public abstract class BaseAop : ContextBoundObject
    {
    }
}
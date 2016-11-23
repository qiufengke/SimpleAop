using System;
using System.Collections.Generic;
using AopIntercept.Attribute;
using AopIntercept.Extension;
using AopIntercept.Interface;
using AopIntercept.Proxy;
using AopTest.Interception;

namespace AopTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine($"int? 的 默认值：{typeof(int?).GetDefault() ?? "null"}");
            //Console.WriteLine($"bool 的 默认值：{typeof(bool).GetDefault() ?? "null"}");
            //Console.WriteLine($"string 的 默认值：{typeof(string).GetDefault() ?? "null"}");
            //Console.WriteLine($"TestAop 的 默认值：{typeof(TestAop).GetDefault() ?? "null"}");
            //Console.WriteLine($"Enum 的 默认值：{typeof(Enum).GetDefault() ?? "null"}");
            //Console.WriteLine($"List<int> 的 默认值：{typeof(List<int>).GetDefault() ?? "null"}");
            //Console.WriteLine($"List<string> 的 默认值：{typeof(List<string>).GetDefault() ?? "null"}");

            #region Attribute 方式

            var t = new TestAop();

            t.Excute01();
            t.Helio();


            #endregion

            #region 代理模式

            //IInterception interception = new AopInterception();
            //var proxyFactory = new ProxyFactory();
            //proxyFactory.EnableAfterInterception = true;
            //proxyFactory.EnablePreInterception = true;
            //var t2 = proxyFactory.CreateProxyInstance<TestAop2>(interception);
            //t2.Excute();

            #endregion

            Console.Read();
        }
    }

    public class TestAop : BaseAop
    {
        public void Excute01()
        {
            Console.WriteLine(" execute method.");
            //throw new Exception("66666666");
        }

        public void Helio()
        {
            Console.WriteLine("make helio great again.");
            //throw new Exception("hhhhh");
        }
    }

    public class TestAop2 : ContextBoundObject
    {
        public void Excute()
        {
            Console.WriteLine("execute method.");
        }
    }

    [AopProxy(typeof(AopInterception), true, true, false)]
    public abstract class BaseAop : ContextBoundObject
    {
    }
}
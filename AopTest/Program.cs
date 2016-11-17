using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAop.Attribute;
using SimpleAop.Interception;

namespace AopTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Attribute 方式
            TestAop t = new TestAop();

            var result = t.Excute();

            Console.WriteLine($"返回值：{result}");

            // 代理模式
            //IInterception interception = new LogInterception();
            //ProxyFactory.EnableAfterInterception = true;
            //ProxyFactory.EnablePreInterception = true;
            //var t2 = ProxyFactory.CreateProxyInstance<TestAop2>(interception);
            //t2.Excute();

            Console.Read();
        }
    }
    public class TestAop : BaseAop
    {
        public bool Excute()
        {
            Console.WriteLine(" execute method.");

            throw new Exception("55");

            Console.WriteLine(" end method.");

            return true;

        }
    }

    public class TestAop2 : ContextBoundObject
    {
        public void Excute()
        {
            Console.WriteLine("execute method.");
        }
    }

    [AopLogProxy(typeof(LogInterception), true, true)]
    public abstract class BaseAop : ContextBoundObject
    {
    }
}

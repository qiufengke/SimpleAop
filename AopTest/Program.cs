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
            #region Attribute 方式

            TestAop t = new TestAop();

            var r = t.Excute();
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
        public bool Excute()
        {
            Console.WriteLine(" execute method.");
            throw new Exception("66666666");
            Console.WriteLine(" end method.");
            return true;
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

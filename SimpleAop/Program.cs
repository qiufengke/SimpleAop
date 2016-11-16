using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAop.Attribute;

namespace SimpleAop
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAop t = new TestAop();
            t.Excute();
            Console.Read();
        }
    }

    public class TestAop : BaseAop
    {
        public void Excute()
        {
            Console.WriteLine("start excute method.");
            throw new Exception("55");
            Console.WriteLine("end excute method.");
        }
    }

    [AopLogProxy(typeof(LogInterception), true, true)]
    public abstract class BaseAop : ContextBoundObject
    {

    }
}

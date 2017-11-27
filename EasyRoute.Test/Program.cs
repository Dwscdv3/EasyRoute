using System;

namespace EasyRoute.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            EasyRoute.Settings.RequireAttribute = false; // IF YOU SET THIS, BEWARE OF MALICIOUS CALLS!
            EasyRoute.Settings.CaseSensitive = false;
            var obj = new MyDirectory();
            obj.SetAsRoot();
            Console.WriteLine(obj.Call("/greet,Dwscdv3/toupper"));
            Console.WriteLine(obj.Call("greet,1000"));
            Console.WriteLine(obj.Call("greet,1000,"));
            Console.WriteLine(EasyRoute.ObjectHelper.Call(typeof(MyDirectory), "greet,1000,7"));
            Console.ReadKey(true);
        }
    }

    class MyDirectory
    {
        [Route("Greet")]
        public string Greeting(string s)
        {
            return $"Hello, {s}!";
        }
        [Route("Greet")]
        public string Greeting(long i)
        {
            return $"Hello, {i.ToString("x")}!";
        }
        [Route("Greet")]
        public static string Greeting(int a, int b)
        {
            return $"Hello, {a - b}!";
        }
    }
}

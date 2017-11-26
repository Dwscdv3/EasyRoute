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
            Console.WriteLine(obj.Call("greet,Dwscdv3/toupper"));
            Console.ReadKey(true);
        }
    }

    class MyDirectory : EasyRoute.IDirectory
    {
        [Route("Greet")]
        public string Greeting(string s)
        {
            return $"Hello, {s}!";
        }
    }
}

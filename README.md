# EasyRoute

This project is designed for call methods from a URI.

Warning: This project may be very buggy at this time. Use it at your own risk.  
If you find a bug, please submit an issue!

## Example

``` C#
class Foo : EasyRoute.IDirectory {
    [Route]
    public string Greet(string name) => $"Hello, {name}!";

    [Route("Bar")]
    public Bar GetBar(string name) => new Bar();
}

class Bar : EasyRoute.IDirectory {
    [Route]
    public int Pow2(int i) => Math.Pow(2, i);
}
```

``` C#
var foo = new Foo();
Console.WriteLine(foo.Call("Greet,Dwscdv3"));  // Hello, Dwscdv3!

// Root directory
foo.SetAsRoot();
var something = new Bar();
Console.WriteLine(something.Call("/Bar/Pow2,16"));  // 65536
// Access root without an IDirectory object hasn't been implemented yet.

EasyRoute.Settings.CaseSensitive = false;
Console.WriteLine(foo.Call("pow2,8"));  // 256

// Allow methods without a RouteAttribute to be called.
EasyRoute.Settings.RequireAttribute = false; // IF YOU SET THIS, BEWARE OF MALICIOUS CALLS!
Console.WriteLine(foo.Call("/greet, Dwscdv3 /substring, 0, 9 /toupper"));  // HELLO, DW
// As you see, you can insert spaces around '/' and ',' if you preferred.
```

## Note

Interface IDirectory seems unnecessary, extension methods may move to System.Object in the future.
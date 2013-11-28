footswitch
==========

Mini class to work with [PCsensor][1] footswitch from C#.

Usage
-----

This class is using [LibUsbDotNet][2]. First install it and add device filter.
Example of usage:

```cs
var footswtich = new FootswitchListener();
 
try
{
     footswtich.StartListen();
 
     footswtich.Press += () => Console.WriteLine("Pressed button");
     footswtich.Release += () => Console.WriteLine("Release button");
 
catch (Exception fe)
{
     Console.WriteLine(fe.Message);
}
```


Authors
-------
[Vitaliy Velikodniy](mailto:vitaliy@velikodniy.name)

[1]: http://www.pcsensor.com/index.php?_a=viewCat&catId=2
[2]: http://libusbdotnet.sourceforge.net/V2/Index.html

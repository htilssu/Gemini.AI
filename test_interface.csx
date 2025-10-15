using Microsoft.Extensions.AI;
using System;
using System.Linq;

var type = typeof(IChatClient);
var methods = type.GetMethods();
foreach (var method in methods.Where(m => m.Name.Contains("Response") || m.Name.Contains("Complete")))
{
    Console.WriteLine($"{method.Name}: {method.ReturnType}");
    foreach (var param in method.GetParameters())
    {
        Console.WriteLine($"  - {param.Name}: {param.ParameterType}");
    }
}

# ChannelAdam.DispatchProxies

A .NET Standard library with disposable and retry-enabled dispatch proxies.

Targets:

- .NET 5.0
- .NET Standard 2.1
- .NET Standard 2.0
- .NET Standard 1.0

## Usage

Invoke Handler:

```csharp
public class MyObjectInvokeHandler : IObjectInvokeHandler
{
    public object? Invoke(object targetObject, MethodInfo targetMethod, object?[]? args)
    {
        Console.WriteLine($"Handling invocation of method {targetMethod.Name}");

        // Add your dispatch proxy code here!

        return targetMethod.Invoke(targetObject, args);
    }
}
```

Creation of the dispatch proxy:

```csharp
ICalculator calculatorProxy = DispatchProxyFactory.CreateObjectDispatchProxy<ICalculator>(
    new Calculator(),
    new MyObjectInvokeHandler());

// When you want the dispatch proxy to wrap around an IDisposable
ICalculator calculatorProxy = DispatchProxyFactory.CreateDisposableObjectDispatchProxy<ICalculator>(
    new Calculator(),
    new MyObjectInvokeHandler());

// When you want the dispatch proxy to wrap around an IDisposable with a destructor
ICalculator calculatorProxy = DispatchProxyFactory.CreateDisposableObjectDispatchProxyWithDestructor<ICalculator>(
    new Calculator(),
    new MyObjectInvokeHandler());
```

See the BehaviourSpecs for further example usage.

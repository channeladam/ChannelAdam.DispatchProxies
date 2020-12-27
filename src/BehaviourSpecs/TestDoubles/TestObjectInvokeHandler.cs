using System;
using System.Reflection;
using ChannelAdam.DispatchProxies.Abstractions;

namespace BehaviourSpecs.TestDoubles
{
    public class TestObjectInvokeHandler : IObjectInvokeHandler
    {
        public object? Invoke(object targetObject, MethodInfo targetMethod, object?[]? args)
        {
            Console.WriteLine($"Handling invocation of method {targetMethod.Name}");

            return targetMethod.Invoke(targetObject, args);
        }
    }
}
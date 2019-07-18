using System.Reflection;
using ChannelAdam.DispatchProxies.Abstractions;

namespace BehaviourSpecs.TestDoubles
{
    public class TestObjectDispatchProxy : ObjectDisposableDispatchProxy
    {
        public static T Create<T>(object objectToProxy)
        {
            object dispatchProxyAsObject = DispatchProxy.Create<T, TestObjectDispatchProxy>();

            // DispatchProxy.Create() actually returns a 'T' but the cast below to TestObjectDispatchProxy won't compile if it isn't declared as 'object'
            var objectDispatchProxy = (TestObjectDispatchProxy)dispatchProxyAsObject;
            objectDispatchProxy.ProxiedObject = objectToProxy;

            return (T)dispatchProxyAsObject;
        }

        protected override object InvokeMethodOnProxiedObject(MethodInfo targetMethod, object[] args)
        {
            // Do nothing different here for this test proxy
            return base.InvokeMethodOnProxiedObject(targetMethod, args);
        }

        protected override void DisposeManagedResources()
        {
            // Do nothing different here for this test proxy
            base.DisposeManagedResources();
        }

        protected override void DisposeUnmanagedResources()
        {
            // Do nothing different here for this test proxy
            base.DisposeUnmanagedResources();
        }
    }
}
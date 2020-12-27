//-----------------------------------------------------------------------
// <copyright file="DisposableObjectDispatchProxy.cs">
//     Copyright (c) 2018-2021 Adam Craven. All rights reserved.
// </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

namespace ChannelAdam.DispatchProxies
{
    using System;
    using System.Reflection;
    using ChannelAdam.DispatchProxies.Abstractions;

    /// <summary>
    /// Proxies the given object using the given handler using a Dispose Pattern on top of a <see cref="DispatchProxy"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the static factory method <c>Create</c> to create an instance.
    /// </para>
    /// <para>
    /// Dispose pattern implementation follows the approach used in ChannelAdam.Disposing.
    /// </para>
    /// </remarks>
    public class DisposableObjectDispatchProxy : Abstractions.Internal.CoreDisposableObjectDispatchProxy, IDisposable
    {
        /// <summary>
        /// This class must have a public constructor with no parameters - otherwise DispatchProxy.Create() will fail.
        /// </summary>
        public DisposableObjectDispatchProxy()
        {
        }

        /// <summary>
        /// Factory method to create a disposable object dispatch proxy with a destructor.
        /// </summary>
        /// <param name="objectToProxy">The actual object to be proxied.</param>
        /// <param name="invokeHandler">The handler for performing the proxying on the actual object.</param>
        /// <typeparam name="TObjectInterface">The interface of the object to be proxied.</typeparam>
        /// <returns>An instance of <see cref="DisposableObjectDispatchProxy"/>.</returns>
        public static TObjectInterface Create<TObjectInterface>(object objectToProxy, IObjectInvokeHandler invokeHandler)
        {
            /// Create an object that derives from <see cref="DisposableObjectDispatchProxy"/> and implements interface <c>TObjectInterface</c>.
            /// The class must have a public constructor with no parameters - otherwise <c>DispatchProxy.Create()</c> will fail.
            TObjectInterface dispatchProxy = DispatchProxy.Create<TObjectInterface, DisposableObjectDispatchProxy>();

            object? dispatchProxyAsObject = dispatchProxy;
            if (dispatchProxyAsObject is null)
            {
                throw new InvalidOperationException($"Unable to create the dispatch proxy object derived from DisposableObjectDispatchProxy with interface: {typeof(TObjectInterface).FullName}");
            }

            var objectDispatchProxy = (DisposableObjectDispatchProxy)dispatchProxyAsObject;
            objectDispatchProxy.ProxiedObject = objectToProxy ?? throw new ArgumentNullException(nameof(objectToProxy));
            objectDispatchProxy.InvokeHandler = invokeHandler ?? throw new ArgumentNullException(nameof(invokeHandler));

            return dispatchProxy;
        }

        #region Public Methods - Dispose Pattern

        /// <summary>
        /// Performs the deterministic, application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// Implementation of <c>IDisposable.Dispose</c> method.
        /// </remarks>
        public void Dispose()
        {
            // Call the Dispose method and indicate we are calling it deterministically from our application code.
            // (Garbage collection calls finalisers that destroys managed objects non-deterministically.)
            base.Dispose(disposing:true);

            // This object is being cleaned up by the Dispose method.
            // Calling GC.SupressFinalize() takes this object off the finalization queue and prevents
            // finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods - Dispose Pattern - Abstract Implementations

        /// <summary>
        /// Dispose of managed resources here.
        /// </summary>
        /// <remarks>
        /// This releases them faster than if they were reclaimed non-deterministically from a finaliser.
        /// Call <c>SafeDispose()</c> on every managed resource that needs to be disposed.
        /// </remarks>
        protected override void DisposeManagedResources()
        {
            SafeDispose(this.ProxiedObject);
            SafeDispose(this.InvokeHandler);
        }

        /// <summary>
        /// Release unmanaged resources here.
        /// </summary>
        /// <remarks>
        /// The implementer is responsible for ensuring that they do not interact with managed objects that may have been reclaimed by the Garbage Collector.
        /// <remarks>
        protected override void DisposeUnmanagedResources()
        {
            // nothing to do here
        }

        /// <summary>
        /// Set the disposed resources to null to make them unreachable, and to prevent double disposal attempts.
        /// </summary>
        /// <remarks>
        /// This executes after all the Dispose*anagedResources*() methods.
        /// </remarks>
        protected override void SetResourcesToNull()
        {
            this.ProxiedObject = null;
            this.InvokeHandler = null;
        }

        #endregion

        #region Protected Methods - DispatchProxy - Overrides

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            object? result = null;

            try
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException($"Calling {targetMethod.Name} while proxying via {this.GetType().FullName}");
                }

                if ("Dispose".Equals(targetMethod.Name, StringComparison.OrdinalIgnoreCase))
                {
                    // Invoke Dispose() on this proxy object, with hooks that should be overriden to invoke it on the actual object
                    this.Dispose();
                }
                else
                {
                    // Invoke the target method on the proxied/actual object via the handler
                    result = base.Invoke(targetMethod, args);
                }
            }
            catch (TargetInvocationException targetEx)
            {
                if (targetEx.InnerException != null)
                {
                    // Unwrap the real exception from the TargetInvocationException
                    throw new AggregateException(new[] { targetEx.InnerException });
                }

                throw;
            }

            return result;
        }

        #endregion
    }
}

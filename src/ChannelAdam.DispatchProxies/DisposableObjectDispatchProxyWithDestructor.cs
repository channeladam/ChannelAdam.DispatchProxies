//-----------------------------------------------------------------------
// <copyright file="DisposableObjectDispatchProxyWithDestructor.cs">
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
    /// NOTE: the implementation of this class must have a public constructor with no parameters otherwise DispatchProxy.Create() will fail.
    /// </para>
    /// <para>
    /// Inspiration for a Dispose Pattern - see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    /// See https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/b1yfkh5e(v=vs.100)
    /// </para>
    /// </remarks>
    public class DisposableObjectDispatchProxyWithDestructor : DisposableObjectDispatchProxy
    {
        /// <summary>
        /// This class must have a public constructor with no parameters - otherwise DispatchProxy.Create() will fail.
        /// </summary>
        public DisposableObjectDispatchProxyWithDestructor()
        {
        }

        #region Destructor

        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableObjectDispatchProxyWithDestructor"/> class.
        /// </summary>
        /// <remarks>
        /// This destructor will be called by the GC only if the Dispose method does not get called.
        /// Do not provide destructors in types derived from this class - derived types should instead override the Dispose method.
        /// </remarks>
        ~DisposableObjectDispatchProxyWithDestructor()
        {
            try
            {
                // Pass false to indicate that this is a finaliser, calling it non-deterministically.
                // The Garbage Collector calls finalisers that destroys managed objects non-deterministically.
                this.Dispose(disposing:false);
            }
            catch (Exception e)
            {
                // Suppress exceptions thrown from a destructor/finalizer so they do not go back into the Garbage Collector!
                // But, allow the client code to handle the exception - e.g. log it
                this.OnDestructorException(e);
            }
        }

        #endregion

        #region Public Properties

         /// <summary>
        /// Gets or sets the behaviour to perform when an exception occurs during the destructor/finalize.
        /// </summary>
        /// <value>
        /// The exception behaviour.
        /// </value>
        public Action<Exception>? DestructorExceptionBehaviour
        {
            get;
            set;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Factory method to create a disposable object dispatch proxy with a destructor.
        /// </summary>
        /// <param name="objectToProxy">The actual object to be proxied.</param>
        /// <param name="invokeHandler">The handler for performing the proxying on the actual object.</param>
        /// <typeparam name="TObjectInterface">The interface of the object to be proxied.</typeparam>
        /// <returns>An instance of <see cref="DisposableObjectDispatchProxyWithDestructor"/>.</returns>
        public static new TObjectInterface Create<TObjectInterface>(object objectToProxy, IObjectInvokeHandler invokeHandler)
        {
            /// Create an object that derives from <see cref="DisposableObjectDispatchProxyWithDestructor"/> and implements interface <c>TObjectInterface</c>.
            /// The class must have a public constructor with no parameters - otherwise <c>DispatchProxy.Create()</c> will fail.
            TObjectInterface dispatchProxy = DispatchProxy.Create<TObjectInterface, DisposableObjectDispatchProxyWithDestructor>();

            object? dispatchProxyAsObject = dispatchProxy;
            if (dispatchProxyAsObject is null)
            {
                throw new InvalidOperationException($"Unable to create the dispatch proxy object derived from DisposableObjectDispatchProxyWithDestructor with interface: {typeof(TObjectInterface).FullName}");
            }

            var objectDispatchProxy = (DisposableObjectDispatchProxyWithDestructor)dispatchProxyAsObject;
            objectDispatchProxy.ProxiedObject = objectToProxy ?? throw new ArgumentNullException(nameof(objectToProxy));
            objectDispatchProxy.InvokeHandler = invokeHandler ?? throw new ArgumentNullException(nameof(invokeHandler));

            return dispatchProxy;
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called when there is an exception in the destructor/finalize.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual void OnDestructorException(Exception exception)
        {
            try
            {
                this.DestructorExceptionBehaviour?.Invoke(exception);
            }
            catch (Exception ex)
            {
                // Failsafe - Suppress this so they do not go back into the Garbage Collector!
                Console.Error.WriteLine("Exception occurred during destructor: " + ex);
            }
        }

        #endregion Protected Methods
    }
}

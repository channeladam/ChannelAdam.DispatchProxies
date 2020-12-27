//-----------------------------------------------------------------------
// <copyright file="ObjectDispatchProxy.cs">
//     Copyright (c) 2020-2021 Adam Craven. All rights reserved.
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
    /// Proxies the given object using the given handler.
    /// </summary>
    /// <remarks>
    /// Use the static factory method <c>Create</c> to create an instance.
    /// </remarks>
    public class ObjectDispatchProxy : CoreObjectDispatchProxy
    {
        /// <summary>
        /// This class must have a public constructor with no parameters - otherwise DispatchProxy.Create() will fail.
        /// </summary>
        public ObjectDispatchProxy()
        {
        }

        /// <summary>
        /// Factory method to create an object dispatch proxy.
        /// </summary>
        /// <param name="objectToProxy">The actual object to be proxied.</param>
        /// <param name="invokeHandler">The handler for performing the proxying on the actual object.</param>
        /// <typeparam name="TObjectInterface">The interface of the object to be proxied.</typeparam>
        /// <returns>An instance of <see cref="ObjectDispatchProxy"/>.</returns>
        public static TObjectInterface Create<TObjectInterface>(object objectToProxy, IObjectInvokeHandler invokeHandler)
        {
            /// Create an object that derives from <see cref="ObjectDispatchProxy"/> and implements interface <c>TObjectInterface</c>.
            /// The class must have a public constructor with no parameters - otherwise <c>DispatchProxy.Create()</c> will fail.
            TObjectInterface dispatchProxy = DispatchProxy.Create<TObjectInterface, ObjectDispatchProxy>();

            object? dispatchProxyAsObject = dispatchProxy;
            if (dispatchProxyAsObject is null)
            {
                throw new InvalidOperationException($"Unable to create the dispatch proxy object derived from ObjectDispatchProxy with interface: {typeof(TObjectInterface).FullName}");
            }

            var objectDispatchProxy = (ObjectDispatchProxy)dispatchProxyAsObject;
            objectDispatchProxy.ProxiedObject = objectToProxy ?? throw new ArgumentNullException(nameof(objectToProxy));
            objectDispatchProxy.InvokeHandler = invokeHandler ?? throw new ArgumentNullException(nameof(invokeHandler));

            return dispatchProxy;
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="DispatchProxyFactory.cs">
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
    using ChannelAdam.DispatchProxies.Abstractions;

    /// <summary>
    /// Proxies the given object using the given handler.
    /// </summary>
    /// <remarks>
    /// Use the static factory method <c>Create</c> to create an instance.
    /// </remarks>
    public static class DispatchProxyFactory
    {
        /// <summary>
        /// Factory method to create an <see cref="ObjectDispatchProxy"/>.
        /// </summary>
        /// <param name="objectToProxy">The actual object to be proxied.</param>
        /// <param name="invokeHandler">The handler for performing the proxying on the actual object.</param>
        /// <typeparam name="TObjectInterface">The interface of the object to be proxied.</typeparam>
        /// <returns>An instance of <see cref="ObjectDispatchProxy"/>.</returns>
        public static TObjectInterface CreateObjectDispatchProxy<TObjectInterface>(object objectToProxy, IObjectInvokeHandler invokeHandler)
        {
            return ObjectDispatchProxy.Create<TObjectInterface>(objectToProxy, invokeHandler);
        }

        /// <summary>
        /// Factory method to create a <see cref="DisposableObjectDispatchProxy"/>.
        /// </summary>
        /// <param name="objectToProxy">The actual object to be proxied.</param>
        /// <param name="invokeHandler">The handler for performing the proxying on the actual object.</param>
        /// <typeparam name="TObjectInterface">The interface of the object to be proxied.</typeparam>
        /// <returns>An instance of <see cref="DisposableObjectDispatchProxy"/>.</returns>
        public static TObjectInterface CreateDisposableObjectDispatchProxy<TObjectInterface>(object objectToProxy, IObjectInvokeHandler invokeHandler)
        {
            return DisposableObjectDispatchProxy.Create<TObjectInterface>(objectToProxy, invokeHandler);
        }

        /// <summary>
        /// Factory method to create a <see cref="DisposableObjectDispatchProxyWithDestructor"/>.
        /// </summary>
        /// <param name="objectToProxy">The actual object to be proxied.</param>
        /// <param name="invokeHandler">The handler for performing the proxying on the actual object.</param>
        /// <typeparam name="TObjectInterface">The interface of the object to be proxied.</typeparam>
        /// <returns>An instance of <see cref="DisposableObjectDispatchProxyWithDestructor"/>.</returns>
        public static TObjectInterface CreateDisposableObjectDispatchProxyWithDestructor<TObjectInterface>(object objectToProxy, IObjectInvokeHandler invokeHandler)
        {
            return DisposableObjectDispatchProxyWithDestructor.Create<TObjectInterface>(objectToProxy, invokeHandler);
        }
    }
}

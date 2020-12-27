//-----------------------------------------------------------------------
// <copyright file="IObjectInvokeHandler.cs">
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

using System.Reflection;

namespace ChannelAdam.DispatchProxies.Abstractions
{
    /// <summary>
    /// An interface for handling the invocation of a method on a given object.
    /// </summary>
    public interface IObjectInvokeHandler
    {
        /// <summary>
        /// Invokes the named method on the given object and returns the object to be returned to the caller of the proxy.
        /// </summary>
        /// <param name="targetObject">The target object on which to invoke the method.</param>
        /// <param name="targetMethod">The method to call on the target object.</param>
        /// <param name="args">The parameters to pass to the method when it is called.</param>
        /// <returns>The object to be returned to the caller of the proxy.</returns>
        object? Invoke(object targetObject, MethodInfo targetMethod, object?[]? args);
    }
}
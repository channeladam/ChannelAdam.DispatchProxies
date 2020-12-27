//-----------------------------------------------------------------------
// <copyright file="CoreObjectDispatchProxy.cs">
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

namespace ChannelAdam.DispatchProxies.Abstractions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Common base class for our dispath proxies implementations that use an <see cref="IObjectInvokeHandler"/>.
    /// </summary>
    public abstract class CoreObjectDispatchProxy : DispatchProxy
    {
        protected IObjectInvokeHandler? InvokeHandler { get; set; }

        protected object? ProxiedObject { get; set; }

        #region Protected Methods - DispatchProxy - Overrides

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            try
            {
                return this.InvokeTargetMethodViaHandler(targetMethod, args);
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
        }

        #endregion

        #region Protected Methods - Virtual Methods

        /// <summary>
        /// Invokes the method on the proxied/actual object and returns the result to the caller.
        /// </summary>
        /// <param name="targetMethod">The method to call on the actual object being proxied.</param>
        /// <param name="args">The parameters to pass to the method when it is called.</param>
        /// <returns>The object result to be returned to the caller.</returns>
        protected virtual object? InvokeTargetMethodViaHandler(MethodInfo targetMethod, object?[]? args)
        {
            if (this.InvokeHandler is null)
            {
                throw new InvalidOperationException("Invoke handler is null");
            }

            if (this.ProxiedObject is null)
            {
                throw new InvalidOperationException("Proxied object is null");
            }

            // Use the invokeHandler to invoke the target method on the proxied/actual object
            return this.InvokeHandler.Invoke(this.ProxiedObject, targetMethod, args);
        }

        #endregion
    }
}

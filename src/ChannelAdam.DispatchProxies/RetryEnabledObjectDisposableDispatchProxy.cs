//-----------------------------------------------------------------------
// <copyright file="RetryEnabledObjectDisposableDispatchProxy.cs">
//     Copyright (c) 2018 Adam Craven. All rights reserved.
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
    using ChannelAdam.TransientFaultHandling;
    using System.Reflection;

    /// <summary>
    /// Proxies a given object and follows a specified retry policy.
    /// </summary>
    /// <remarks>
    /// Note: this class must have a public constructor with no parameters otherwise DispatchProxy.Create() will fail.
    /// </remarks>
    public class RetryEnabledObjectDisposableDispatchProxy : ObjectDisposableDispatchProxy
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the retry policy to use when invoking the method on the object being proxied.
        /// </summary>
        public IRetryPolicyFunction RetryPolicy { get; protected set; }

        #endregion Public Properties

        #region Public Static Methods

        public static T Create<T>(object objectToProxy, IRetryPolicyFunction retryPolicy)
        {
            object proxy = DispatchProxy.Create<T, RetryEnabledObjectDisposableDispatchProxy>();

            var retryDispatchProxy = (RetryEnabledObjectDisposableDispatchProxy)proxy;
            retryDispatchProxy.ProxiedObject = objectToProxy;
            retryDispatchProxy.RetryPolicy = retryPolicy;

            return (T)proxy;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Invokes the method and returns the object to be returned to the caller of the proxy.
        /// </summary>
        /// <param name="targetMethod">The method to call on the actual object being proxied.</param>
        /// <param name="args">The parameters to pass to the method when it is called.</param>
        /// <returns>The object to be returned to the caller of the proxy.</returns>
        protected override object InvokeMethodOnProxiedObject(MethodInfo targetMethod, object[] args)
        {
            if (this.RetryPolicy != null)
            {
                return this.RetryPolicy.Execute(() => base.InvokeMethodOnProxiedObject(targetMethod, args));
            }
            else
            {
                return base.InvokeMethodOnProxiedObject(targetMethod, args);
            }
        }

        protected override void DisposeManagedResources()
        {
            this.RetryPolicy = null;

            base.DisposeManagedResources();
        }

        #endregion Protected Methods
    }
}

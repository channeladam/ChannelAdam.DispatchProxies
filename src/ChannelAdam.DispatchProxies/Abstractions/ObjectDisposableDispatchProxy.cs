//-----------------------------------------------------------------------
// <copyright file="ObjectDisposableDispatchProxy.cs">
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

namespace ChannelAdam.DispatchProxies.Abstractions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Proxies the given object.
    /// </summary>
    /// <remarks>
    /// Note: this class must have a public constructor with no parameters otherwise DispatchProxy.Create() will fail.
    /// </remarks>
    public abstract class ObjectDisposableDispatchProxy : DisposableDispatchProxy
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the actual object that is being proxied.
        /// </summary>
        public object ProxiedObject { get; protected set; }

        #endregion

        #region Public Static Methods

        public static T Create<T>(object objectToProxy)
        {
            object proxy = DispatchProxy.Create<T, ObjectDisposableDispatchProxy>();

            var objectDispatchProxy = (ObjectDisposableDispatchProxy)proxy;
            objectDispatchProxy.ProxiedObject = objectToProxy;

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
            targetMethod = targetMethod ?? throw new ArgumentException(nameof(targetMethod));

            try
            {
                return targetMethod.Invoke(this.ProxiedObject, args);
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

        protected override void DisposeManagedResources()
        {
            this.ProxiedObject = null;

            base.DisposeManagedResources();
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Hide the overridable method from <see cref="DispatchProxy"/> from future inheritors.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private new object Invoke(MethodInfo targetMethod, object[] args)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}

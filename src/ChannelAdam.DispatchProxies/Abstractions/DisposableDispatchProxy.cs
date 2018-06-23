//-----------------------------------------------------------------------
// <copyright file="DisposableDispatchProxy.cs">
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
    using ChannelAdam.Disposing.Abstractions;
    using System;
    using System.Reflection;

    /// <summary>
    /// Abstract class that implements the Dispose Pattern on top of a <see cref="DispatchProxy"/>.
    /// </summary>
    /// <remarks>
    /// Note: this class must have a public constructor with no parameters otherwise DispatchProxy.Create() will fail.
    /// The Dispose Pattern - See http://msdn.microsoft.com/en-us/library/b1yfkh5e.aspx.
    /// See http://msdn.microsoft.com/en-us/library/vstudio/b1yfkh5e(v=vs.100).aspx".
    /// </remarks>
    public abstract class DisposableDispatchProxy : DispatchProxy, IDisposable
    {
        #region Fields

        private volatile bool isDisposed;   // volatile because the Garbage Collector calls finalizers in a different thread

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableDispatchProxy"/> class.
        /// </summary>
        /// <remarks>
        /// This destructor will be called by the GC only if the Dispose method does not get called.
        /// Do not provide destructors in types derived from this class - derived types should instead override the Dispose method.
        /// </remarks>
        ~DisposableDispatchProxy()
        {
            try
            {
                this.Dispose(false);
            }
            catch (Exception e)
            {
                // Suppress exceptions thrown from a destructor/finalizer so they do not go back into the Garbage Collector!
                // But, allow the client code to handle the exception - e.g. log it
                this.OnDestructorException(e);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the object is about to be disposed.
        /// </summary>
        public event EventHandler<DisposingEventArgs> Disposing;

        /// <summary>
        /// Occurs when the object has been disposed.
        /// </summary>
        public event EventHandler Disposed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        /// <summary>
        /// Gets or sets the behaviour to perform when an exception occurs during the destructor/finalize.
        /// </summary>
        /// <value>
        /// The exception behaviour.
        /// </value>
        public Action<Exception> DestructorExceptionBehaviour
        {
            get;
            set;
        }

        #endregion

        #region Public Methods - Dispose Pattern

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object is being cleaned up by the Dispose method.
            // Calling GC.SupressFinalize() takes this object off the finalization queue and prevents
            // finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods - DispatchProxy - Overrides

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            object result = null;

            targetMethod = targetMethod ?? throw new ArgumentException(nameof(targetMethod));

            try
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException($"Calling {targetMethod.Name} while proxying via {this.GetType().FullName}");
                }

                if (targetMethod.Name == "Dispose")
                {
                    // Invoke Dispose() on the proxy object
                    result = targetMethod.Invoke(this, args);
                }
                else
                {
                    // Invoke whatever method on the actual object
                    result = this.InvokeMethodOnProxiedObject(targetMethod, args);
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

        #region Protected Methods - DispatchProxy - Abstract Method for Invoking a Method

        /// <summary>
        /// Invokes the method on the actual object being proxied and returns the return object to the caller of the proxy.
        /// </summary>
        /// <param name="targetMethod">The method to call on the actual object being proxied.</param>
        /// <param name="args">The parameters to pass to the method when it is called.</param>
        /// <returns>The object to be returned to the caller of the proxy.</returns>
        protected abstract object InvokeMethodOnProxiedObject(MethodInfo targetMethod, object[] args);

        #endregion

        #region Protected Methods - Dispose Pattern - Virtual Methods

        /// <summary>
        /// Release managed resources here.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            // override this method
        }

        /// <summary>
        /// Release unmanaged resources here.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
            // override this method
        }

        /// <summary>
        /// Called when the object is about to be disposed.
        /// </summary>
        /// <param name="isDisposing">If set to <c>true</c> then the object is being disposed from a call to Dispose(); <c>false</c> if it is from a finalizer/destructor.</param>
        protected virtual void OnDisposing(bool isDisposing)
        {
            this.Disposing?.Invoke(this, new DisposingEventArgs(isDisposing));
        }

        /// <summary>
        /// Called when the object has been disposed.
        /// </summary>
        protected virtual void OnDisposed()
        {
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Protected Methods

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

        #endregion

        #region Private Methods - Dispose Pattern

        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.OnDisposing(disposing);

            this.DisposeUnmanagedResources();

            if (disposing)
            {
                this.DisposeManagedResources();
            }

            this.isDisposed = true;

            this.OnDisposed();
        }

        #endregion
    }
}

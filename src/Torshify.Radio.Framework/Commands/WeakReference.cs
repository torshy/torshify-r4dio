#region Header

// <copyright file="WeakReference.cs" company="Nito Programs">
//     Copyright (c) 2009 Nito Programs.
// </copyright>

#endregion Header

using System;
using System.Runtime.InteropServices;

namespace Torshify.Radio.Framework.Commands
{
    /// <summary>
    /// Represents a weak reference, which references an object while still allowing that object to be reclaimed by garbage collection.
    /// </summary>
    /// <remarks>
    /// <para>We define our own type, unrelated to <see cref="System.WeakReference"/> both to provide type safety and because <see cref="System.WeakReference"/> is an incorrect implementation (it does not implement <see cref="IDisposable"/>).</para>
    /// </remarks>
    /// <typeparam name="T">The type of object to reference.</typeparam>
    internal sealed class WeakReference<T> : IDisposable
        where T : class
    {
        #region Fields

        /// <summary>
        /// The contained <see cref="SafeGCHandle"/>.
        /// </summary>
        private SafeGCHandle safeHandle;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}"/> class, referencing the specified object.
        /// </summary>
        /// <param name="target">The object to track. May not be null.</param>
        public WeakReference(T target)
        {
            this.safeHandle = new SafeGCHandle(target, GCHandleType.Weak);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the object is still alive (has not been garbage collected).
        /// </summary>
        public bool IsAlive
        {
            get { return this.safeHandle.Handle.Target != null; }
        }

        /// <summary>
        /// Gets the referenced object. Will return null if the object has been garbage collected.
        /// </summary>
        public T Target
        {
            get { return this.safeHandle.Handle.Target as T; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Frees the weak reference.
        /// </summary>
        public void Dispose()
        {
            this.safeHandle.Dispose();
        }

        #endregion Methods
    }
}
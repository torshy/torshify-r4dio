#region Header

// <copyright file="CanExecuteChangedCore.cs" company="Nito Programs">
//     Copyright (c) 2009 Nito Programs.
// </copyright>

#endregion Header

using System;
using System.Windows.Input;

namespace Torshify.Radio.Framework.Commands
{
    /// <summary>
    /// Class that implements <see cref="CanExecuteChanged"/> as a weak event.
    /// </summary>
    public sealed class CanExecuteChangedCore : IDisposable
    {
        #region Fields

        /// <summary>
        /// The weak collection of delegates for <see cref="CanExecuteChanged"/>.
        /// </summary>
        private IWeakCollection<EventHandler> canExecuteChanged = new WeakCollection<EventHandler>();

        #endregion Fields

        #region Events

        /// <summary>
        /// This is a weak event. Provides notification that the result of <see cref="ICommand.CanExecute"/> may be different.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { this.canExecuteChanged.Add(value); }
            remove { this.canExecuteChanged.Remove(value); }
        }

        #endregion Events

        #region Methods

        /// <summary>
        /// Frees all weak references to delegates held by <see cref="CanExecuteChanged"/>.
        /// </summary>
        public void Dispose()
        {
            this.canExecuteChanged.Dispose();
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event for any listeners still alive, and removes any references to garbage collected listeners.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            foreach (EventHandler cb in this.canExecuteChanged.LiveList)
            {
                cb(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}
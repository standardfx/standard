using System;
using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Extension methods for the <see cref="EventHandler"/> class.
    /// </summary>
    public static class EventHandlerExtension
    {
        /// <summary>
        /// Raise an event.
        /// </summary>
        /// <typeparam name="T">The type of the event handler method.</typeparam>
        /// <param name="handler">The event handler method.</param>
        /// <param name="sender">The control that has invoked the event.</param>
        /// <param name="e">Event argument for the event handler.</param>
        public static void Raise<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// @ref <see cref="Raise{T}(EventHandler{T}, object, T)"/>
        /// </summary>
        public static void Raise(this EventHandler handler, object sender)
        {
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }
    }
}
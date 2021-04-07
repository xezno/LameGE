﻿using Engine.Utils.MathUtils;
using System;
using System.Numerics;

namespace Engine.ECS.Observer
{
    public class WindowResizeNotifyArgs : INotifyArgs
    {
        /// <summary>
        /// The object triggering the notification.
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The time at which the notification was broadcast.
        /// </summary>
        public DateTime TimeSent { get; set; }

        /// <summary>
        /// The window's new size.
        /// </summary>
        public Vector2 WindowSize { get; }

        /// <summary>
        /// Construct a new instance of <see cref="WindowResizeNotifyArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="windowSize">The window's new size.</param>
        /// <param name="sender">The object triggering the notification.</param>
        public WindowResizeNotifyArgs(Vector2 windowSize, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            WindowSize = windowSize;
        }
    }
}
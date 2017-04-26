using System;

namespace Taobao.Top.Link.Channel
{
    /// <summary>channel closed reason
    /// </summary>
    public class ChannelClosedEventArgs : EventArgs
    {
        /// <summary>get reason why closed
        /// </summary>
        public string Reason { get; private set; }
        public ChannelClosedEventArgs(string reason)
            : base()
        {
            this.Reason = reason;
        }
    }
}
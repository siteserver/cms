using System;

namespace Taobao.Top.Link.Channel
{
    /// <summary>the channel that client connect to server
    /// </summary>
    public interface IClientChannel : IChannelSender
    {
        /// <summary>while message received on this channel
        /// </summary>
        EventHandler<ChannelContext> OnMessage { get; set; }
        /// <summary>while error occur on this channel
        /// </summary>
        EventHandler<ChannelContext> OnError { get; set; }
        /// <summary>while channel was closed by given reason
        /// </summary>
        EventHandler<ChannelClosedEventArgs> OnClosed { get; set; }
        /// <summary>get or set remote uri
        /// </summary>
        Uri Uri { get; set; }
        /// <summary>weather the channel is valid
        /// </summary>
        bool IsConnected { get; }
        /// <summary>timer for heartbeat if set
        /// </summary>
        ResetableTimer HeartbeatTimer { set; }
    }
}
using System;
using Top.Api;

namespace Taobao.Top.Link.Channel
{
    /// <summary>the channel that server bound
    /// </summary>
    public abstract class ServerChannel
    {
        protected ITopLogger Logger { get; private set; }
        /// <summary>get server address
        /// </summary>
        protected int Port { get; private set; }
        /// <summary>get or set connection max idle time that do not send or receive
        /// </summary>
        public int MaxIdleTimeSeconds { get; set; }
        /// <summary>while message received on this channel
        /// </summary>
        public EventHandler<ChannelContext> OnMessage { get; set; }
        /// <summary>while error occur on this channel
        /// </summary>
        public EventHandler<ChannelContext> OnError { get; set; }

        public ServerChannel(ITopLogger logger, int port)
        {
            this.Logger = logger;
            this.Port = port;
        }

        /// <summary>start server
        /// </summary>
        public abstract void Start();
        /// <summary>stop server
        /// </summary>
        public abstract void Stop();
    }
}
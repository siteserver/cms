using System;

namespace Taobao.Top.Link.Channel
{
    /// <summary>context used with channel event
    /// </summary>
    public class ChannelContext : EventArgs
    {
        /// <summary>error from channel
        /// </summary>
        public Exception Error { get; private set; }
        /// <summary>the channel used to sending message
        /// </summary>
        public IChannelSender Sender { get; private set; }
        /// <summary>received message
        /// </summary>
        public object Message { get; private set; }

        public ChannelContext(Exception error)
        {
            this.Error = error;
        }
        public ChannelContext(object message, IChannelSender sender)
        {
            this.Message = message;
            this.Sender = sender;
        }

        /// <summary>
        /// send data to channel where the message come from
        /// </summary>
        /// <param name="data"></param>
        public void Reply(byte[] data)
        {
            if (this.Sender == null)
                throw new LinkException("can not send");
            this.Sender.Send(data);
        }
    }
}
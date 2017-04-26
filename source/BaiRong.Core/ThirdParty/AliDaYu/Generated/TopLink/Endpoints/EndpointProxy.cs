using System;
using System.Collections.Generic;
using Taobao.Top.Link.Channel;

namespace Taobao.Top.Link.Endpoints
{
    /// <summary>logic endpoint local proxy object
    /// </summary>
    public class EndpointProxy
    {
        private IList<IChannelSender> _senders;
        private Random _random;
        private EndpointHandler _handler;

        /// <summary>get id
        /// </summary>
        public Identity Identity { get; internal set; }
        /// <summary>known by both side, like a sessionId
        /// </summary>
        public string Token { get; internal set; }

        public EndpointProxy(EndpointHandler handler)
        {
            this._senders = new List<IChannelSender>();
            this._random = new Random();
            this._handler = handler;
        }

        internal void Add(IChannelSender sender)
        {
            if (!this._senders.Contains(sender))
                lock (this._senders)
                    if (!this._senders.Contains(sender))
                        this._senders.Add(sender);
        }

        internal void Remove(IChannelSender sender)
        {
            lock (this._senders)
                this._senders.Remove(sender);
        }

        internal void Remove(string uri)
        {
            lock (this._senders)
            {
                for (var i = 0; i < this._senders.Count; i++)
                {
                    var channel = this._senders[i] as IClientChannel;
                    if (channel == null)
                        continue;
                    if (!channel.Uri.ToString().Equals(uri))
                        continue;
                    this._senders.Remove(channel);
                    i--;
                }
            }
        }

        internal void Close(string uri, string reason)
        {
            lock (this._senders)
            {
                for (var i = 0; i < this._senders.Count; i++)
                {
                    var channel = this._senders[i] as IClientChannel;
                    if (channel == null)
                        continue;
                    if (!channel.Uri.ToString().Equals(uri))
                        continue;
                    channel.Close(reason);
                    this._senders.Remove(channel);
                    i--;
                }
            }
        }

        /// <summary>check is there any sender can be used to send
        /// </summary>
        /// <returns></returns>
        public bool hasValidSender()
        {
            foreach (IClientChannel sender in this._senders)
            {
                if (sender.IsConnected)
                    return true;
            }
            return false;
        }
        /// <summary>send message and wait reply
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IDictionary<string, object> SendAndWait(IDictionary<string, object> message)
        {
            return this.SendAndWait(message, Endpoint.TIMEOUT);
        }
        /// <summary>send message and wait reply
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeout">timeout in milliseconds</param>
        /// <returns></returns>
        public IDictionary<string, object> SendAndWait(IDictionary<string, object> message, int timeout)
        {
            return this.SendAndWait(null, message, timeout);
        }
        /// <summary>send message and wait reply
        /// </summary>
        /// <param name="sender">use to send, must belong this proxy</param>
        /// <param name="message"></param>
        /// <param name="timeout">timeout in milliseconds</param>
        /// <returns></returns>
        internal IDictionary<string, object> SendAndWait(IChannelSender sender, IDictionary<string, object> message, int timeout)
        {
            return this._handler.SendAndWait(this,
                    this.GetSender(sender),
                    this.CreateMessage(message),
                    timeout);
        }
        /// <summary>send message
        /// </summary>
        /// <param name="message"></param>
        public void Send(IDictionary<string, object> message)
        {
            this.Send(null, message);
        }
        /// <summary>send message
        /// </summary>
        /// <param name="sender">use to send, must belong this proxy</param>
        /// <param name="message"></param>
        internal void Send(IChannelSender sender, IDictionary<string, object> message)
        {
            this._handler.Send(this.CreateMessage(message), this.GetSender(sender));
        }

        private Message CreateMessage(IDictionary<string, object> message)
        {
            Message msg = new Message();
            msg.MessageType = MessageType.SEND;
            msg.Content = message;
            msg.Token = this.Token;
            return msg;
        }
        private IChannelSender GetSender(IChannelSender sender)
        {
            if (this._senders.Count == 0)
                throw new ChannelException(Text.E_NO_SENDER);
            if (this._senders.Contains(sender))
                return sender;
            return this._senders[this._random.Next(this._senders.Count)];
        }
    }
}
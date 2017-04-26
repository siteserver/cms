using System;
using System.Collections.Generic;
using Taobao.Top.Link.Channel;

namespace Taobao.Top.Link.Endpoints
{
    /// <summary>context for receiving message
    /// </summary>
    public class EndpointContext : EventArgs
    {
        private int _flag;
        private string _token;
        private EndpointHandler _handler;
        private ChannelContext _channelContext;

        /// <summary>get received message
        /// </summary>
        public IDictionary<string, object> Message { get; internal set; }
        /// <summary>get where the message sent from
        /// </summary>
        public Identity MessageFrom { get; internal set; }

        public EndpointContext(ChannelContext channelContext
            , EndpointHandler handler
            , Identity messageFrom
            , int flag
            , string token)
        {
            this._channelContext = channelContext;
            this._handler = handler;
            this.MessageFrom = messageFrom;
            this._flag = flag;
            this._token = token;
        }
        /// <summary>reply message
        /// </summary>
        /// <param name="message"></param>
        public void Reply(IDictionary<string, object> message)
        {
            this._handler.Send(this.CreateMessage(message), this._channelContext.Sender);
        }
        /// <summary>tell error occur
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="statusPhase"></param>
        public void Error(int statusCode, string statusPhase)
        {
            Message msg = this.CreateMessage(null);
            msg.StatusCode = statusCode;
            msg.StatusPhase = statusPhase;
            this._handler.Send(msg, this._channelContext.Sender);
        }

        private Message CreateMessage(IDictionary<string, object> message)
        {
            Message msg = new Message();
            msg.MessageType = MessageType.SENDACK;
            msg.Content = message;
            msg.Flag = this._flag;
            msg.Token = this._token;
            return msg;
        }
    }
}
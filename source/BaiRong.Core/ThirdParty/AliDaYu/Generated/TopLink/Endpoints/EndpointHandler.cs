using System;
using System.Collections.Generic;
using System.IO;
using Taobao.Top.Link.Channel;
using Top.Api;

namespace Taobao.Top.Link.Endpoints
{
    /// <summary>deal with protocol/callback/send
    /// </summary>
    public class EndpointHandler
    {
        private ITopLogger _log;
        private int _flag;
        //all connect in/out endpoints
        private IDictionary<string, Identity> _idByToken;
        private IDictionary<int, SendCallback> _callbacks;
        private EventHandler<ChannelContext> _onMessage;

        public Action<EndpointContext> MessageHandler { get; set; }
        public OnAckMessage AckMessageHandler { get; set; }

        public EndpointHandler(ITopLogger logger)
        {
            this._log = logger;
            this._idByToken = new Dictionary<string, Identity>();
            this._callbacks = new Dictionary<int, SendCallback>();
            this._onMessage = this.OnMessage;
        }

        public void Send(Message message, IChannelSender sender)
        {
            this.Send(message, sender, null);
        }
        public void Send(Message message, IChannelSender sender, SendCallback callback)
        {
            if (callback != null)
            {
                message.Flag = System.Threading.Interlocked.Increment(ref this._flag);
                this._callbacks.Add(message.Flag, callback);
            }
            using (var s = new MemoryStream())
            {
                MessageIO.WriteMessage(s, message);
                this.GetChannel(sender).Send(s.ToArray());
            }
        }

        internal IDictionary<string, object> SendAndWait(EndpointProxy e
            , IChannelSender sender
            , Message message
            , int timeout)
        {
            SendCallback callback = new SendCallback(e);
            this.Send(message, sender, callback);
            callback.WaitReturn(timeout);
            if (callback.Error != null)
                throw callback.Error;
            return callback.Return;
        }

        private IClientChannel GetChannel(IChannelSender sender)
        {
            var channel = sender as IClientChannel;
            if (channel.OnMessage == null)
                channel.OnMessage = this._onMessage;
            return channel;
        }
        private void OnMessage(object sender, ChannelContext ctx)
        {
            Message msg = MessageIO.ReadMessage(new MemoryStream((byte[])ctx.Message));
            SendCallback callback = this._callbacks.ContainsKey(msg.Flag)
                ? this._callbacks[msg.Flag]
                : null;

            if (msg.MessageType == MessageType.CONNECTACK)
            {
                this.HandleConnectAck(callback, msg);
                return;
            }

            Identity msgFrom = msg.Token != null && this._idByToken.ContainsKey(msg.Token)
                ? this._idByToken[msg.Token]
                : null;
            // must CONNECT/CONNECTACK for got token before SEND
            if (msgFrom == null)
            {
                var error = new LinkException(Text.E_UNKNOWN_MSG_FROM);
                if (callback == null)
                    throw error;
                callback.Error = error;
                return;
            }

            #region raise callback of client
            if (callback != null)
            {
                this.HandleCallback(callback, msg, msgFrom);
                return;
            }
            else if (this.IsError(msg))
            {
                this._log.Error(Text.E_GOT_ERROR, msg.StatusCode, msg.StatusPhase);
                return;
            }
            #endregion

            #region raise event
            if (msg.MessageType == MessageType.SENDACK)
            {
                if (this.AckMessageHandler != null)
                    this.AckMessageHandler(msg.Content, msgFrom);
                return;
            }

            if (this.MessageHandler == null)
                return;
            EndpointContext endpointContext = new EndpointContext(ctx, this, msgFrom, msg.Flag, msg.Token);
            endpointContext.Message = msg.Content;
            try
            {
                this.MessageHandler(endpointContext);
            }
            catch (Exception e)
            {
                // onMessage error should be reply to client
                if (e is LinkException)
                    endpointContext.Error(
                            ((LinkException)e).ErrorCode,
                            ((LinkException)e).Message);
                else
                    endpointContext.Error(0, e.Message);
            }
            #endregion
        }
        private void HandleConnectAck(SendCallback callback, Message msg)
        {
            if (callback == null)
                throw new LinkException(Text.E_NO_CALLBACK);
            if (this.IsError(msg))
                callback.Error = new LinkException(msg.StatusCode, msg.StatusPhase);
            else
            {
                callback.Return = null;
                // set token for proxy for sending message next time
                callback.Target.Token = msg.Token;
                // store token from target endpoint for receiving it's message
                // next time
                if (this._idByToken.ContainsKey(msg.Token))
                    this._idByToken[msg.Token] = callback.Target.Identity;
                else
                    this._idByToken.Add(msg.Token, callback.Target.Identity);

                this._log.Info(Text.E_CONNECT_SUCCESS, callback.Target.Identity, msg.Token);
            }
        }
        private void HandleCallback(SendCallback callback, Message msg, Identity msgFrom)
        {
            if (!callback.Target.Identity.Equals(msgFrom))
            {
                this._log.Warn(Text.E_IDENTITY_NOT_MATCH_WITH_CALLBACK, msgFrom, callback.Target.Identity);
                return;
            }
            if (this.IsError(msg))
                callback.Error = new LinkException(msg.StatusCode, msg.StatusPhase);
            else
                callback.Return = msg.Content;
        }
        private bool IsError(Message msg)
        {
            return msg.StatusCode > 0 || !string.IsNullOrEmpty(msg.StatusPhase);
        }

        public delegate void OnAckMessage(IDictionary<string, object> message, Identity messageFrom);
    }
}
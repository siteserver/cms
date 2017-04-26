using System;
using WebSocketSharp;
using WebSocketSharp.Frame;

namespace Taobao.Top.Link.Channel.WebSocket
{
    /// <summary>websocket clientchannel via websocket-sharp impl
    /// </summary>
    public class WebSocketClientChannel : IClientChannel
    {
        private WebSocketSharp.WebSocket _socket;
        private ResetableTimer _timer;
        private EventHandler<ChannelContext> _onMessage;
        private EventHandler<ChannelContext> _onError;
        private EventHandler<ChannelClosedEventArgs> _onClosed;

        public EventHandler<ChannelContext> OnMessage
        {
            get { this.DelayPing(); return this._onMessage; }
            set { this._onMessage = value; }
        }
        public EventHandler<ChannelContext> OnError
        {
            get { this.DelayPing(); return this._onError; }
            set { this._onError = value; }
        }
        public EventHandler<ChannelClosedEventArgs> OnClosed
        {
            get { return this._onClosed; }
            set { this._onClosed = value; }
        }

        public Uri Uri { get; set; }
        public bool IsConnected => this._socket.ReadyState == WsState.OPEN;

        public WebSocketClientChannel(WebSocketSharp.WebSocket socket)
        {
            this._socket = socket;
            this._onClosed += (o, e) =>
            {
                this.Close(e.Reason);
            };
        }

        public void Send(byte[] data)
        {
            this.CheckChannel();
            this._socket.Send(data);
        }

        public void Close(string reason)
        {
            this._socket.Close(CloseStatusCode.NORMAL, reason);
            if (this._timer != null)
            {
                this._timer.Cancel();
                this._timer = null;
#if DEBUG
                Console.WriteLine("TMC: Info@close: " + reason);
#endif
            }
        }

        public ResetableTimer HeartbeatTimer
        {
            set
            {
                this._timer = value;
                this._timer.Elapsed += (s, e) =>
                {
                    if (this.IsConnected)
                        //websocket-sharp's ping is sync
                        this._socket.Ping();
                };
            }
        }

        private void CheckChannel()
        {
            if (!this.IsConnected)
            {
                if (this._timer != null)
                    this._timer.Cancel();
                throw new LinkException("websocket channel closed");
            }
            this.DelayPing();
        }
        private void DelayPing()
        {
            try
            {
                if (this._timer != null)
                    this._timer.Delay();
            }
            catch (Exception)
            {

            }
        }
    }
}
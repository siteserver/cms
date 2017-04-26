using System.Collections.Generic;
using System.Net.Sockets;

namespace Taobao.Top.Link.Channel.TCP
{
    /// <summary>the channel that can send message to client via raw tcp
    /// </summary>
    public class TcpServerChannelSender : IServerChannelSender
    {
        private IDictionary<object, object> _context;
        TcpClient _tcpClient;

        public bool IsOpen => this._tcpClient.Connected;

        public TcpServerChannelSender(TcpClient tcpClient)
        {
            this._tcpClient = tcpClient;
            this._context = new Dictionary<object, object>();
        }

        public object GetContext(object key)
        {
            object val;
            return this._context.TryGetValue(key, out val) ? val : null;
        }
        public void SetContext(object key, object value)
        {
            this._context[key] = value;
        }

        public void Send(byte[] data)
        {
            this._tcpClient.GetStream().Write(data, 0, data.Length);
        }
        public void Close(string reason)
        {
            this._tcpClient.Close();
        }
    }
}
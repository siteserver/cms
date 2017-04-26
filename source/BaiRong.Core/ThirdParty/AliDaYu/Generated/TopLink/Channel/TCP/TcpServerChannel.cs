using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Top.Api;

namespace Taobao.Top.Link.Channel.TCP
{
    /// <summary>server channel bind on raw tcp, just impl an easy server
    /// </summary>
    public class TcpServerChannel : ServerChannel
    {
        private bool _running;
        private TcpListener _tcpListener;
        private Thread _acceptWorker;
        private IOWorker _ioWorker;
        private int _ioWorkerCount;
        private LinkedList<TcpClient> _tcpClients;

        public delegate void IOWorker(ITopLogger log, TcpServerChannel server, TcpClient tcpClient);

        /// <summary>init tcp server channel
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="port"></param>
        /// <param name="ioWorker">deal with networkstream</param>
        public TcpServerChannel(ITopLogger log
            , int port
            , IOWorker ioWorker)
            : base(log
            , port)
        {
            this._ioWorker = ioWorker;
            this._tcpClients = new LinkedList<TcpClient>();
        }

        public override void Start()
        {
            this._running = true;
            this._tcpListener = new TcpListener(IPAddress.Any, this.Port);
            this._tcpListener.Start();
            this._acceptWorker = new Thread(() =>
            {
                while (this._running)
                {
                    try
                    {
                        var client = this._tcpListener.AcceptTcpClient();
                        this._tcpClients.AddLast(client);
                        ThreadPool.QueueUserWorkItem((state) =>
                        {
                            try { this.AcceptSocket(client); }
                            catch (Exception e)
                            {
                                this.InternalOnError(e);
                                this._tcpClients.Remove(client);
                            }
                        });
                    }
                    catch (SocketException) { break; }
                    catch (Exception e)
                    {
                        this.InternalOnError(e);
                        break;
                    }
                }
            });
            this._acceptWorker.IsBackground = true;
            this._acceptWorker.Start();
        }
        public override void Stop()
        {
            this._running = false;
            this._acceptWorker.Abort();
            this._tcpListener.Stop();
        }

        private void AcceptSocket(TcpClient client)
        {
            if (this._ioWorker == null)
                return;
            var logName = "IO-Worker#" + (++this._ioWorkerCount);
            ThreadPool.QueueUserWorkItem(o => 
                this._ioWorker(new DefaultTopLogger(logName), this, client));
        }
        private void InternalOnError(Exception e)
        {
            if (this.OnError != null)
                this.OnError(this, new ChannelContext(e));
        }
    }
}
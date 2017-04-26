using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Taobao.Top.Link.Channel;
using Taobao.Top.Link.Endpoints;
using Taobao.Top.Link.Util;
using Top.Api;
using Top.Api.Util;

namespace Top.Tmc
{
    /// <summary>消息服务客户端</summary>
    public class TmcClient
    {
        // sign parameters
        private const string GROUP_NAME = "group_name";
        private const string SDK = "sdk";
        private const string INTRANET_IP = "intranet_ip";

        private TmcClientIdentity _id;
        private string _appSecret;
        private string _uri;
        private Endpoint _endpoint;
        private EndpointProxy _serverProxy;

        private volatile bool running;

        private int _heartbeatInterval = 45000; // 心跳频率（单位：毫秒）
        private int _reconnectIntervalSeconds = 15; // 重连周期（单位：秒）
        private int _pullRequestIntervalSeconds = 30; // 定时获取消息周期（单位：秒）
        private Timer _reconnectTimer;
        private Timer _pullRequestTimer;

        public event EventHandler<MessageArgs> OnMessage;

        /// <summary>获取或设置Log</summary>
        public ITopLogger Log { get; set; }

        /// <summary>获取或设置定时发送拉取请求的周期（单位：秒）</summary>
        public int PullRequestIntervalSeconds
        {
            get { return this._pullRequestIntervalSeconds; }
            set
            {
                this._pullRequestIntervalSeconds = value;
                if (this._pullRequestTimer != null)
                    this._pullRequestTimer.Change(TimeSpan.FromSeconds(this._pullRequestIntervalSeconds)
                        , TimeSpan.FromSeconds(this._pullRequestIntervalSeconds));
            }
        }

        /// <summary>获取或设置自动重连间隔（单位：秒）</summary>
        public int ReconnectIntervalSeconds
        {
            get { return this._reconnectIntervalSeconds; }
            set
            {
                this._reconnectIntervalSeconds = value;
                if (this._reconnectTimer != null)
                    this._reconnectTimer.Change(TimeSpan.FromSeconds(this._reconnectIntervalSeconds)
                        , TimeSpan.FromSeconds(this._reconnectIntervalSeconds));
            }
        }

        /// <summary>以默认分组，初始化TMC客户端</summary>
        public TmcClient(string appKey, string appSecret) : this(appKey, appSecret, "default") { }

        /// <summary>初始化TMC客户端</summary>
        public TmcClient(string appKey, string appSecret, string groupName)
        {
            this._appSecret = appSecret;
            this._id = new TmcClientIdentity(appKey, groupName);
            this.PrepareEndpoint();
        }

        /// <summary>连接到 TMC Server</summary>
        /// <param name="uri">TMC server address, eg: ws://mc.api.taobao.com/</param>
        public void Connect(string uri)
        {
            this.running = true;
            doConnect(uri);
            this.StartReconnect();
            this.StartPullRequest();
        }

        private void doConnect(string uri)
        {
            var signHeader = new Dictionary<string, string>();
            var connHeader = new Dictionary<string, object>();
            signHeader.Add(Constants.APP_KEY, this._id.AppKey);
            connHeader.Add(Constants.APP_KEY, signHeader[Constants.APP_KEY]);

            signHeader.Add(GROUP_NAME, this._id.GroupName);
            connHeader.Add(GROUP_NAME, signHeader[GROUP_NAME]);

            signHeader.Add(Constants.TIMESTAMP, DateTime.Now.Ticks.ToString());
            connHeader.Add(Constants.TIMESTAMP, signHeader[Constants.TIMESTAMP]);

            connHeader.Add(Constants.SIGN, TopUtils.SignTopRequest(signHeader, this._appSecret, Constants.SIGN_METHOD_MD5));
            //extra fields
            connHeader.Add(SDK, Constants.SDK_VERSION);
            connHeader.Add(INTRANET_IP, TopUtils.GetIntranetIp());
            this._serverProxy = this._endpoint.GetEndpoint(new TmcServerIdentity(), uri, connHeader);
            this._uri = uri;
            this.Log.Info("connected to tmc server: {0}", uri);
        }

        /// <summary>向指定的主题发布一条与用户无关的消息。</summary>
        /// <param name="topic">主题名称</param>
        /// <param name="content">严格根据主题定义的消息内容（JSON/XML）</param>
        public void Send(string topic, string content)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentNullException("topic");
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException("content");

            IDictionary<string, object> msg = new Dictionary<string, object>();
            msg.Add(MessageFields.KIND, MessageKind.Data);
            msg.Add(MessageFields.DATA_TOPIC, topic);
            msg.Add(MessageFields.DATA_CONTENT, content);
            this._serverProxy.SendAndWait(msg, 2000);
        }

        /// <summary>向指定的主题发布一条与用户相关的消息。</summary>
        /// <param name="topic">主题名称</param>
        /// <param name="content">严格根据主题定义的消息内容（JSON/XML）</param>
        /// <param name="session">用户授权码</param>
        public void Send(string topic, string content, string session)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentNullException("topic");
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException("content");
            if (string.IsNullOrEmpty(session))
                throw new ArgumentNullException("session");

            IDictionary<string, object> msg = new Dictionary<string, object>();
            msg.Add(MessageFields.KIND, MessageKind.Data);
            msg.Add(MessageFields.DATA_TOPIC, topic);
            msg.Add(MessageFields.DATA_CONTENT, content);
            msg.Add(MessageFields.DATA_INCOMING_USER_SESSION, session);
            this._serverProxy.SendAndWait(msg, 2000);
        }

        /// <summary>向服务端发送消息拉取请求</summary>
        protected internal void PullRequest()
        {
            IDictionary<string, object> msg = new Dictionary<string, object>();
            msg.Add(MessageFields.KIND, MessageKind.PullRequest);
            this._serverProxy.Send(msg);
        }

        /// <summary>确认消息</summary>
        protected internal void Confirm(Message message)
        {
            IDictionary<string, object> msg = new Dictionary<string, object>();
            msg.Add(MessageFields.KIND, MessageKind.Confirm);
            msg.Add(MessageFields.CONFIRM_ID, message.Id);
            this._serverProxy.Send(msg);
        }

        private void PrepareEndpoint()
        {
            this.Log = new DefaultTopLogger("TmcClient");
            this._endpoint = new Endpoint(Log, this._id);
            this._endpoint.ChannelSelector = new ClientChannelSharedSelector(Log) { HeartbeatPeriod = this._heartbeatInterval };
            this._endpoint.OnMessage += InternalOnMessage;
            this._endpoint.OnAckMessage += InternalOnAckMessage;
        }

        private void InternalOnMessage(object sender, EndpointContext context)
        {
            if (this.Log.IsDebugEnabled())
                this.Log.Debug("messsage from {0}: {1}", context.MessageFrom, this.Dump(context.Message));
            if (this.OnMessage == null)
                return;

            ThreadPool.QueueUserWorkItem(o =>
            {
                if (!this.running)
                {
                    if (this.Log.IsDebugEnabled())
                        this.Log.Debug(string.Format("message dropped as client closed: {0}", this.Dump(context.Message)));
                    return;
                }

                Message msg = this.ParseMessage(context.Message);
                var args = new MessageArgs(msg, m => this.Confirm(m));
                var sw = new Stopwatch();
                try
                {
                    sw.Start();
                    this.OnMessage(this, args);
                    sw.Stop();
                }
                catch (Exception e)
                {
                    args.Fail(e.Message);
                }

                if (args._isFail)
                {
                    if (this.Log.IsDebugEnabled())
                        this.Log.Debug("process message error: {0}", args._reason);
                    return;
                }

                // prevent confirm attach
                if (sw.ElapsedMilliseconds <= 1)
                {
                    if (this.Log.IsDebugEnabled())
                        this.Log.Debug("maybe too fast or attack? server maybe reject your request");
                    Thread.Sleep(10);
                }

                if (args._isConfirmed)
                    return;
                try
                {
                    this.Confirm(msg);
                    if (this.Log.IsDebugEnabled())
                        this.Log.Debug("confirm message: {0}", this.Dump(context.Message));
                }
                catch (Exception e)
                {
                    this.Log.Warn(string.Format("confirm message {0} error {1}", this.Dump(context.Message), e.StackTrace));
                }
            });
        }

        private void InternalOnAckMessage(object sender, AckMessageArgs e)
        {
            if (this.Log.IsDebugEnabled())
                this.Log.Debug("ack messsage from {0}: {1}", e.MessageFrom, e.Message);
        }

        private void StartReconnect()
        {
            if (this._reconnectTimer != null) return;
            this._reconnectTimer = new Timer(o =>
            {
                try
                {
                    if (!this._serverProxy.hasValidSender())
                    {
                        this.Log.Info("reconning...");
                        this.doConnect(this._uri);
                    }
                }
                catch (Exception e)
                {
                    this.Log.Warn("reconnect error", e);
                }
            }, null
            , TimeSpan.FromSeconds(this._reconnectIntervalSeconds)
            , TimeSpan.FromSeconds(this._reconnectIntervalSeconds));
        }

        private void StartPullRequest()
        {
            if (this._pullRequestTimer != null) return;
            this._pullRequestTimer = new Timer(o =>
            {
                try
                {
                    if (this._serverProxy.hasValidSender())
                    {
                        this.PullRequest();
                    }
                }
                catch (Exception e)
                {
                    this.Log.Warn("pull request error", e);
                }
            }
            , null
            , TimeSpan.FromMilliseconds(500)
            , TimeSpan.FromSeconds(this.PullRequestIntervalSeconds));
        }

        private Message ParseMessage(IDictionary<string, object> raw)
        {
            var msg = new Message();
            msg.Id = this.GetValue<long>(raw, MessageFields.OUTGOING_ID);
            msg.Topic = this.GetValue<string>(raw, MessageFields.DATA_TOPIC);
            msg.PubAppKey = this.GetValue<string>(raw, MessageFields.DATA_OUTGOING_PUBLISHER);
            msg.PubTime = this.GetValue<DateTime>(raw, MessageFields.DATA_PUBLISH_TIME);
            msg.UserId = this.GetValue<long>(raw, MessageFields.DATA_OUTGOING_USER_ID);
            msg.UserNick = this.GetValue<string>(raw, MessageFields.DATA_OUTGOING_USER_NICK);
            msg.OutgoingTime = this.GetValue<DateTime>(raw, MessageFields.DATA_ATTACH_OUTGOING_TIME);

            if (!raw.ContainsKey(MessageFields.DATA_CONTENT))
                return msg;
            msg.Content = raw[MessageFields.DATA_CONTENT] is byte[]
                ? Encoding.UTF8.GetString(GZIPHelper.Unzip(raw[MessageFields.DATA_CONTENT] as byte[]))
                : (string)raw[MessageFields.DATA_CONTENT];

            return msg;
        }

        private T GetValue<T>(IDictionary<string, object> raw, string key)
        {
            if (raw.ContainsKey(key))
            {
                object value = raw[key];
                if (value != null)
                {
                    return (T)value;
                }
            }
            return default(T);
        }

        private string Dump(IDictionary<string, object> raw)
        {
            var buf = new StringBuilder();
            foreach (var i in raw)
                buf.AppendFormat("{0}={1}|", i.Key, i.Value);
            return buf.ToString();
        }

        public void Close()
        {
            this.running = false;
            if (this._pullRequestTimer != null)
            {
                this._pullRequestTimer.Dispose();
                this._pullRequestTimer = null;
            }
            if (this._reconnectTimer != null)
            {
                this._reconnectTimer.Dispose();
                this._reconnectTimer = null;
            }
            this._serverProxy.Close(this._uri, "client closed");
            this.Log.Warn("tmc client closed");
        }

        public bool Online => this._serverProxy != null && this._serverProxy.hasValidSender();
    }
}
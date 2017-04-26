using System;

namespace Top.Tmc
{
    /// <summary>
    /// 消息服务-通用消息结构。
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>消息标识</summary>
        public long Id { get; protected internal set; }
        /// <summary>主题</summary>
        public string Topic { get; set; }
        /// <summary>发布者标识</summary>
        public string PubAppKey { get; set; }
        /// <summary>发布时间</summary>
        public DateTime PubTime { get; set; }
        /// <summary>从服务器发送时间</summary>
        public DateTime OutgoingTime { get; set; }
        /// <summary>消息所属的用户ID，若不是用户相关消息则为空</summary>
        public Nullable<long> UserId { get; set; }
        /// <summary>消息所属的用户昵称，若不是用户相关消息则为空</summary>
        public string UserNick { get; set; }
        /// <summary>消息的业务具体内容（JSON结构）</summary>
        public string Content { get; set; }
    }
}
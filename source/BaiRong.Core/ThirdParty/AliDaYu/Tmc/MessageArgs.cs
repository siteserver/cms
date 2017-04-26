using System;

namespace Top.Tmc
{
    public class MessageArgs : EventArgs
    {
        protected internal bool _isFail;
        protected internal string _reason;
        protected internal bool _isConfirmed;
        private Action<Message> confirm;

        /// <summary>获取消息
        /// </summary>
        public Message Message { get; private set; }

        public MessageArgs(Message message, Action<Message> confirm)
        {
            this.Message = message;
            this.confirm = confirm;
        }
        /// <summary>将当前消息处理设置为失败，若该消息启用了重试，失败的消息将会再指定时间内重新发送
        /// </summary>
        public void Fail()
        {
            this.Fail(string.Empty);
        }
        /// <summary>将当前消息处理设置为失败，若该消息启用了重试，失败的消息将会再指定时间内重新发送
        /// <param name="reason">指定失败原因</param>
        /// </summary>
        public void Fail(string reason)
        {
            this._reason = reason;
            this._isFail = true;
        }

        /// <summary>
        /// 对当前消息进行确认
        /// </summary>
        public void Confirm()
        {
            this.confirm(this.Message);
            this._isConfirmed = true;
        }
    }
}
namespace Taobao.Top.Link.Channel
{
    /// <summary>a channel used to sending message
    /// </summary>
    public interface IChannelSender
    {
        /// <summary>send bytes
        /// </summary>
        /// <param name="data"></param>
        void Send(byte[] data);
        /// <summary>close channel with given reason
        /// </summary>
        /// <param name="reason"></param>
        void Close(string reason);
    }
}

namespace Taobao.Top.Link.Channel
{
    /// <summary>the channel that can send message to client
    /// </summary>
    public interface IServerChannelSender : IChannelSender
    {
        /// <summary>weather channel is open
        /// </summary>
        bool IsOpen { get; }
        /// <summary>get channel context by given object key, channel conext can be used to store something that belong itself
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetContext(object key);
        /// <summary>set channel context by given object key, channel conext can be used to store something that belong itself
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetContext(object key, object value);
    }
}
using System;

namespace Taobao.Top.Link.Channel
{
    /// <summary>channel holder/pool that switch channel by uri.scheme
    /// </summary>
    public interface IClientChannelSelector
    {
        /// <summary>get channel
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        IClientChannel GetChannel(Uri uri);
        /// <summary>return channel
        /// </summary>
        /// <param name="channel"></param>
        void ReturnChannel(IClientChannel channel);
    }
}
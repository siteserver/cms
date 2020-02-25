using System;

namespace SS.CMS.Abstractions
{
    /// <summary>
    /// 为内容操作事件提供数据
    /// </summary>
    public class ContentEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SS.CMS.Abstractions.ContentEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentId">内容Id。</param>
        public ContentEventArgs(int siteId, int channelId, int contentId)
        {
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
        }

        /// <summary>
        /// 内容的站点Id。
        /// </summary>
        public int SiteId { get; }

        /// <summary>
        /// 内容的栏目Id。
        /// </summary>
        public int ChannelId { get; }

        /// <summary>
        /// 内容Id。
        /// </summary>
        public int ContentId { get; }
    }
}
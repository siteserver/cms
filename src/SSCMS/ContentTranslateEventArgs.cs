using System;

namespace SSCMS
{
    /// <summary>
    /// 为内容转移事件提供数据
    /// </summary>
    public class ContentTranslateEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SSCMS.ContentTranslateEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="siteId">原始内容的站点Id。</param>
        /// <param name="channelId">原始内容的栏目Id。</param>
        /// <param name="contentId">原始内容的Id。</param>
        /// <param name="targetSiteId">转移后内容的站点Id。</param>
        /// <param name="targetChannelId">转移后内容的栏目Id。</param>
        /// <param name="targetContentId">转移后内容的Id。</param>
        public ContentTranslateEventArgs(int siteId, int channelId, int contentId, int targetSiteId,
            int targetChannelId, int targetContentId)
        {
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            TargetSiteId = targetSiteId;
            TargetChannelId = targetChannelId;
            TargetContentId = targetContentId;
        }

        /// <summary>
        /// 原始内容的站点Id。
        /// </summary>
        public int SiteId { get; }

        /// <summary>
        /// 原始内容的栏目Id。
        /// </summary>
        public int ChannelId { get; }

        /// <summary>
        /// 原始内容的Id。
        /// </summary>
        public int ContentId { get; }

        /// <summary>
        /// 转移后内容的站点Id。
        /// </summary>
        public int TargetSiteId { get; }

        /// <summary>
        /// 转移后内容的栏目Id。
        /// </summary>
        public int TargetChannelId { get; }

        /// <summary>
        /// 转移后内容的Id。
        /// </summary>
        public int TargetContentId { get; }
    }
}
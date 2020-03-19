namespace SSCMS
{
    /// <summary>
    /// 内容事件关联的上下文。
    /// </summary>
    public interface IContentContext
    {
        /// <summary>
        /// 内容所属的站点Id。
        /// </summary>
        int SiteId { get; }

        /// <summary>
        /// 内容所属的栏目Id。
        /// </summary>
        int ChannelId { get; }

        /// <summary>
        /// 内容Id。
        /// </summary>
        int ContentId { get; }
    }
}

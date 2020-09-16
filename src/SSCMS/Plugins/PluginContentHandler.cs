using System.Threading.Tasks;

namespace SSCMS.Plugins
{
    public abstract class PluginContentHandler : IPluginExtension
    {
        public PluginContentHandler()
        {

        }

        public virtual void OnDeleted(int siteId, int channelId, int contentId)
        {

        }

        public virtual Task OnDeletedAsync(int siteId, int channelId, int contentId) => Task.CompletedTask;

        /// <summary>
        /// 转移后触发
        /// </summary>
        /// <param name="siteId">原始内容的站点Id。</param>
        /// <param name="channelId">原始内容的栏目Id。</param>
        /// <param name="contentId">原始内容的Id。</param>
        /// <param name="targetSiteId">转移后内容的站点Id。</param>
        /// <param name="targetChannelId">转移后内容的栏目Id。</param>
        /// <param name="targetContentId">转移后内容的Id。</param>
        public virtual void OnTranslated(int siteId, int channelId, int contentId, int targetSiteId,
            int targetChannelId, int targetContentId)
        {

        }

        /// <summary>
        /// 转移后触发
        /// </summary>
        /// <param name="siteId">原始内容的站点Id。</param>
        /// <param name="channelId">原始内容的栏目Id。</param>
        /// <param name="contentId">原始内容的Id。</param>
        /// <param name="targetSiteId">转移后内容的站点Id。</param>
        /// <param name="targetChannelId">转移后内容的栏目Id。</param>
        /// <param name="targetContentId">转移后内容的Id。</param>
        public virtual Task OnTranslatedAsync(int siteId, int channelId, int contentId, int targetSiteId,
            int targetChannelId, int targetContentId) => Task.CompletedTask;
    }
}

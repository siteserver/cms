using System.Collections;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPathManager
    {
        Task<string> GetContentFilePathRuleAsync(SiteInfo siteInfo, int channelId);

        Task<IDictionary> ContentRulesGetDictionaryAsync(IPluginManager pluginManager, SiteInfo siteInfo, int channelId);

        Task<string> ContentRulesParseAsync(SiteInfo siteInfo, int channelId, int contentId);

        Task<string> ContentRulesParseAsync(SiteInfo siteInfo, int channelId, ContentInfo contentInfo);
    }
}
using System.Collections;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPathManager
    {
        Task<string> GetContentFilePathRuleAsync(Site siteInfo, int channelId);

        Task<IDictionary> ContentRulesGetDictionaryAsync(IPluginManager pluginManager, Site siteInfo, int channelId);

        Task<string> ContentRulesParseAsync(Site siteInfo, int channelId, int contentId);

        Task<string> ContentRulesParseAsync(Site siteInfo, int channelId, Content contentInfo);
    }
}
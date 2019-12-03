using System.Collections;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IPathManager
    {
        Task<string> GetContentFilePathRuleAsync(Site siteInfo, int channelId);

        Task<IDictionary> ContentRulesGetDictionaryAsync(IPluginManager pluginManager, Site siteInfo, int channelId);

        Task<string> ContentRulesParseAsync(Site siteInfo, int channelId, int contentId);

        Task<string> ContentRulesParseAsync(Site siteInfo, int channelId, Content contentInfo);
    }
}
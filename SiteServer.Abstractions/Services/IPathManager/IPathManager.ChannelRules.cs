using System.Collections;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IPathManager
    {
        Task<string> GetChannelFilePathRuleAsync(Site siteInfo, int channelId);

        Task<IDictionary> ChannelRulesGetDictionaryAsync(Site siteInfo, int channelId);

        Task<string> ChannelRulesParseAsync(Site siteInfo, int channelId);
    }
}
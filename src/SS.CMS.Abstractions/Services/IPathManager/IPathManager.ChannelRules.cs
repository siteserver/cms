using System.Collections;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPathManager
    {
        Task<string> GetChannelFilePathRuleAsync(SiteInfo siteInfo, int channelId);

        Task<IDictionary> ChannelRulesGetDictionaryAsync(SiteInfo siteInfo, int channelId);

        Task<string> ChannelRulesParseAsync(SiteInfo siteInfo, int channelId);
    }
}
using System.Collections;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPathManager
    {
        IDictionary ContentRulesGetDictionary(IPluginManager pluginManager, SiteInfo siteInfo, int channelId);

        string ContentRulesParse(SiteInfo siteInfo, int channelId, int contentId);

        string ContentRulesParse(SiteInfo siteInfo, int channelId, ContentInfo contentInfo);
    }
}
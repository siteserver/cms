using System.Collections;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPathManager
    {
        IDictionary ContentRulesGetDictionary(IPluginManager pluginManager, ITableStyleRepository tableStyleRepository, SiteInfo siteInfo, int channelId);

        string ContentRulesParse(SiteInfo siteInfo, int channelId, int contentId);

        string ContentRulesParse(SiteInfo siteInfo, int channelId, ContentInfo contentInfo);
    }
}
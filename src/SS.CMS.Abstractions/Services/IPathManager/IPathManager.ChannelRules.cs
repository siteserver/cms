using System.Collections;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPathManager
    {
        IDictionary ChannelRulesGetDictionary(ITableStyleRepository tableStyleRepository, SiteInfo siteInfo, int channelId);

        string ChannelRulesParse(SiteInfo siteInfo, int channelId);
    }
}
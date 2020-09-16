using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Context
{
    public class PluginParseContext : IParseContext
    {
        protected readonly IParseManager ParseManager;

        public PluginParseContext(IParseManager parseManager)
        {
            ParseManager = parseManager;
        }

        public void Set<T>(string key, T objectValue)
        {
            if (!string.IsNullOrEmpty(key))
            {
                ParseManager.PageInfo.PluginItems[key] = objectValue;
            }
        }

        public T Get<T>(string key)
        {
            return TranslateUtils.Get<T>(ParseManager.PageInfo.PluginItems, key);
        }

        public int SiteId => ParseManager.ContextInfo.Site.Id;

        public int ChannelId => ParseManager.ContextInfo.ChannelId;

        public int ContentId => ParseManager.ContextInfo.ContentId;

        public TemplateType TemplateType => ParseManager.PageInfo.Template.TemplateType;

        public int TemplateId => ParseManager.PageInfo.Template.Id;

        public SortedDictionary<string, string> HeadCodes => ParseManager.PageInfo.HeadCodes;

        public SortedDictionary<string, string> BodyCodes => ParseManager.PageInfo.BodyCodes;

        public SortedDictionary<string, string> FootCodes => ParseManager.PageInfo.FootCodes;

        public async Task<string> ParseAsync(string template)
        {
            var builder = new StringBuilder(template);
            await ParseManager.ParseInnerContentAsync(builder);
            return builder.ToString();
        }
    }
}

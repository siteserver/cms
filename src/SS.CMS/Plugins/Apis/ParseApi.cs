using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SS.CMS.Plugins.Impl;


namespace SiteServer.CMS.Plugin.Apis
{
    public class ParseApi
    {
        private ParseApi() { }

        private static ParseApi _instance;
        public static ParseApi Instance => _instance ??= new ParseApi();

        public Dictionary<string, string> GetStlElements(string html, List<string> stlElementNames)
        {
            return StlParserUtility.GetStlElements(html, stlElementNames);
        }

        public async Task<string> ParseAsync(string html, IParseContext context)
        {
            return await StlParserManager.ParseInnerContentAsync(html, (ParseContextImpl)context);
        }

        public async Task<string> ParseAttributeValueAsync(string attributeValue, IParseContext context)
        {
            var site = await DataProvider.SiteRepository.GetAsync(context.SiteId);
            var templateInfo = new Template
            {
                Id = context.TemplateId,
                TemplateType = context.TemplateType
            };
            var pageInfo = await PageInfo.GetPageInfoAsync(context.ChannelId, context.ContentId, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);
            return await StlEntityParser.ReplaceStlEntitiesForAttributeValueAsync(attributeValue, pageInfo, contextInfo);
        }

        public async Task<string> GetCurrentUrlAsync(IParseContext context)
        {
            var site = await DataProvider.SiteRepository.GetAsync(context.SiteId);
            return await StlParserUtility.GetStlCurrentUrlAsync(site, context.ChannelId, context.ContentId,
                (Content)context.ContentInfo, context.TemplateType, context.TemplateId, false);
        }
    }
}

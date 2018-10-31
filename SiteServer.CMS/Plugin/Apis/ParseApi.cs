using System.Collections.Generic;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ParseApi : IParseApi
    {
        private ParseApi() { }

        private static ParseApi _instance;
        public static ParseApi Instance => _instance ?? (_instance = new ParseApi());

        public Dictionary<string, string> GetStlElements(string html, List<string> stlElementNames)
        {
            return StlParserUtility.GetStlElements(html, stlElementNames);
        }

        public string Parse(string html, IParseContext context)
        {
            return StlParserManager.ParseInnerContent(html, (ParseContextImpl)context);
        }

        public string ParseAttributeValue(string attributeValue, IParseContext context)
        {
            var siteInfo = SiteManager.GetSiteInfo(context.SiteId);
            var templateInfo = new TemplateInfo
            {
                Id = context.TemplateId,
                TemplateType = context.TemplateType
            };
            var pageInfo = new PageInfo(context.ChannelId, context.ContentId, siteInfo, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);
            return StlEntityParser.ReplaceStlEntitiesForAttributeValue(attributeValue, pageInfo, contextInfo);
        }

        public string GetCurrentUrl(IParseContext context)
        {
            var siteInfo = SiteManager.GetSiteInfo(context.SiteId);
            return StlParserUtility.GetStlCurrentUrl(siteInfo, context.ChannelId, context.ContentId,
                context.ContentInfo, context.TemplateType, context.TemplateId, false);
        }
    }
}

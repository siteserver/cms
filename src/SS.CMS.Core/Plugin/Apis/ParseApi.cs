using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Components;
using SS.CMS.Core.Models;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Parsers;
using SS.CMS.Core.StlParser.Utility;

namespace SS.CMS.Core.Plugin.Apis
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
                Type = context.TemplateType
            };
            var pageInfo = new PageInfo(context.ChannelId, context.ContentId, siteInfo, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);
            return StlEntityParser.ReplaceStlEntitiesForAttributeValue(attributeValue, pageInfo, contextInfo);
        }

        public string GetCurrentUrl(IParseContext context)
        {
            var siteInfo = SiteManager.GetSiteInfo(context.SiteId);
            return StlParserUtility.GetStlCurrentUrl(siteInfo, context.ChannelId, context.ContentId,
                (ContentInfo)context.ContentInfo, context.TemplateType, context.TemplateId, false);
        }
    }
}

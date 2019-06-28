using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Core.Plugin
{
    public class ParseContextImpl : IParseContext
    {
        public async Task LoadAsync(string stlOuterHtml, string stlInnerHtml, NameValueCollection stlAttributes, PageInfo pageInfo, ParseContext parseContext)
        {
            SiteId = parseContext.SiteInfo.Id;
            ChannelId = parseContext.ChannelId;
            ContentId = parseContext.ContentId;
            ContentInfo = await parseContext.GetContentInfoAsync();
            TemplateType = pageInfo.TemplateInfo.Type;
            TemplateId = pageInfo.TemplateInfo.Id;

            HeadCodes = pageInfo.HeadCodes;
            BodyCodes = pageInfo.BodyCodes;
            FootCodes = pageInfo.FootCodes;
            StlOuterHtml = stlOuterHtml;
            StlInnerHtml = stlInnerHtml;
            StlAttributes = stlAttributes;

            IsStlElement = !parseContext.IsStlEntity;
            PluginItems = pageInfo.PluginItems;
        }

        public Dictionary<string, object> PluginItems { get; private set; }

        public void Set<T>(string key, T objectValue)
        {
            if (PluginItems != null && !string.IsNullOrEmpty(key))
            {
                PluginItems[key] = objectValue;
            }
        }

        public T Get<T>(string key)
        {
            object objectValue;
            if (PluginItems.TryGetValue(key, out objectValue))
            {
                if (objectValue is T)
                {
                    return (T)objectValue;
                }
            }

            return default(T);
        }

        public int SiteId { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public ContentInfo ContentInfo { get; private set; }

        public TemplateType TemplateType { get; private set; }

        public int TemplateId { get; private set; }

        public SortedDictionary<string, string> HeadCodes { get; private set; }

        public SortedDictionary<string, string> BodyCodes { get; private set; }

        public SortedDictionary<string, string> FootCodes { get; private set; }

        public string StlOuterHtml { get; private set; }

        public string StlInnerHtml { get; private set; }

        public NameValueCollection StlAttributes { get; private set; }

        public bool IsStlElement { get; private set; }
    }
}

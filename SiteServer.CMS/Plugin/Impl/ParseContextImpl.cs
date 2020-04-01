using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Impl
{
    public class ParseContextImpl: IParseContext
    {
        public ParseContextImpl(string stlOuterHtml, string stlInnerHtml, NameValueCollection stlAttributes, PageInfo pageInfo, ContextInfo contextInfo)
        {
            SiteId = contextInfo.SiteInfo.Id;
            ChannelId = contextInfo.ChannelId;
            ContentId = contextInfo.ContentId;
            ContentInfo = contextInfo.ContentInfo;
            TemplateType = pageInfo.TemplateInfo.TemplateType;
            TemplateId = pageInfo.TemplateInfo.Id;

            HeadCodes = pageInfo.HeadCodes;
            BodyCodes = pageInfo.BodyCodes;
            FootCodes = pageInfo.FootCodes;
            StlOuterHtml = stlOuterHtml;
            StlInnerHtml = stlInnerHtml;
            StlAttributes = stlAttributes;
            
            IsStlElement = !contextInfo.IsStlEntity;
            PluginItems = pageInfo.PluginItems;
        }

        public Dictionary<string, object> PluginItems { get; }

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
                    return (T) objectValue;
                }
            }

            return default(T);
        }

        public int SiteId  { get; }

        public int ChannelId  { get; }

        public int ContentId { get; }

        public IContentInfo ContentInfo { get; }

        public TemplateType TemplateType { get; }

        public int TemplateId { get; }

        public SortedDictionary<string, string> HeadCodes { get; }

        public SortedDictionary<string, string> BodyCodes { get; }

        public SortedDictionary<string, string> FootCodes { get; }

        public string StlOuterHtml { get; }

        public string StlInnerHtml { get; }

        public NameValueCollection StlAttributes { get; }

        public bool IsStlElement { get; }
    }
}

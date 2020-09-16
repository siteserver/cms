using System.Collections.Specialized;
using SSCMS.Models;

namespace SSCMS.Parse
{
    public class ParseContext
    {
        public ParseContext(ParsePage pageInfo)
        {
            Site = pageInfo.Site;
            ChannelId = pageInfo.PageChannelId;
            ContentId = pageInfo.PageContentId;
        }

        //用于clone
        private ParseContext(ParseContext contextInfo)
        {
            ContextType = contextInfo.ContextType;
            Site = contextInfo.Site;
            ChannelId = contextInfo.ChannelId;
            ContentId = contextInfo.ContentId;
            Channel = contextInfo.Channel;
            Content = contextInfo.Content;

            IsInnerElement = contextInfo.IsInnerElement;
            IsStlEntity = contextInfo.IsStlEntity;
            PageItemIndex = contextInfo.PageItemIndex;
            ItemContainer = contextInfo.ItemContainer;
            ContainerClientId = contextInfo.ContainerClientId;

            OuterHtml = contextInfo.OuterHtml;
            InnerHtml = contextInfo.InnerHtml;
            Attributes = contextInfo.Attributes;
        }

        public ParseContext Clone(string outerHtml, string innerHtml, NameValueCollection attributes)
        {
            var contextInfo = new ParseContext(this)
            {
                OuterHtml = outerHtml,
                InnerHtml = innerHtml,
                Attributes = attributes
            };
            return contextInfo;
        }

        public ParseContext Clone()
        {
            var contextInfo = new ParseContext(this);
            return contextInfo;
        }

        public ParseType ContextType { get; set; } = ParseType.Undefined;

        public Site Site { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string OuterHtml { get; set; }

        public string InnerHtml { get; set; }

        public NameValueCollection Attributes { get; set; }

        public Channel Channel { get; set; }
        
        public void SetChannel(Channel value)
        {
            Channel = value;
        }

        public Content Content { get; set; }

        public void SetContent(Content value)
        {
            Content = value;
        }

        public bool IsInnerElement { get; set; }

        public bool IsStlEntity { get; set; }

        public int PageItemIndex { get; set; }

        public DbItemContainer ItemContainer { get; set; }

        public string ContainerClientId { get; set; }
    }
}

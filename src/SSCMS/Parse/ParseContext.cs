using System.Collections.Specialized;
using SSCMS.Models;

namespace SSCMS.Parse
{
    public class ParseContext
    {
        public ParseContext(ParsePage page)
        {
            Site = page.Site;
            ChannelId = page.PageChannelId;
            ContentId = page.PageContentId;
        }

        //用于clone
        private ParseContext(ParseContext context)
        {
            ContextType = context.ContextType;
            Site = context.Site;
            ChannelId = context.ChannelId;
            ContentId = context.ContentId;
            Channel = context.Channel;
            Content = context.Content;

            IsInnerElement = context.IsInnerElement;
            IsStlEntity = context.IsStlEntity;
            PageItemIndex = context.PageItemIndex;
            ItemContainer = context.ItemContainer;
            ContainerClientId = context.ContainerClientId;

            ElementName = context.ElementName;
            OuterHtml = context.OuterHtml;
            InnerHtml = context.InnerHtml;
            Attributes = context.Attributes;
            StartIndex = context.StartIndex;
        }

        public ParseContext Clone(string elementName, string outerHtml, string innerHtml, NameValueCollection attributes, int startIndex)
        {
            var contextInfo = new ParseContext(this)
            {
                ElementName = elementName,
                OuterHtml = outerHtml,
                InnerHtml = innerHtml,
                Attributes = attributes,
                StartIndex = startIndex
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
        public string ElementName { get; set; }

        public string OuterHtml { get; set; }

        public string InnerHtml { get; set; }

        public int StartIndex { get; set; }

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

using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class Container
    {
        private Container() { }

        public static void PopSiteItems(PageInfo pageInfo)
        {
            if (pageInfo.SiteItems.Count > 0)
            {
                pageInfo.SiteItems.Pop();
            }
        }

        public static void PopChannelItem(PageInfo pageInfo)
        {
            if (pageInfo.ChannelItems.Count > 0)
            {
                pageInfo.ChannelItems.Pop();
            }
        }

        public static void PopContentItem(PageInfo pageInfo)
        {
            if (pageInfo.ContentItems.Count > 0)
            {
                pageInfo.ContentItems.Pop();
            }
        }

        public static void PopSqlItem(PageInfo pageInfo)
        {
            if (pageInfo.SqlItems.Count > 0)
            {
                pageInfo.SqlItems.Pop();
            }
        }

        public static void PopEachItem(PageInfo pageInfo)
        {
            if (pageInfo.EachItems.Count > 0)
            {
                pageInfo.EachItems.Pop();
            }
        }

        public static Container GetContainer(PageInfo pageInfo)
        {
            var container = new Container();
            if (pageInfo.SiteItems.Count > 0)
            {
                container.SiteItem = pageInfo.SiteItems.Peek();
            }
            if (pageInfo.ChannelItems.Count > 0)
            {
                container.ChannelItem = pageInfo.ChannelItems.Peek();
            }
            if (pageInfo.ContentItems.Count > 0)
            {
                container.ContentItem = pageInfo.ContentItems.Peek();
            }
            if (pageInfo.EachItems.Count > 0)
            {
                container.EachItem = pageInfo.EachItems.Peek();
            }
            if (pageInfo.SqlItems.Count > 0)
            {
                container.SqlItem = pageInfo.SqlItems.Peek();
            }
            return container;
        }

        public KeyValuePair<int, CMS.Models.Site> SiteItem { get; private set; }

        public KeyValuePair<int, CMS.Models.Channel> ChannelItem { get; private set; }

        public KeyValuePair<int, CMS.Models.Content> ContentItem { get; private set; }

        public KeyValuePair<int, object> EachItem { get; private set; }

        public KeyValuePair<int, Dictionary<string, object>> SqlItem { get; private set; }
    }
}

using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Parse
{
    public class DbItemContainer
    {
        private DbItemContainer() { }

        public static void PopChannelItem(ParsePage pageInfo)
        {
            if (pageInfo.ChannelItems.Count > 0)
            {
                pageInfo.ChannelItems.Pop();
            }
        }

        public static void PopContentItem(ParsePage pageInfo)
        {
            if (pageInfo.ContentItems.Count > 0)
            {
                pageInfo.ContentItems.Pop();
            }
        }

        public static void PopSqlItem(ParsePage pageInfo)
        {
            if (pageInfo.SqlItems.Count > 0)
            {
                pageInfo.SqlItems.Pop();
            }
        }

        public static void PopSiteItems(ParsePage pageInfo)
        {
            if (pageInfo.SiteItems.Count > 0)
            {
                pageInfo.SiteItems.Pop();
            }
        }

        public static void PopEachItem(ParsePage pageInfo)
        {
            if (pageInfo.EachItems.Count > 0)
            {
                pageInfo.EachItems.Pop();
            }
        }

        public static DbItemContainer GetItemContainer(ParsePage pageInfo)
        {
            var dbItemContainer = new DbItemContainer();
            if (pageInfo.ChannelItems.Count > 0)
            {
                dbItemContainer.ChannelItem = pageInfo.ChannelItems.Peek();
            }
            if (pageInfo.ContentItems.Count > 0)
            {
                dbItemContainer.ContentItem = pageInfo.ContentItems.Peek();
            }
            if (pageInfo.SqlItems.Count > 0)
            {
                dbItemContainer.SqlItem = pageInfo.SqlItems.Peek();
            }
            if (pageInfo.SiteItems.Count > 0)
            {
                dbItemContainer.SiteItem = pageInfo.SiteItems.Peek();
            }
            if (pageInfo.EachItems.Count > 0)
            {
                dbItemContainer.EachItem = pageInfo.EachItems.Peek();
            }
            return dbItemContainer;
        }

        public KeyValuePair<int, Channel> ChannelItem { get; private set; }

        public KeyValuePair<int, Content> ContentItem { get; private set; }

        public KeyValuePair<int, IDictionary<string, object>> SqlItem { get; private set; }

        public KeyValuePair<int, Site> SiteItem { get; private set; }

        public KeyValuePair<int, object> EachItem { get; private set; }
    }
}

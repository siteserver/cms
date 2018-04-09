namespace SiteServer.CMS.StlParser.Model
{
    public class DbItemContainer
    {
        private DbItemContainer() { }

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

        public static void PopSiteItems(PageInfo pageInfo)
        {
            if (pageInfo.SiteItems.Count > 0)
            {
                pageInfo.SiteItems.Pop();
            }
        }

        public static void PopEachItem(PageInfo pageInfo)
        {
            if (pageInfo.EachItems.Count > 0)
            {
                pageInfo.EachItems.Pop();
            }
        }

        public static DbItemContainer GetItemContainer(PageInfo pageInfo)
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
                dbItemContainer.SqlItem = (DbItemInfo)pageInfo.SqlItems.Peek();
            }
            if (pageInfo.SiteItems.Count > 0)
            {
                dbItemContainer.SiteItem = (DbItemInfo)pageInfo.SiteItems.Peek();
            }
            if (pageInfo.EachItems.Count > 0)
            {
                dbItemContainer.EachItem = (DbItemInfo)pageInfo.EachItems.Peek();
            }
            return dbItemContainer;
        }

        public ChannelItemInfo ChannelItem { get; private set; }

        public ContentItemInfo ContentItem { get; private set; }

        public DbItemInfo SqlItem { get; private set; }

        public DbItemInfo SiteItem { get; private set; }

        public DbItemInfo EachItem { get; private set; }
    }
}

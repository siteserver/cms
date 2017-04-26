namespace SiteServer.CMS.StlParser.Model
{
    public class DbItemContainer
    {
        private DbItemInfo channelItem;
        private DbItemInfo contentItem;
        private DbItemInfo commentItem;
        private DbItemInfo inputItem;
        private DbItemInfo sqlItem;
        private DbItemInfo siteItem;
        private DbItemInfo photoItem;
        private DbItemInfo eachItem;

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

        public static void PopCommentItem(PageInfo pageInfo)
        {
            if (pageInfo.CommentItems.Count > 0)
            {
                pageInfo.CommentItems.Pop();
            }
        }

        public static void PopInputItem(PageInfo pageInfo)
        {
            if (pageInfo.InputItems.Count > 0)
            {
                pageInfo.InputItems.Pop();
            }
        }

        public static void PopSqlItem(PageInfo pageInfo)
        {
            if (pageInfo.SqlItems.Count > 0)
            {
                pageInfo.SqlItems.Pop();
            }
        }

        public static void PopPhotoItem(PageInfo pageInfo)
        {
            if (pageInfo.PhotoItems.Count > 0)
            {
                pageInfo.PhotoItems.Pop();
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
                dbItemContainer.channelItem = (DbItemInfo)pageInfo.ChannelItems.Peek();
            }
            if (pageInfo.ContentItems.Count > 0)
            {
                dbItemContainer.contentItem = (DbItemInfo)pageInfo.ContentItems.Peek();
            }
            if (pageInfo.CommentItems.Count > 0)
            {
                dbItemContainer.commentItem = (DbItemInfo)pageInfo.CommentItems.Peek();
            }
            if (pageInfo.InputItems.Count > 0)
            {
                dbItemContainer.inputItem = (DbItemInfo)pageInfo.InputItems.Peek();
            }
            if (pageInfo.SqlItems.Count > 0)
            {
                dbItemContainer.sqlItem = (DbItemInfo)pageInfo.SqlItems.Peek();
            }
            if (pageInfo.SiteItems.Count > 0)
            {
                dbItemContainer.siteItem = (DbItemInfo)pageInfo.SiteItems.Peek();
            }
            if (pageInfo.PhotoItems.Count > 0)
            {
                dbItemContainer.photoItem = (DbItemInfo)pageInfo.PhotoItems.Peek();
            }
            if (pageInfo.EachItems.Count > 0)
            {
                dbItemContainer.eachItem = (DbItemInfo)pageInfo.EachItems.Peek();
            }
            return dbItemContainer;
        }

        public DbItemInfo ChannelItem => channelItem;

        public DbItemInfo ContentItem => contentItem;

        public DbItemInfo CommentItem => commentItem;

        public DbItemInfo InputItem => inputItem;

        public DbItemInfo SqlItem => sqlItem;

        public DbItemInfo SiteItem => siteItem;

        public DbItemInfo PhotoItem => photoItem;

        public DbItemInfo EachItem => eachItem;
    }
}

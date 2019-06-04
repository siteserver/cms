using System;
using System.Collections.Generic;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Content;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Plugin;
using SS.CMS.Plugin.Data;

namespace SS.CMS.Core.Plugin.Apis
{
    public class ContentApi : IContentApi
    {
        private ContentApi() { }

        private static ContentApi _instance;
        public static ContentApi Instance => _instance ?? (_instance = new ContentApi());

        public IContentInfo GetContentInfo(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            return ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
        }

        public List<IContentInfo> GetContentInfoList(int siteId, int channelId, string whereString, string orderString, int limit, int offset)
        {
            if (siteId <= 0 || channelId <= 0) return null;

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            var list = channelInfo.ContentDao.GetContentInfoList(whereString, orderString, offset, limit);
            var retval = new List<IContentInfo>();
            foreach (var contentInfo in list)
            {
                retval.Add(contentInfo);
            }
            return retval;
        }

        public int GetCount(int siteId, int channelId, string whereString)
        {
            if (siteId <= 0 || channelId <= 0) return 0;

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            return channelInfo.ContentDao.GetCount(whereString);
        }

        public string GetTableName(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return string.Empty;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            return ChannelManager.GetTableName(siteInfo, nodeInfo);
        }

        public List<TableColumn> GetTableColumns(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return null;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var tableStyleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, nodeInfo);
            var tableColumnList = new List<TableColumn>
            {
                new TableColumn
                {
                    AttributeName = ContentAttribute.Title,
                    DataType = DataType.VarChar,
                    DataLength = 255
                }
            };

            foreach (var styleInfo in tableStyleInfoList)
            {
                tableColumnList.Add(new TableColumn
                {
                    AttributeName = styleInfo.AttributeName,
                    DataType = DataType.VarChar,
                    DataLength = 50
                });
            }

            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsTop,
                DataType = DataType.VarChar,
                DataLength = 18
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsRecommend,
                DataType = DataType.VarChar,
                DataLength = 18
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsHot,
                DataType = DataType.VarChar,
                DataLength = 18
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsColor,
                DataType = DataType.VarChar,
                DataLength = 18
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.AddDate,
                DataType = DataType.DateTime
            });

            return tableColumnList;
        }

        public string GetContentValue(int siteId, int channelId, int contentId, string attributeName)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            return channelInfo.ContentDao.GetValue<string>(contentId, attributeName);
        }

        public IContentInfo NewInstance(int siteId, int channelId)
        {
            return new ContentInfo(new Dictionary<string, object>
            {
                {ContentAttribute.SiteId, siteId},
                {ContentAttribute.ChannelId, channelId},
                {ContentAttribute.AddDate, DateTime.Now}
            });
        }

        //public void SetValuesToContentInfo(int siteId, int channelId, NameValueCollection form, IContentInfo contentInfo)
        //{
        //    var siteInfo = SiteManager.GetSiteInfo(siteId);
        //    var nodeInfo = NodeManager.GetChannelInfo(siteId, channelId);
        //    var tableName = NodeManager.GetTableName(siteInfo, nodeInfo);
        //    var tableStyle = NodeManager.GetTableStyle(siteInfo, nodeInfo);
        //    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(siteId, channelId);

        //    var extendImageUrl = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
        //    if (form.AllKeys.Contains(StringUtils.LowerFirst(extendImageUrl)))
        //    {
        //        form[extendImageUrl] = form[StringUtils.LowerFirst(extendImageUrl)];
        //    }

        //    InputTypeParser.AddValuesToAttributes(tableStyle, tableName, siteInfo, relatedIdentities, form, contentInfo.ToNameValueCollection(), ContentAttribute.HiddenAttributes);
        //}

        public int Insert(int siteId, int channelId, IContentInfo contentInfo)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            return channelInfo.ContentDao.Insert(siteInfo, channelInfo, (ContentInfo)contentInfo);
        }

        public void Update(int siteId, int channelId, IContentInfo contentInfo)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            channelInfo.ContentDao.Update(siteInfo, channelInfo, (ContentInfo)contentInfo);
        }

        public void Delete(int siteId, int channelId, int contentId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var contentIdList = new List<int> { contentId };
            channelInfo.ContentDao.UpdateTrashContents(siteId, channelId, contentIdList);
        }

        public IList<int> GetContentIdList(int siteId, int channelId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            return channelInfo.ContentDao.GetContentIdListCheckedByChannelId(siteId, channelId);
        }

        public string GetContentUrl(int siteId, int channelId, int contentId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetContentUrl(siteInfo, ChannelManager.GetChannelInfo(siteId, channelId), contentId, false);
        }
    }
}

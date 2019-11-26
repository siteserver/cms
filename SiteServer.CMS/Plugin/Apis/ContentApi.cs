using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ContentApi : IContentApi
    {
        private ContentApi() { }

        private static ContentApi _instance;
        public static ContentApi Instance => _instance ?? (_instance = new ContentApi());

        public async Task<IContent> GetContentInfoAsync(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

            //return ContentManager.GetContentInfo(site, channel, contentId);
            return await DataProvider.ContentDao.GetContentAsync(await ChannelManager.GetTableNameAsync(site, channelInfo), contentId);
        }

        //public async Task<List<IContent>> GetContentInfoListAsync(int siteId, int channelId, string whereString, string orderString, int limit, int offset)
        //{
        //    if (siteId <= 0 || channelId <= 0) return null;

        //    var site = await DataProvider.SiteDao.GetAsync(siteId);
        //    var tableName = await ChannelManager.GetTableNameAsync(site, channelId);

        //    var list = await DataProvider.ContentDao.GetContentInfoListAsync(tableName, whereString, orderString, offset, limit);
        //    var retVal = new List<IContent>();
        //    foreach (var contentInfo in list)
        //    {
        //        retVal.Add(contentInfo);
        //    }
        //    return retVal;
        //}

        public async Task<int> GetCountAsync(int siteId, int channelId, string whereString)
        {
            if (siteId <= 0 || channelId <= 0) return 0;

            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channelId);

            return DataProvider.ContentDao.GetCount(tableName, whereString);
        }

        public async Task<string> GetTableNameAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return string.Empty;

            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            return await ChannelManager.GetTableNameAsync(site, nodeInfo);
        }

        public async Task<List<TableColumn>> GetTableColumnsAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return null;

            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var tableStyleInfoList = await TableStyleManager.GetContentStyleListAsync(site, nodeInfo);
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

        public async Task<List<InputStyle>> GetInputStylesAsync(int siteId, int channelId)
        {
            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

            return await ChannelManager.GetInputStylesAsync(site, channelInfo);
        }

        public async Task<string> GetContentValueAsync(int siteId, int channelId, int contentId, string attributeName)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channelId);

            var tuple = DataProvider.ContentDao.GetValue(tableName, contentId, attributeName);
            return tuple?.Item2;
        }

        public IContent NewInstance(int siteId, int channelId)
        {
            return new Content(new Dictionary<string, object>
            {
                {ContentAttribute.SiteId, siteId},
                {ContentAttribute.ChannelId, channelId},
                {ContentAttribute.AddDate, DateTime.Now}
            });
        }

        //public void SetValuesToContentInfo(int siteId, int channelId, NameValueCollection form, IContentInfo contentInfo)
        //{
        //    var site = DataProvider.SiteDao.GetSite(siteId);
        //    var node = NodeManager.GetChannelInfo(siteId, channelId);
        //    var tableName = NodeManager.GetTableName(site, node);
        //    var tableStyle = NodeManager.GetTableStyle(site, node);
        //    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(siteId, channelId);

        //    var extendImageUrl = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
        //    if (form.AllKeys.Contains(StringUtils.LowerFirst(extendImageUrl)))
        //    {
        //        form[extendImageUrl] = form[StringUtils.LowerFirst(extendImageUrl)];
        //    }

        //    InputTypeParser.AddValuesToAttributes(tableStyle, tableName, site, relatedIdentities, form, contentInfo.ToNameValueCollection(), ContentAttribute.HiddenAttributes);
        //}

        public async Task<int> InsertAsync(int siteId, int channelId, IContent contentInfo)
        {
            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);

            return await DataProvider.ContentDao.InsertAsync(tableName, site, channelInfo, (Content)contentInfo);
        }

        public async Task UpdateAsync(int siteId, int channelId, IContent contentInfo)
        {
            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            await DataProvider.ContentDao.UpdateAsync(site, channelInfo, (Content)contentInfo);
        }

        public async Task DeleteAsync(int siteId, int channelId, int contentId)
        {
            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var tableName = await ChannelManager.GetTableNameAsync(site, nodeInfo);
            var contentIdList = new List<int> { contentId };
            await DataProvider.ContentDao.UpdateTrashContentsAsync(siteId, channelId, tableName, contentIdList);
        }

        public async Task<IList<int>> GetContentIdListAsync(int siteId, int channelId)
        {
            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
            return DataProvider.ContentDao.GetContentIdListCheckedByChannelId(tableName, siteId, channelId);
        }

        public async Task<string> GetContentUrlAsync(int siteId, int channelId, int contentId)
        {
            var site = await DataProvider.SiteDao.GetAsync(siteId);
            return await PageUtility.GetContentUrlAsync(site, await ChannelManager.GetChannelAsync(siteId, channelId), contentId, false);
        }
    }
}

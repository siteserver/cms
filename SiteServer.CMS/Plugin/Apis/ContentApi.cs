using System.Collections.Generic;
using SiteServer.Utils.Model;
using SiteServer.Utils.Table;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Apis;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ContentApi : IContentApi
    {
        private ContentApi() { }

        private static ContentApi _instance;
        public static ContentApi Instance => _instance ?? (_instance = new ContentApi());

        public IContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetContentInfo(tableName, contentId);
        }

        public List<IContentInfo> GetContentInfoList(int publishmentSystemId, int channelId, string whereString, string orderString, int limit, int offset)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetListByLimitAndOffset(tableName, channelId, whereString, orderString, limit, offset);
        }

        public int GetCount(int publishmentSystemId, int channelId, string whereString)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return 0;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetCount(tableName, channelId, whereString);
        }

        public string GetTableName(int publishmentSystemId, int channelId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return string.Empty;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            return NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
        }

        public List<TableColumn> GetTableColumns(int publishmentSystemId, int channelId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, channelId);

            var tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);
            var tableColumnList = new List<TableColumn>
            {
                new TableColumn
                {
                    AttributeName = ContentAttribute.Title,
                    DataType = DataType.VarChar,
                    DataLength = 255,
                    InputStyle = new InputStyle
                    {
                        InputType = InputType.Text,
                        DisplayName = "标题",
                        IsRequired = true,
                        ValidateType = ValidateType.None
                    }
                }
            };

            foreach (var styleInfo in tableStyleInfoList)
            {
                tableColumnList.Add(new TableColumn
                {
                    AttributeName = styleInfo.AttributeName,
                    DataType = DataType.VarChar,
                    DataLength = 50,
                    InputStyle = new InputStyle
                    {
                        InputType = styleInfo.InputType,
                        DisplayName = styleInfo.DisplayName,
                        DefaultValue = styleInfo.DefaultValue,
                        IsRequired = styleInfo.Additional.IsRequired,
                        ValidateType = styleInfo.Additional.ValidateType,
                        MinNum = styleInfo.Additional.MinNum,
                        MaxNum = styleInfo.Additional.MaxNum,
                        RegExp = styleInfo.Additional.RegExp,
                        Width = styleInfo.Additional.Width,
                    }
                });
            }

            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsTop,
                DataType = DataType.VarChar,
                DataLength = 18,
                InputStyle = new InputStyle
                {
                    InputType = InputType.CheckBox,
                    DisplayName = "置顶"
                }
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsRecommend,
                DataType = DataType.VarChar,
                DataLength = 18,
                InputStyle = new InputStyle
                {
                    InputType = InputType.CheckBox,
                    DisplayName = "推荐"
                }
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsHot,
                DataType = DataType.VarChar,
                DataLength = 18,
                InputStyle = new InputStyle
                {
                    InputType = InputType.CheckBox,
                    DisplayName = "热点"
                }
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsColor,
                DataType = DataType.VarChar,
                DataLength = 18,
                InputStyle = new InputStyle
                {
                    InputType = InputType.CheckBox,
                    DisplayName = "醒目"
                }
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.AddDate,
                DataType = DataType.DateTime,
                InputStyle = new InputStyle
                {
                    InputType = InputType.DateTime,
                    DisplayName = "添加时间"
                }
            });

            return tableColumnList;
        }

        public string GetContentValue(int publishmentSystemId, int channelId, int contentId, string attributeName)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetValue(tableName, contentId, attributeName);
        }

        public IContentInfo NewInstance()
        {
            return new ContentInfo();
        }

        //public void SetValuesToContentInfo(int publishmentSystemId, int channelId, NameValueCollection form, IContentInfo contentInfo)
        //{
        //    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
        //    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
        //    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
        //    var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
        //    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, channelId);

        //    var extendImageUrl = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
        //    if (form.AllKeys.Contains(StringUtils.LowerFirst(extendImageUrl)))
        //    {
        //        form[extendImageUrl] = form[StringUtils.LowerFirst(extendImageUrl)];
        //    }

        //    InputTypeParser.AddValuesToAttributes(tableStyle, tableName, publishmentSystemInfo, relatedIdentities, form, contentInfo.ToNameValueCollection(), ContentAttribute.HiddenAttributes);
        //}

        public int Insert(int publishmentSystemId, int channelId, IContentInfo contentInfo)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            return DataProvider.ContentDao.Insert(tableName, publishmentSystemInfo, contentInfo);
        }

        public void Update(int publishmentSystemId, int channelId, IContentInfo contentInfo)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            DataProvider.ContentDao.Update(tableName, publishmentSystemInfo, contentInfo);
        }

        public void Delete(int publishmentSystemId, int channelId, int contentId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var contentIdList = new List<int> { contentId };
            DataProvider.ContentDao.TrashContents(publishmentSystemId, tableName, contentIdList);
        }

        public List<int> GetContentIdList(int publishmentSystemId, int channelId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);
            return DataProvider.ContentDao.GetContentIdListCheckedByNodeId(tableName, publishmentSystemId, channelId);
        }
    }
}

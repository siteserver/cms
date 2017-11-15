using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ContentApi : IContentApi
    {
        private ContentApi() { }

        public static ContentApi Instance { get; } = new ContentApi();

        public IContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
        }

        public List<IContentInfo> GetContentInfoList(int publishmentSystemId, int channelId, string whereString, string orderString, int limit, int offset)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetListByLimitAndOffset(tableName, tableStyle, channelId, whereString, orderString, limit, offset);
        }

        public int GetCount(int publishmentSystemId, int channelId, string whereString)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return 0;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetCount(tableName, tableStyle, channelId, whereString);
        }

        public List<PluginTableColumn> GetTableColumns(int publishmentSystemId, int channelId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, channelId);

            var tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            var tableColumnList = new List<PluginTableColumn>
            {
                new PluginTableColumn
                {
                    AttributeName = ContentAttribute.Title,
                    DisplayName = "标题",
                    DataType = nameof(DataType.VarChar),
                    DataLength = 255,
                    InputType = nameof(InputType.Text),
                    IsRequired = true,
                    ValidateType = nameof(ValidateType.None),
                    IsVisibleInEdit = true,
                    IsVisibleInList = true
                }
            };

            foreach (var styleInfo in tableStyleInfoList)
            {
                tableColumnList.Add(new PluginTableColumn
                {
                    AttributeName = styleInfo.AttributeName,
                    DisplayName = styleInfo.DisplayName,
                    DataType = nameof(DataType.VarChar),
                    DataLength = 50,
                    InputType = styleInfo.InputType,
                    DefaultValue = styleInfo.DefaultValue,
                    IsRequired = styleInfo.Additional.IsRequired,
                    ValidateType = ValidateTypeUtils.GetValue(styleInfo.Additional.ValidateType),
                    MinNum = styleInfo.Additional.MinNum,
                    MaxNum = styleInfo.Additional.MaxNum,
                    RegExp = styleInfo.Additional.RegExp,
                    Width = styleInfo.Additional.Width,
                    IsVisibleInEdit = styleInfo.IsVisible,
                    IsVisibleInList = styleInfo.IsVisibleInList
                });
            }

            tableColumnList.Add(new PluginTableColumn
            {
                AttributeName = ContentAttribute.IsTop,
                DisplayName = "置顶",
                DataType = nameof(DataType.VarChar),
                DataLength = 18,
                InputType = nameof(InputType.CheckBox)
            });
            tableColumnList.Add(new PluginTableColumn
            {
                AttributeName = ContentAttribute.IsRecommend,
                DisplayName = "推荐",
                DataType = nameof(DataType.VarChar),
                DataLength = 18,
                InputType = nameof(InputType.CheckBox)
            });
            tableColumnList.Add(new PluginTableColumn
            {
                AttributeName = ContentAttribute.IsHot,
                DisplayName = "热点",
                DataType = nameof(DataType.VarChar),
                DataLength = 18,
                InputType = nameof(InputType.CheckBox)
            });
            tableColumnList.Add(new PluginTableColumn
            {
                AttributeName = ContentAttribute.IsColor,
                DisplayName = "醒目",
                DataType = nameof(DataType.VarChar),
                DataLength = 18,
                InputType = nameof(InputType.CheckBox)
            });
            tableColumnList.Add(new PluginTableColumn
            {
                AttributeName = ContentAttribute.AddDate,
                DisplayName = "添加时间",
                DataType = nameof(DataType.DateTime),
                InputType = nameof(InputType.DateTime)
            });

            return tableColumnList;
        }

        public string GetContentValue(int publishmentSystemId, int channelId, int contentId, string attributeName)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return BaiRongDataProvider.ContentDao.GetValue(tableName, contentId, attributeName);
        }

        public IContentInfo NewInstance(int publishmentSystemId, int channelId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);

            return ContentUtility.GetContentInfo(tableStyle);
        }

        public void SetValuesToContentInfo(int publishmentSystemId, int channelId, NameValueCollection form, IContentInfo contentInfo)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, channelId);

            var extendImageUrl = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
            if (form.AllKeys.Contains(StringUtils.LowerFirst(extendImageUrl)))
            {
                form[extendImageUrl] = form[StringUtils.LowerFirst(extendImageUrl)];
            }

            InputTypeParser.AddValuesToAttributes(tableStyle, tableName, publishmentSystemInfo, relatedIdentities, form, contentInfo.Attributes.GetExtendedAttributes(), ContentAttribute.HiddenAttributes);
        }

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
    }
}

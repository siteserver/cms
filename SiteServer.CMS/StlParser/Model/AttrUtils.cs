using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Provider;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.Model
{
    public static class AttrUtils
    {
        public static List<AttrEnum> GetEnums(Type elementType, string name, int siteId)
        {
            var enums = new List<AttrEnum>();
            if (name == "channelIndex")
            {
                var channelInfoList = ChannelManager.GetChannelInfoList(siteId);
                foreach (var channelInfo in channelInfoList)
                {
                    if (!string.IsNullOrEmpty(channelInfo.IndexName))
                    {
                        enums.Add(new AttrEnum(channelInfo.IndexName, string.Empty));
                    }
                }
            }
            else if (name == "channelName")
            {
                var channelInfoList = ChannelManager.GetChannelInfoList(siteId);
                foreach (var channelInfo in channelInfoList)
                {
                    enums.Add(new AttrEnum(channelInfo.ChannelName, string.Empty));
                }
            }
            else if (name == "context")
            {
                enums = new List<AttrEnum>
                {
                    new AttrEnum("site", "站点"),
                    new AttrEnum("channel", "栏目"),
                    new AttrEnum("content", "内容"),
                    new AttrEnum("sqlContent", "SQL")
                };
            }
            else if (name == "type")
            {
                if (elementType == typeof(StlAudio))
                {
                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    var tableName = ChannelManager.GetTableName(siteInfo, siteId);
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(siteId, siteId);
                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);
                    var editableStyleInfoList = ContentUtility.GetEditableTableStyleInfoList(styleInfoList);

                    foreach (var styleInfo in editableStyleInfoList)
                    {
                        if (styleInfo.InputType == InputType.Text || styleInfo.InputType == InputType.Video || styleInfo.InputType == InputType.Image || styleInfo.InputType == InputType.File)
                        {
                            enums.Add(new AttrEnum(styleInfo.AttributeName, styleInfo.DisplayName));
                        }
                    }
                }
                else if (elementType == typeof(StlChannel))
                {
                    var fields = typeof(ChannelAttribute).GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach (var field in fields)
                    {
                        
                        var stlAttribute = (StlFieldAttribute)field.GetCustomAttribute(typeof(StlFieldAttribute));
                        var desc = string.Empty;
                        if (stlAttribute != null)
                        {
                            desc = stlAttribute.Description;
                        }
                        enums.Add(new AttrEnum(field.Name, desc));
                    }
                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.ChannelDao.TableName, RelatedIdentities.GetChannelRelatedIdentities(siteId, siteId));
                    foreach (var styleInfo in styleInfoList)
                    {
                        enums.Add(new AttrEnum(styleInfo.AttributeName, styleInfo.DisplayName));
                    }
                }
            }

            return enums;
        }

        public static string GetAttrTypeText(AttrType attrType, List<AttrEnum> attrEnums)
        {
            if (attrType == AttrType.Boolean)
            {
                return "布尔值";
            }
            if (attrType == AttrType.DateTime)
            {
                return "日期";
            }
            if (attrType == AttrType.Decimal)
            {
                return "小数";
            }
            if (attrType == AttrType.Integer)
            {
                return "整数";
            }
            if (attrType == AttrType.Enum)
            {
                if (attrEnums != null && attrEnums.Count > 0)
                {
                    var builder = new StringBuilder();
                    foreach (var attrEnum in attrEnums)
                    {
                        builder.Append(
                            $@"<div class=""list-group-item"" style=""padding: 6px 10px"">
    <small class=""float-left"">{attrEnum.Name}</small>
    <small class=""float-right"">{attrEnum.Description}</small>
  </div>");
                    }
                    return $@"
<div class=""row"">
    <div class=""col-6"">枚举</div>
    <div class=""col-6 text-right"">
        <a class=""enum-link ion-ios-arrow-down"" href=""javascript:;""></a>
    </div>
</div>
<div style=""display: none"">
    <hr />
    <div class=""list-group"">
      {builder}
    </div>
</div>
";
                }
                return "枚举";
            }

            return "字符串";
        }
    }
}

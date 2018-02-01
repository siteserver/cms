using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSelectColumns : BasePageCms
    {
        public CheckBoxList CblDisplayAttributes;

        private int _relatedIdentity;
        private List<int> _relatedIdentities;
        private bool _isList;
        private bool _isContent;

        public static string GetOpenWindowStringToChannel(int siteId, bool isList)
        {
            return LayerUtils.GetOpenScript("选择需要显示的项", PageUtils.GetCmsUrl(siteId, nameof(ModalSelectColumns), new NameValueCollection
            {
                {"RelatedIdentity", siteId.ToString()},
                {"IsList", isList.ToString()}
            }), 520, 550);
        }

        public static string GetOpenWindowStringToContent(int siteId, int relatedIdentity, bool isList)
        {
            return LayerUtils.GetOpenScript("选择需要显示的项", PageUtils.GetCmsUrl(siteId, nameof(ModalSelectColumns), new NameValueCollection
            {
                {"RelatedIdentity", relatedIdentity.ToString()},
                {"IsContent", true.ToString()},
                {"IsList", isList.ToString()}
            }), 520, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _relatedIdentity = Body.GetQueryInt("RelatedIdentity");
            _isList = Body.GetQueryBool("IsList");
            _isContent = Body.GetQueryBool("IsContent");

            if (!_isContent)
            {
                var displayAttributes = SiteInfo.Additional.ChannelDisplayAttributes;
                if (!_isList)
                {
                    displayAttributes = SiteInfo.Additional.ChannelEditAttributes;
                }

                if (IsPostBack) return;

                //添加默认属性
                var listitem = new ListItem("栏目名称", ChannelAttribute.ChannelName);
                if (CompareUtils.Contains(displayAttributes, ChannelAttribute.ChannelName))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                listitem = new ListItem("栏目索引", ChannelAttribute.ChannelIndex);
                if (CompareUtils.Contains(displayAttributes, ChannelAttribute.ChannelIndex))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                listitem = new ListItem("生成页面路径", ChannelAttribute.FilePath);
                if (CompareUtils.Contains(displayAttributes, ChannelAttribute.FilePath))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                if (!_isList)
                {
                    listitem = new ListItem("栏目图片地址", ChannelAttribute.ImageUrl);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.ImageUrl))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("栏目正文", ChannelAttribute.Content);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.Content))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("外部链接", ChannelAttribute.LinkUrl);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.LinkUrl))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("链接类型", ChannelAttribute.LinkUrl);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.LinkUrl))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("内容默认排序规则", nameof(ChannelInfoExtend.DefaultTaxisType));
                    if (CompareUtils.Contains(displayAttributes, nameof(ChannelInfoExtend.DefaultTaxisType)))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("栏目模版", ChannelAttribute.ChannelTemplateId);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.ChannelTemplateId))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("内容模版", ChannelAttribute.ContentTemplateId);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.ContentTemplateId))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("关键字列表", ChannelAttribute.Keywords);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.Keywords))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("页面描述", ChannelAttribute.Description);
                    if (CompareUtils.Contains(displayAttributes, ChannelAttribute.Description))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);
                }

                listitem = new ListItem("栏目组", ChannelAttribute.ChannelGroupNameCollection);
                if (CompareUtils.Contains(displayAttributes, ChannelAttribute.ChannelGroupNameCollection))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.ChannelDao.TableName, _relatedIdentities);

                foreach (var styleInfo in styleInfoList)
                {
                    listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);

                    if (CompareUtils.Contains(displayAttributes, styleInfo.AttributeName))
                    {
                        listitem.Selected = true;
                    }

                    CblDisplayAttributes.Items.Add(listitem);
                }

                if (string.IsNullOrEmpty(displayAttributes))
                {
                    if (!_isList)
                    {
                        foreach (ListItem item in CblDisplayAttributes.Items)
                        {
                            item.Selected = true;
                        }
                    }
                    else
                    {
                        ControlUtils.SelectMultiItems(CblDisplayAttributes, ChannelAttribute.ChannelName, ChannelAttribute.ChannelIndex);
                    }
                }
            }
            else
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _relatedIdentity);
                var tableName = ChannelManager.GetTableName(SiteInfo, nodeInfo);
                _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _relatedIdentity);
                var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(nodeInfo.Additional.ContentAttributesOfDisplay);

                if (IsPostBack) return;

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);
                var columnTableStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(SiteInfo, styleInfoList);
                foreach (var styleInfo in columnTableStyleInfoList)
                {
                    if (styleInfo.AttributeName == ContentAttribute.Title) continue;
                    var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);

                    if (_isList)
                    {
                        if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                        {
                            listitem.Selected = true;
                        }
                    }
                    else
                    {
                        listitem.Selected = true;
                    }

                    CblDisplayAttributes.Items.Add(listitem);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var displayAttributes = ControlUtils.SelectedItemsValueToStringCollection(CblDisplayAttributes.Items);
            if (!_isContent)
            {
                if (!_isList)
                {
                    if (CblDisplayAttributes.Items.Count == 0)
                    {
                        FailMessage("必须至少选择一项！");
                        return;
                    }
                    SiteInfo.Additional.ChannelEditAttributes = displayAttributes;

                    Body.AddSiteLog(SiteId, "设置栏目编辑项", $"编辑项:{displayAttributes}");
                }
                else
                {
                    if (!CompareUtils.Contains(displayAttributes, ChannelAttribute.ChannelName))
                    {
                        FailMessage("必须选择栏目名称项！");
                        return;
                    }
                    if (!CompareUtils.Contains(displayAttributes, ChannelAttribute.ChannelIndex))
                    {
                        FailMessage("必须选择栏目索引项！");
                        return;
                    }
                    SiteInfo.Additional.ChannelDisplayAttributes = displayAttributes;

                    Body.AddSiteLog(SiteId, "设置栏目显示项", $"显示项:{displayAttributes}");
                }
                DataProvider.SiteDao.Update(SiteInfo);
            }
            else
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _relatedIdentity);
                var attributesOfDisplay = ControlUtils.SelectedItemsValueToStringCollection(CblDisplayAttributes.Items);
                nodeInfo.Additional.ContentAttributesOfDisplay = attributesOfDisplay;

                DataProvider.ChannelDao.Update(nodeInfo);

                Body.AddSiteLog(SiteId, "设置内容显示项", $"显示项:{attributesOfDisplay}");
            }

            if (!_isList)
            {
                LayerUtils.CloseWithoutRefresh(Page);
            }
            else
            {
                LayerUtils.Close(Page);
            }
        }

    }
}

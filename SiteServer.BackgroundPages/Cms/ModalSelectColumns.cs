using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model;
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

        public static string GetOpenWindowStringToChannel(int publishmentSystemId, bool isList)
        {
            return LayerUtils.GetOpenScript("选择需要显示的项", PageUtils.GetCmsUrl(nameof(ModalSelectColumns), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedIdentity", publishmentSystemId.ToString()},
                {"IsList", isList.ToString()}
            }), 520, 550);
        }

        public static string GetOpenWindowStringToContent(int publishmentSystemId, int relatedIdentity, bool isList)
        {
            return LayerUtils.GetOpenScript("选择需要显示的项", PageUtils.GetCmsUrl(nameof(ModalSelectColumns), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedIdentity", relatedIdentity.ToString()},
                {"IsContent", true.ToString()},
                {"IsList", isList.ToString()}
            }), 520, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _relatedIdentity = Body.GetQueryInt("RelatedIdentity");
            _isList = Body.GetQueryBool("IsList");
            _isContent = Body.GetQueryBool("IsContent");

            if (!_isContent)
            {
                var displayAttributes = PublishmentSystemInfo.Additional.ChannelDisplayAttributes;
                if (!_isList)
                {
                    displayAttributes = PublishmentSystemInfo.Additional.ChannelEditAttributes;
                }

                if (IsPostBack) return;

                //添加默认属性
                var listitem = new ListItem("栏目名称", NodeAttribute.ChannelName);
                if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelName))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                listitem = new ListItem("栏目索引", NodeAttribute.ChannelIndex);
                if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelIndex))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                listitem = new ListItem("生成页面路径", NodeAttribute.FilePath);
                if (CompareUtils.Contains(displayAttributes, NodeAttribute.FilePath))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                if (!_isList)
                {
                    listitem = new ListItem("栏目图片地址", NodeAttribute.ImageUrl);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.ImageUrl))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("栏目正文", NodeAttribute.Content);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.Content))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("外部链接", NodeAttribute.LinkUrl);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.LinkUrl))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("链接类型", NodeAttribute.LinkUrl);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.LinkUrl))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("内容默认排序规则", nameof(NodeInfoExtend.DefaultTaxisType));
                    if (CompareUtils.Contains(displayAttributes, nameof(NodeInfoExtend.DefaultTaxisType)))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("栏目模版", NodeAttribute.ChannelTemplateId);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelTemplateId))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("内容模版", NodeAttribute.ContentTemplateId);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.ContentTemplateId))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("关键字列表", NodeAttribute.Keywords);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.Keywords))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);

                    listitem = new ListItem("页面描述", NodeAttribute.Description);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.Description))
                    {
                        listitem.Selected = true;
                    }
                    CblDisplayAttributes.Items.Add(listitem);
                }

                listitem = new ListItem("栏目组", NodeAttribute.ChannelGroupNameCollection);
                if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelGroupNameCollection))
                {
                    listitem.Selected = true;
                }
                CblDisplayAttributes.Items.Add(listitem);

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.NodeDao.TableName, _relatedIdentities);

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
                        ControlUtils.SelectMultiItems(CblDisplayAttributes, NodeAttribute.ChannelName, NodeAttribute.ChannelIndex);
                    }
                }
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _relatedIdentity);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
                _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _relatedIdentity);
                var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(nodeInfo.Additional.ContentAttributesOfDisplay);

                if (IsPostBack) return;

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);
                var columnTableStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(PublishmentSystemInfo, styleInfoList);
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
                    PublishmentSystemInfo.Additional.ChannelEditAttributes = displayAttributes;

                    Body.AddSiteLog(PublishmentSystemId, "设置栏目编辑项", $"编辑项:{displayAttributes}");
                }
                else
                {
                    if (!CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelName))
                    {
                        FailMessage("必须选择栏目名称项！");
                        return;
                    }
                    if (!CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelIndex))
                    {
                        FailMessage("必须选择栏目索引项！");
                        return;
                    }
                    PublishmentSystemInfo.Additional.ChannelDisplayAttributes = displayAttributes;

                    Body.AddSiteLog(PublishmentSystemId, "设置栏目显示项", $"显示项:{displayAttributes}");
                }
                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _relatedIdentity);
                var attributesOfDisplay = ControlUtils.SelectedItemsValueToStringCollection(CblDisplayAttributes.Items);
                nodeInfo.Additional.ContentAttributesOfDisplay = attributesOfDisplay;

                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                Body.AddSiteLog(PublishmentSystemId, "设置内容显示项", $"显示项:{attributesOfDisplay}");
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSelectColumns : BasePageCms
    {
        public CheckBoxList DisplayAttributeCheckBoxList;

        private int _relatedIdentity;
        private List<int> _relatedIdentities;
        private ETableStyle _tableStyle;
        private bool _isList;

        public static string GetOpenWindowStringToChannel(int publishmentSystemId, bool isList)
        {
            return PageUtils.GetOpenWindowString("选择需要显示的项", PageUtils.GetCmsUrl(nameof(ModalSelectColumns), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedIdentity", publishmentSystemId.ToString()},
                {"IsList", isList.ToString()},
                {"TableStyle", ETableStyleUtils.GetValue(ETableStyle.Channel)}
            }), 520, 550);
        }

        public static string GetOpenWindowStringToContent(int publishmentSystemId, int relatedIdentity, bool isList)
        {
            return PageUtils.GetOpenWindowString("选择需要显示的项", PageUtils.GetCmsUrl(nameof(ModalSelectColumns), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedIdentity", relatedIdentity.ToString()},
                {"IsContent", true.ToString()},
                {"IsList", isList.ToString()}
            }), 520, 550);
        }

        public static string GetOpenWindowStringToInputContent(int publishmentSystemId, int relatedIdentity, bool isList)
        {
            return PageUtils.GetOpenWindowString("选择需要显示的项", PageUtils.GetCmsUrl(nameof(ModalSelectColumns), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"IsList", isList.ToString()},
                {"TableStyle", ETableStyleUtils.GetValue(ETableStyle.InputContent)},
                {"RelatedIdentity", relatedIdentity.ToString()}
            }), 520, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _relatedIdentity = Body.GetQueryInt("RelatedIdentity");
            _isList = Body.GetQueryBool("IsList");
            if (Body.GetQueryBool("IsContent"))
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _relatedIdentity);
                _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            }
            else
            {
                _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            }

            if (_tableStyle == ETableStyle.Channel)
            {
                var displayAttributes = PublishmentSystemInfo.Additional.ChannelDisplayAttributes;
                if (!_isList)
                {
                    displayAttributes = PublishmentSystemInfo.Additional.ChannelEditAttributes;
                }

                if (!IsPostBack)
                {
                    //添加默认属性
                    var listitem = new ListItem("栏目名称", NodeAttribute.ChannelName);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelName))
                    {
                        listitem.Selected = true;
                    }
                    DisplayAttributeCheckBoxList.Items.Add(listitem);

                    listitem = new ListItem("栏目索引", NodeAttribute.ChannelIndex);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelIndex))
                    {
                        listitem.Selected = true;
                    }
                    DisplayAttributeCheckBoxList.Items.Add(listitem);

                    listitem = new ListItem("生成页面路径", NodeAttribute.FilePath);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.FilePath))
                    {
                        listitem.Selected = true;
                    }
                    DisplayAttributeCheckBoxList.Items.Add(listitem);

                    if (!_isList)
                    {
                        listitem = new ListItem("栏目图片地址", NodeAttribute.ImageUrl);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.ImageUrl))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);

                        listitem = new ListItem("栏目正文", NodeAttribute.Content);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.Content))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);

                        listitem = new ListItem("外部链接", NodeAttribute.LinkUrl);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.LinkUrl))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);

                        listitem = new ListItem("链接类型", NodeAttribute.LinkUrl);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.LinkUrl))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);

                        listitem = new ListItem("栏目模版", NodeAttribute.ChannelTemplateId);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelTemplateId))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);

                        listitem = new ListItem("内容模版", NodeAttribute.ContentTemplateId);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.ContentTemplateId))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);

                        listitem = new ListItem("关键字列表", NodeAttribute.Keywords);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.Keywords))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);

                        listitem = new ListItem("页面描述", NodeAttribute.Description);
                        if (CompareUtils.Contains(displayAttributes, NodeAttribute.Description))
                        {
                            listitem.Selected = true;
                        }
                        DisplayAttributeCheckBoxList.Items.Add(listitem);
                    }

                    listitem = new ListItem("栏目组", NodeAttribute.ChannelGroupNameCollection);
                    if (CompareUtils.Contains(displayAttributes, NodeAttribute.ChannelGroupNameCollection))
                    {
                        listitem.Selected = true;
                    }
                    DisplayAttributeCheckBoxList.Items.Add(listitem);

                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, DataProvider.NodeDao.TableName, _relatedIdentities);

                    foreach (var styleInfo in styleInfoList)
                    {
                        if (styleInfo.IsVisible == false) continue;
                        listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);

                        if (CompareUtils.Contains(displayAttributes, styleInfo.AttributeName))
                        {
                            listitem.Selected = true;
                        }

                        DisplayAttributeCheckBoxList.Items.Add(listitem);
                    }

                    if (string.IsNullOrEmpty(displayAttributes))
                    {
                        if (!_isList)
                        {
                            foreach (ListItem item in DisplayAttributeCheckBoxList.Items)
                            {
                                item.Selected = true;
                            }
                        }
                        else
                        {
                            ControlUtils.SelectListItems(DisplayAttributeCheckBoxList, NodeAttribute.ChannelName, NodeAttribute.ChannelIndex);
                        }
                    }
                }
            }
            else if (ETableStyleUtils.IsContent(_tableStyle))
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _relatedIdentity);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
                _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _relatedIdentity);
                var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(nodeInfo.Additional.ContentAttributesOfDisplay);

                if (!IsPostBack)
                {
                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, tableName, _relatedIdentities);
                    var columnTableStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(PublishmentSystemInfo, _tableStyle, styleInfoList);
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
                            if (styleInfo.IsVisible)
                            {
                                listitem.Selected = true;
                            }
                        }

                        DisplayAttributeCheckBoxList.Items.Add(listitem);
                    }
                }
            }
            else if (_tableStyle == ETableStyle.InputContent)
            {
                var inputInfo = DataProvider.InputDao.GetInputInfo(_relatedIdentity);
                _relatedIdentities = RelatedIdentities.GetRelatedIdentities(_tableStyle, PublishmentSystemId, _relatedIdentity);

                if (!IsPostBack)
                {
                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, DataProvider.InputContentDao.TableName, _relatedIdentities);

                    foreach (var styleInfo in styleInfoList)
                    {
                        var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);

                        if (_isList)
                        {
                            if (styleInfo.IsVisibleInList)
                            {
                                listitem.Selected = true;
                            }
                        }
                        else
                        {
                            if (styleInfo.IsVisible)
                            {
                                listitem.Selected = true;
                            }
                        }

                        DisplayAttributeCheckBoxList.Items.Add(listitem);
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var displayAttributes = ControlUtils.SelectedItemsValueToStringCollection(DisplayAttributeCheckBoxList.Items);
            if (_tableStyle == ETableStyle.Channel)
            {
                if (!_isList)
                {
                    if (DisplayAttributeCheckBoxList.Items.Count == 0)
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
            else if (ETableStyleUtils.IsContent(_tableStyle))
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _relatedIdentity);
                var attributesOfDisplay = ControlUtils.SelectedItemsValueToStringCollection(DisplayAttributeCheckBoxList.Items);
                nodeInfo.Additional.ContentAttributesOfDisplay = attributesOfDisplay;

                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                Body.AddSiteLog(PublishmentSystemId, "设置内容显示项", $"显示项:{attributesOfDisplay}");
            }
            else if (_tableStyle == ETableStyle.InputContent)
            {
                var inputInfo = DataProvider.InputDao.GetInputInfo(_relatedIdentity);

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, DataProvider.InputContentDao.TableName, _relatedIdentities);
                var selectedValues = ControlUtils.GetSelectedListControlValueArrayList(DisplayAttributeCheckBoxList);

                foreach (var styleInfo in styleInfoList)
                {
                    if (_isList)
                    {
                        styleInfo.IsVisibleInList = selectedValues.Contains(styleInfo.AttributeName);
                    }
                    else
                    {
                        styleInfo.IsVisible = selectedValues.Contains(styleInfo.AttributeName);
                    }
                    styleInfo.RelatedIdentity = _relatedIdentity;

                    if (styleInfo.TableStyleId == 0)
                    {
                        TableStyleManager.Insert(styleInfo, _tableStyle);
                    }
                    else
                    {
                        TableStyleManager.Update(styleInfo);
                    }
                }


                if (_isList)
                {
                    Body.AddSiteLog(PublishmentSystemId, "设置提交表单显示项",
                        $"表单名称：{inputInfo.InputName},显示项:{TranslateUtils.ObjectCollectionToString(selectedValues)}");
                }
                else
                {
                    Body.AddSiteLog(PublishmentSystemId, "设置提交表单编辑项",
                        $"表单名称：{inputInfo.InputName},编辑项:{TranslateUtils.ObjectCollectionToString(selectedValues)}");
                }
            }

            if (!_isList)
            {
                PageUtils.CloseModalPageWithoutRefresh(Page);
            }
            else
            {
                PageUtils.CloseModalPage(Page);
            }
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalChannelEdit : BasePageCms
    {
        public Control NodeNameRow;
        public Control NodeIndexNameRow;
        public Control LinkUrlRow;
        public Control NodeGroupNameCollectionRow;
        public Control LinkTypeRow;
        public Control ChannelTemplateIDRow;
        public Control ContentTemplateIDRow;
        public Control ImageUrlRow;
        public Control FilePathRow;
        public Control ContentRow;
        public Control KeywordsRow;
        public Control DescriptionRow;

        public TextBox NodeName;
        public TextBox NodeIndexName;
        public TextBox LinkUrl;
        public CheckBoxList NodeGroupNameCollection;
        public DropDownList LinkType;
        public DropDownList ChannelTemplateID;
        public DropDownList ContentTemplateID;
        public TextBox tbImageUrl;
        public Literal ltlImageUrlButtonGroup;
        public TextBox FilePath;
        public TextBox Keywords;
        public TextBox Description;

        public TextEditorControl Content;

        public ChannelAuxiliaryControl channelControl;

        public Button btnSubmit;

        private int _nodeId;
        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetOpenLayerString("快速修改栏目", PageUtils.GetCmsUrl(nameof(ModalChannelEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }));
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(ModalChannelEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl");
            _nodeId = Body.GetQueryInt("NodeID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            channelControl = (ChannelAuxiliaryControl)FindControl("ControlForAuxiliary");
            if (!IsPostBack)
            {
                if (!HasChannelPermissions(_nodeId, AppManager.Cms.Permission.Channel.ChannelEdit))
                {
                    PageUtils.RedirectToErrorPage("您没有修改栏目的权限！");
                    return;
                }

                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
                if (nodeInfo != null)
                {
                    if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
                    {
                        LinkUrlRow.Visible = false;
                        LinkTypeRow.Visible = false;
                        ChannelTemplateIDRow.Visible = false;
                        FilePathRow.Visible = false;
                    }

                    btnSubmit.Attributes.Add("onclick", "if (UE && UE.getEditor('Content', {{allowDivTransToP: false}})){ UE.getEditor('Content', {{allowDivTransToP: false}}).sync(); }");

                    if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ChannelEditAttributes))
                    {
                        var channelEditAttributes = TranslateUtils.StringCollectionToStringList(PublishmentSystemInfo.Additional.ChannelEditAttributes);
                        if (channelEditAttributes.Count > 0)
                        {
                            NodeNameRow.Visible = NodeIndexNameRow.Visible = LinkUrlRow.Visible = NodeGroupNameCollectionRow.Visible = LinkTypeRow.Visible = ChannelTemplateIDRow.Visible = ContentTemplateIDRow.Visible = ImageUrlRow.Visible = FilePathRow.Visible = ContentRow.Visible = KeywordsRow.Visible = DescriptionRow.Visible = false;
                            foreach (string attribute in channelEditAttributes)
                            {
                                if (attribute == NodeAttribute.ChannelName)
                                {
                                    NodeNameRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.ChannelIndex)
                                {
                                    NodeIndexNameRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.LinkUrl)
                                {
                                    LinkUrlRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.ChannelGroupNameCollection)
                                {
                                    NodeGroupNameCollectionRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.LinkType)
                                {
                                    LinkTypeRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.ChannelTemplateId)
                                {
                                    ChannelTemplateIDRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.ContentTemplateId)
                                {
                                    ContentTemplateIDRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.ImageUrl)
                                {
                                    ImageUrlRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.FilePath)
                                {
                                    FilePathRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.Content)
                                {
                                    ContentRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.Keywords)
                                {
                                    KeywordsRow.Visible = true;
                                }
                                else if (attribute == NodeAttribute.Description)
                                {
                                    DescriptionRow.Visible = true;
                                }
                            }
                        }
                    }

                    if (channelControl.Visible)
                    {
                        //List<string> displayAttributes = null;
                        //if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ChannelEditAttributes))
                        //{
                        //    displayAttributes = TranslateUtils.StringCollectionToStringList(PublishmentSystemInfo.Additional.ChannelEditAttributes);
                        //}
                        channelControl.SetParameters(nodeInfo.Additional.Attributes, true, IsPostBack);
                    }

                    if (LinkTypeRow.Visible)
                    {
                        ELinkTypeUtils.AddListItems(LinkType);
                    }

                    if (NodeGroupNameCollectionRow.Visible)
                    {
                        NodeGroupNameCollection.DataSource = DataProvider.NodeGroupDao.GetDataSource(PublishmentSystemId);
                    }
                    if (ChannelTemplateIDRow.Visible)
                    {
                        ChannelTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
                    }
                    if (ContentTemplateIDRow.Visible)
                    {
                        ContentTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);
                    }

                    DataBind();

                    if (ChannelTemplateIDRow.Visible)
                    {
                        ChannelTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                        ControlUtils.SelectListItems(ChannelTemplateID, nodeInfo.ChannelTemplateId.ToString());
                    }

                    if (ContentTemplateIDRow.Visible)
                    {
                        ContentTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                        ControlUtils.SelectListItems(ContentTemplateID, nodeInfo.ContentTemplateId.ToString());
                    }

                    if (NodeNameRow.Visible)
                    {
                        NodeName.Text = nodeInfo.NodeName;
                    }
                    if (NodeIndexNameRow.Visible)
                    {
                        NodeIndexName.Text = nodeInfo.NodeIndexName;
                    }
                    if (LinkUrlRow.Visible)
                    {
                        LinkUrl.Text = nodeInfo.LinkUrl;
                    }

                    if (NodeGroupNameCollectionRow.Visible)
                    {
                        foreach (ListItem item in NodeGroupNameCollection.Items)
                        {
                            if (CompareUtils.Contains(nodeInfo.NodeGroupNameCollection, item.Value))
                            {
                                item.Selected = true;
                            }
                            else
                            {
                                item.Selected = false;
                            }
                        }
                    }
                    if (FilePathRow.Visible)
                    {
                        FilePath.Text = nodeInfo.FilePath;
                    }

                    if (LinkTypeRow.Visible)
                    {
                        ControlUtils.SelectListItems(LinkType, ELinkTypeUtils.GetValue(nodeInfo.LinkType));
                    }

                    if (ImageUrlRow.Visible)
                    {
                        tbImageUrl.Text = nodeInfo.ImageUrl;
                        ltlImageUrlButtonGroup.Text = ControlUtility.GetImageUrlButtonGroupHtml(PublishmentSystemInfo, tbImageUrl.ClientID);
                    }
                    if (ContentRow.Visible)
                    {
                        var formCollection = new NameValueCollection();
                        formCollection[NodeAttribute.Content] = nodeInfo.Content;
                        Content.SetParameters(PublishmentSystemInfo, NodeAttribute.Content, formCollection, true, IsPostBack);

                        //this.Content.PublishmentSystemID = base.PublishmentSystemID;
                        //this.Content.Text = StringUtility.TextEditorContentDecode(nodeInfo.Content, ConfigUtils.Instance.ApplicationPath, base.PublishmentSystemInfo.PublishmentSystemUrl);
                    }
                    if (Keywords.Visible)
                    {
                        Keywords.Text = nodeInfo.Keywords;
                    }
                    if (Description.Visible)
                    {
                        Description.Text = nodeInfo.Description;
                    }


                }
            }
            else
            {
                if (channelControl.Visible)
                {
                    channelControl.SetParameters(Request.Form, true, IsPostBack);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var isChanged = false;

                try
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                    if (NodeIndexNameRow.Visible)
                    {
                        if (!nodeInfo.NodeIndexName.Equals(NodeIndexName.Text) && NodeIndexName.Text.Length != 0)
                        {
                            var nodeIndexNameList = DataProvider.NodeDao.GetNodeIndexNameList(PublishmentSystemId);
                            if (nodeIndexNameList.IndexOf(NodeIndexName.Text) != -1)
                            {
                                FailMessage("栏目修改失败，栏目索引已存在！");
                                return;
                            }
                        }
                    }

                    if (FilePathRow.Visible)
                    {
                        FilePath.Text = FilePath.Text.Trim();
                        if (!nodeInfo.FilePath.Equals(FilePath.Text) && FilePath.Text.Length != 0)
                        {
                            if (!DirectoryUtils.IsDirectoryNameCompliant(FilePath.Text))
                            {
                                FailMessage("栏目页面路径不符合系统要求！");
                                return;
                            }

                            if (PathUtils.IsDirectoryPath(FilePath.Text))
                            {
                                FilePath.Text = PageUtils.Combine(FilePath.Text, "index.html");
                            }

                            var filePathArrayList = DataProvider.NodeDao.GetAllFilePathByPublishmentSystemId(PublishmentSystemId);
                            if (filePathArrayList.IndexOf(FilePath.Text) != -1)
                            {
                                FailMessage("栏目修改失败，栏目页面路径已存在！");
                                return;
                            }
                        }
                    }

                    if (channelControl.Visible)
                    {
                        var extendedAttributes = new ExtendedAttributes();
                        var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
                        InputTypeParser.AddValuesToAttributes(ETableStyle.Channel, DataProvider.NodeDao.TableName, PublishmentSystemInfo, relatedIdentities, Request.Form, extendedAttributes.Attributes);
                        if (extendedAttributes.Attributes.Count > 0)
                        {
                            nodeInfo.Additional.SetExtendedAttribute(extendedAttributes.Attributes);
                        }
                    }

                    if (NodeNameRow.Visible)
                    {
                        nodeInfo.NodeName = NodeName.Text;
                    }
                    if (NodeIndexNameRow.Visible)
                    {
                        nodeInfo.NodeIndexName = NodeIndexName.Text;
                    }
                    if (FilePathRow.Visible)
                    {
                        nodeInfo.FilePath = FilePath.Text;
                    }

                    if (NodeGroupNameCollectionRow.Visible)
                    {
                        var list = new ArrayList();
                        foreach (ListItem item in NodeGroupNameCollection.Items)
                        {
                            if (item.Selected)
                            {
                                list.Add(item.Value);
                            }
                        }
                        nodeInfo.NodeGroupNameCollection = TranslateUtils.ObjectCollectionToString(list);
                    }
                    if (ImageUrlRow.Visible)
                    {
                        nodeInfo.ImageUrl = tbImageUrl.Text;
                    }
                    if (ContentRow.Visible)
                    {
                        nodeInfo.Content = StringUtility.TextEditorContentEncode(Request.Form[NodeAttribute.Content], PublishmentSystemInfo, PublishmentSystemInfo.Additional.IsSaveImageInTextEditor);
                    }
                    if (Keywords.Visible)
                    {
                        nodeInfo.Keywords = Keywords.Text;
                    }
                    if (Description.Visible)
                    {
                        nodeInfo.Description = Description.Text;
                    }



                    if (LinkUrlRow.Visible)
                    {
                        nodeInfo.LinkUrl = LinkUrl.Text;
                    }
                    if (LinkTypeRow.Visible)
                    {
                        nodeInfo.LinkType = ELinkTypeUtils.GetEnumType(LinkType.SelectedValue);
                    }
                    if (ChannelTemplateIDRow.Visible)
                    {
                        nodeInfo.ChannelTemplateId = (ChannelTemplateID.Items.Count > 0) ? int.Parse(ChannelTemplateID.SelectedValue) : 0;
                    }
                    if (ContentTemplateIDRow.Visible)
                    {
                        nodeInfo.ContentTemplateId = (ContentTemplateID.Items.Count > 0) ? int.Parse(ContentTemplateID.SelectedValue) : 0;
                    }

                    DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                    Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "修改栏目", $"栏目:{nodeInfo.NodeName}");

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"栏目修改失败：{ex.Message}");
                    LogUtils.AddErrorLog(ex);
                }

                if (isChanged)
                {
                    CreateManager.CreateChannel(PublishmentSystemId, _nodeId);

                    if (string.IsNullOrEmpty(_returnUrl))
                    {
                        PageUtils.CloseModalPage(Page);
                    }
                    else
                    {
                        PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
                    }
                }
            }
        }
    }
}

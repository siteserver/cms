using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelEdit : BasePageCms
    {
        public Control LinkURLRow;
        public Control LinkTypeRow;
        public Control ChannelTemplateIDRow;
        public Control FilePathRow;

        public TextBox NodeName;
        public TextBox NodeIndexName;
        public DropDownList ContentModelID;
        public TextBox LinkUrl;
        public CheckBoxList NodeGroupNameCollection;
        public RadioButtonList IsChannelAddable;
        public RadioButtonList IsContentAddable;
        public RadioButtonList TypeList;

        //NodeProperty
        public DropDownList LinkType;
        public DropDownList ChannelTemplateID;
        public DropDownList ContentTemplateID;
        public DropDownList dpChangeType;
        public DropDownList Priority;
        public TextBox NavigationPicPath;
        public TextBox FilePath;
        public TextBox ChannelFilePathRule;
        public TextBox ContentFilePathRule;

        public TextEditorControl Content;
        public TextBox Keywords;
        public TextBox Description;
        public ChannelAuxiliaryControl channelControl;

        public Button CreateChannelRule;
        public Button CreateContentRule;
        public Button SelectImage;
        public Button UploadImage;
        public Button Submit;

        private int _nodeId;
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public string PreviewImageSrc
        {
            get
            {
                if (!string.IsNullOrEmpty(NavigationPicPath.Text))
                {
                    var extension = PathUtils.GetExtension(NavigationPicPath.Text);
                    if (EFileSystemTypeUtils.IsImage(extension))
                    {
                        return PageUtility.ParseNavigationUrl(PublishmentSystemInfo, NavigationPicPath.Text);
                    }
                    else if (EFileSystemTypeUtils.IsFlash(extension))
                    {
                        return SiteServerAssets.GetIconUrl("flash.jpg");
                    }
                    else if (EFileSystemTypeUtils.IsPlayer(extension))
                    {
                        return SiteServerAssets.GetIconUrl("player.gif");
                    }
                }
                return SiteServerAssets.GetIconUrl("empty.gif");
            }
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl");

            _nodeId = Body.GetQueryInt("NodeID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            if (Body.GetQueryString("CanNotEdit") == null && Body.GetQueryString("UncheckedChannel") == null)
            {
                if (!HasChannelPermissions(_nodeId, AppManager.Cms.Permission.Channel.ChannelEdit))
                {
                    PageUtils.RedirectToErrorPage("您没有修改栏目的权限！");
                    return;
                }
            }
            if (Body.IsQueryExists("CanNotEdit"))
            {
                Submit.Visible = false;
            }

            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            if (nodeInfo != null)
            {
                channelControl = (ChannelAuxiliaryControl)FindControl("ControlForAuxiliary");
                if (!IsPostBack)
                {
                    BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "编辑栏目", string.Empty);

                    var contentModelInfoList = ContentModelManager.GetContentModelInfoList(PublishmentSystemInfo);
                    foreach (var modelInfo in contentModelInfoList)
                    {
                        ContentModelID.Items.Add(new ListItem(modelInfo.ModelName, modelInfo.ModelId));
                    }
                    ControlUtils.SelectListItems(ContentModelID, nodeInfo.ContentModelId);

                    channelControl.SetParameters(nodeInfo.Additional.Attributes, true, IsPostBack);

                    NavigationPicPath.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", PublishmentSystemInfo.PublishmentSystemUrl));

                    var showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, true, ChannelFilePathRule.ClientID);
                    CreateChannelRule.Attributes.Add("onclick", showPopWinString);

                    showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, false, ContentFilePathRule.ClientID);
                    CreateContentRule.Attributes.Add("onclick", showPopWinString);

                    showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, NavigationPicPath.ClientID);
                    SelectImage.Attributes.Add("onclick", showPopWinString);

                    showPopWinString = ModalUploadImage.GetOpenWindowString(PublishmentSystemId, NavigationPicPath.ClientID);
                    UploadImage.Attributes.Add("onclick", showPopWinString);
                    IsChannelAddable.Items[0].Value = true.ToString();
                    IsChannelAddable.Items[1].Value = false.ToString();
                    IsContentAddable.Items[0].Value = true.ToString();
                    IsContentAddable.Items[1].Value = false.ToString();

                    ELinkTypeUtils.AddListItems(LinkType);

                    NodeGroupNameCollection.DataSource = DataProvider.NodeGroupDao.GetDataSource(PublishmentSystemId);

                    ChannelTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);

                    ContentTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);

                    DataBind();

                    ChannelTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                    ControlUtils.SelectListItems(ChannelTemplateID, nodeInfo.ChannelTemplateId.ToString());

                    ContentTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                    ControlUtils.SelectListItems(ContentTemplateID, nodeInfo.ContentTemplateId.ToString());

                    NodeName.Text = nodeInfo.NodeName;
                    NodeIndexName.Text = nodeInfo.NodeIndexName;
                    LinkUrl.Text = nodeInfo.LinkUrl;

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
                    FilePath.Text = nodeInfo.FilePath;
                    ChannelFilePathRule.Text = nodeInfo.ChannelFilePathRule;
                    ContentFilePathRule.Text = nodeInfo.ContentFilePathRule;

                    //NodeProperty
                    ControlUtils.SelectListItems(LinkType, ELinkTypeUtils.GetValue(nodeInfo.LinkType));
                    ControlUtils.SelectListItems(IsChannelAddable, nodeInfo.Additional.IsChannelAddable.ToString());
                    ControlUtils.SelectListItems(IsContentAddable, nodeInfo.Additional.IsContentAddable.ToString());

                    NavigationPicPath.Text = nodeInfo.ImageUrl;

                    var formCollection = new NameValueCollection();
                    formCollection[NodeAttribute.Content] = nodeInfo.Content;
                    Content.SetParameters(PublishmentSystemInfo, NodeAttribute.Content, formCollection, true, IsPostBack);

                    Keywords.Text = nodeInfo.Keywords;
                    Description.Text = nodeInfo.Description;

                    //this.Content.PublishmentSystemID = base.PublishmentSystemID;
                    //this.Content.Text = StringUtility.TextEditorContentDecode(nodeInfo.Content, ConfigUtils.Instance.ApplicationPath, base.PublishmentSystemInfo.PublishmentSystemUrl);

                    if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
                    {
                        LinkURLRow.Visible = false;
                        LinkTypeRow.Visible = false;
                        ChannelTemplateIDRow.Visible = false;
                        FilePathRow.Visible = false;
                    }
                }
                else
                {
                    channelControl.SetParameters(Request.Form, true, IsPostBack);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                NodeInfo nodeInfo;
                try
                {

                    nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
                    if (!nodeInfo.NodeIndexName.Equals(NodeIndexName.Text) && NodeIndexName.Text.Length != 0)
                    {
                        var nodeIndexNameList = DataProvider.NodeDao.GetNodeIndexNameList(PublishmentSystemId);
                        if (nodeIndexNameList.IndexOf(NodeIndexName.Text) != -1)
                        {
                            FailMessage("栏目属性修改失败，栏目索引已存在！");
                            return;
                        }
                    }

                    if (nodeInfo.ContentModelId != ContentModelID.SelectedValue)
                    {
                        nodeInfo.ContentModelId = ContentModelID.SelectedValue;
                        nodeInfo.ContentNum = BaiRongDataProvider.ContentDao.GetCount(NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo.ContentModelId), nodeInfo.NodeId);
                    }

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

                    if (!string.IsNullOrEmpty(ChannelFilePathRule.Text))
                    {
                        var filePathRule = ChannelFilePathRule.Text.Replace("|", string.Empty);
                        if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                        {
                            FailMessage("栏目页面命名规则不符合系统要求！");
                            return;
                        }
                        if (PathUtils.IsDirectoryPath(filePathRule))
                        {
                            FailMessage("栏目页面命名规则必须包含生成文件的后缀！");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(ContentFilePathRule.Text))
                    {
                        var filePathRule = ContentFilePathRule.Text.Replace("|", string.Empty);
                        if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                        {
                            FailMessage("内容页面命名规则不符合系统要求！");
                            return;
                        }
                        if (PathUtils.IsDirectoryPath(filePathRule))
                        {
                            FailMessage("内容页面命名规则必须包含生成文件的后缀！");
                            return;
                        }
                    }

                    var extendedAttributes = new ExtendedAttributes();
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
                    InputTypeParser.AddValuesToAttributes(ETableStyle.Channel, DataProvider.NodeDao.TableName, PublishmentSystemInfo, relatedIdentities, Request.Form, extendedAttributes.Attributes);
                    var attributes = extendedAttributes.Attributes;
                    foreach (string key in attributes)
                    {
                        nodeInfo.Additional.SetExtendedAttribute(key, attributes[key]);
                    }

                    nodeInfo.NodeName = NodeName.Text;
                    nodeInfo.NodeIndexName = NodeIndexName.Text;
                    nodeInfo.FilePath = FilePath.Text;
                    nodeInfo.ChannelFilePathRule = ChannelFilePathRule.Text;
                    nodeInfo.ContentFilePathRule = ContentFilePathRule.Text;

                    var list = new ArrayList();
                    foreach (ListItem item in NodeGroupNameCollection.Items)
                    {
                        if (item.Selected)
                        {
                            list.Add(item.Value);
                        }
                    }
                    nodeInfo.NodeGroupNameCollection = TranslateUtils.ObjectCollectionToString(list);
                    nodeInfo.ImageUrl = NavigationPicPath.Text;
                    nodeInfo.Content = StringUtility.TextEditorContentEncode(Request.Form[NodeAttribute.Content], PublishmentSystemInfo, PublishmentSystemInfo.Additional.IsSaveImageInTextEditor);

                    nodeInfo.Keywords = Keywords.Text;
                    nodeInfo.Description = Description.Text;

                    nodeInfo.Additional.IsChannelAddable = TranslateUtils.ToBool(IsChannelAddable.SelectedValue);
                    nodeInfo.Additional.IsContentAddable = TranslateUtils.ToBool(IsContentAddable.SelectedValue);

                    nodeInfo.LinkUrl = LinkUrl.Text;
                    nodeInfo.LinkType = ELinkTypeUtils.GetEnumType(LinkType.SelectedValue);
                    nodeInfo.ChannelTemplateId = (ChannelTemplateID.Items.Count > 0) ? int.Parse(ChannelTemplateID.SelectedValue) : 0;
                    nodeInfo.ContentTemplateId = (ContentTemplateID.Items.Count > 0) ? int.Parse(ContentTemplateID.SelectedValue) : 0;

                    DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"栏目修改失败：{ex.Message}");
                    LogUtils.AddErrorLog(ex);
                    return;
                }

                CreateManager.CreateChannel(PublishmentSystemId, nodeInfo.NodeId);

                Body.AddSiteLog(PublishmentSystemId, "修改栏目", $"栏目:{NodeName.Text}");

                SuccessMessage("栏目修改成功！");
                PageUtils.Redirect(_returnUrl);
            }
        }

        public string ReturnUrl => _returnUrl;
    }
}
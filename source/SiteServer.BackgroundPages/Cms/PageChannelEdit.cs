using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelEdit : BasePageCms
    {
        public TextBox TbNodeName;
        public TextBox TbNodeIndexName;
        public DropDownList DdlContentModelId;
        public PlaceHolder PhPlugins;
        public CheckBoxList CblPlugins;
        public CheckBoxList CblNodeGroupNameCollection;
        public RadioButtonList RblIsChannelAddable;
        public RadioButtonList RblIsContentAddable;
        public TextBox TbLinkUrl;
        public DropDownList DdlLinkType;
        public DropDownList DdlTaxisType;
        public DropDownList DdlChannelTemplateId;
        public DropDownList DdlContentTemplateId;
        public TextBox TbImageUrl;
        public TextBox TbFilePath;
        public TextBox TbChannelFilePathRule;
        public TextBox TbContentFilePathRule;
        public TextEditorControl TecContent;
        public TextBox TbKeywords;
        public TextBox TbDescription;
        public ChannelAuxiliaryControl CacAttributes;
        public Button BtnCreateChannelRule;
        public Button BtnCreateContentRule;
        public Button BtnSelectImage;
        public Button BtnUploadImage;
        public Button BtnSubmit;

        private int _nodeId;

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
                if (!string.IsNullOrEmpty(TbImageUrl.Text))
                {
                    var extension = PathUtils.GetExtension(TbImageUrl.Text);
                    if (EFileSystemTypeUtils.IsImage(extension))
                    {
                        return PageUtility.ParseNavigationUrl(PublishmentSystemInfo, TbImageUrl.Text);
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
            ReturnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            if (Body.GetQueryString("CanNotEdit") == null && Body.GetQueryString("UncheckedChannel") == null)
            {
                if (!HasChannelPermissions(_nodeId, AppManager.Permissions.Channel.ChannelEdit))
                {
                    PageUtils.RedirectToErrorPage("您没有修改栏目的权限！");
                    return;
                }
            }
            if (Body.IsQueryExists("CanNotEdit"))
            {
                BtnSubmit.Visible = false;
            }

            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            if (nodeInfo != null)
            {
                if (!IsPostBack)
                {
                    BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "编辑栏目", string.Empty);

                    DdlContentModelId.Items.Add(new ListItem("<默认>", string.Empty));
                    var contentTables = PluginCache.GetEnabledPluginMetadatas<IContentTable>();
                    foreach (var contentTable in contentTables)
                    {
                        DdlContentModelId.Items.Add(new ListItem($"插件：{contentTable.DisplayName}", contentTable.Id));
                    }
                    ControlUtils.SelectListItems(DdlContentModelId, nodeInfo.ContentModelId);

                    var pluginChannels = PluginCache.GetAllChannels(false);
                    if (pluginChannels.Count > 0)
                    {
                        foreach (var pluginMetadata in pluginChannels)
                        {
                            CblPlugins.Items.Add(new ListItem(pluginMetadata.DisplayName, pluginMetadata.Id));
                        }
                    }
                    else
                    {
                        PhPlugins.Visible = false;
                    }

                    CacAttributes.SetParameters(nodeInfo.Additional.GetExtendedAttributes(), true, IsPostBack);

                    TbImageUrl.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", PublishmentSystemInfo.PublishmentSystemUrl));

                    var showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, true, TbChannelFilePathRule.ClientID);
                    BtnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

                    showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, false, TbContentFilePathRule.ClientID);
                    BtnCreateContentRule.Attributes.Add("onclick", showPopWinString);

                    showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, TbImageUrl.ClientID);
                    BtnSelectImage.Attributes.Add("onclick", showPopWinString);

                    showPopWinString = ModalUploadImage.GetOpenWindowString(PublishmentSystemId, TbImageUrl.ClientID);
                    BtnUploadImage.Attributes.Add("onclick", showPopWinString);
                    RblIsChannelAddable.Items[0].Value = true.ToString();
                    RblIsChannelAddable.Items[1].Value = false.ToString();
                    RblIsContentAddable.Items[0].Value = true.ToString();
                    RblIsContentAddable.Items[1].Value = false.ToString();

                    ELinkTypeUtils.AddListItems(DdlLinkType);

                    ETaxisTypeUtils.AddListItemsForChannelEdit(DdlTaxisType);
                    ControlUtils.SelectListItems(DdlTaxisType, nodeInfo.Additional.DefaultTaxisType);

                    CblNodeGroupNameCollection.DataSource = DataProvider.NodeGroupDao.GetDataSource(PublishmentSystemId);

                    DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);

                    DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);

                    DataBind();

                    DdlChannelTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                    ControlUtils.SelectListItems(DdlChannelTemplateId, nodeInfo.ChannelTemplateId.ToString());

                    DdlContentTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                    ControlUtils.SelectListItems(DdlContentTemplateId, nodeInfo.ContentTemplateId.ToString());

                    TbNodeName.Text = nodeInfo.NodeName;
                    TbNodeIndexName.Text = nodeInfo.NodeIndexName;
                    TbLinkUrl.Text = nodeInfo.LinkUrl;

                    foreach (ListItem item in CblNodeGroupNameCollection.Items)
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
                    TbFilePath.Text = nodeInfo.FilePath;
                    TbChannelFilePathRule.Text = nodeInfo.ChannelFilePathRule;
                    TbContentFilePathRule.Text = nodeInfo.ContentFilePathRule;

                    ControlUtils.SelectListItems(DdlLinkType, ELinkTypeUtils.GetValue(nodeInfo.LinkType));
                    ControlUtils.SelectListItems(RblIsChannelAddable, nodeInfo.Additional.IsChannelAddable.ToString());
                    ControlUtils.SelectListItems(RblIsContentAddable, nodeInfo.Additional.IsContentAddable.ToString());

                    TbImageUrl.Text = nodeInfo.ImageUrl;

                    var formCollection = new NameValueCollection
                    {
                        [NodeAttribute.Content] = nodeInfo.Content
                    };
                    TecContent.SetParameters(PublishmentSystemInfo, NodeAttribute.Content, formCollection, true, IsPostBack);

                    TbKeywords.Text = nodeInfo.Keywords;
                    TbDescription.Text = nodeInfo.Description;

                    //this.Content.PublishmentSystemID = base.PublishmentSystemID;
                    //this.Content.Text = StringUtility.TextEditorContentDecode(nodeInfo.Content, ConfigUtils.Instance.ApplicationPath, base.PublishmentSystemInfo.PublishmentSystemUrl);
                }
                else
                {
                    CacAttributes.SetParameters(Request.Form, true, IsPostBack);
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
                    if (!nodeInfo.NodeIndexName.Equals(TbNodeIndexName.Text) && TbNodeIndexName.Text.Length != 0)
                    {
                        var nodeIndexNameList = DataProvider.NodeDao.GetNodeIndexNameList(PublishmentSystemId);
                        if (nodeIndexNameList.IndexOf(TbNodeIndexName.Text) != -1)
                        {
                            FailMessage("栏目属性修改失败，栏目索引已存在！");
                            return;
                        }
                    }

                    if (nodeInfo.ContentModelId != DdlContentModelId.SelectedValue)
                    {
                        nodeInfo.ContentModelId = DdlContentModelId.SelectedValue;
                        nodeInfo.ContentNum = BaiRongDataProvider.ContentDao.GetCount(NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo.ContentModelId), nodeInfo.NodeId);
                    }

                    nodeInfo.Additional.PluginIds = ControlUtils.GetSelectedListControlValueCollection(CblPlugins);

                    TbFilePath.Text = TbFilePath.Text.Trim();
                    if (!nodeInfo.FilePath.Equals(TbFilePath.Text) && TbFilePath.Text.Length != 0)
                    {
                        if (!DirectoryUtils.IsDirectoryNameCompliant(TbFilePath.Text))
                        {
                            FailMessage("栏目页面路径不符合系统要求！");
                            return;
                        }

                        if (PathUtils.IsDirectoryPath(TbFilePath.Text))
                        {
                            TbFilePath.Text = PageUtils.Combine(TbFilePath.Text, "index.html");
                        }

                        var filePathArrayList = DataProvider.NodeDao.GetAllFilePathByPublishmentSystemId(PublishmentSystemId);
                        if (filePathArrayList.IndexOf(TbFilePath.Text) != -1)
                        {
                            FailMessage("栏目修改失败，栏目页面路径已存在！");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(TbChannelFilePathRule.Text))
                    {
                        var filePathRule = TbChannelFilePathRule.Text.Replace("|", string.Empty);
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

                    if (!string.IsNullOrEmpty(TbContentFilePathRule.Text))
                    {
                        var filePathRule = TbContentFilePathRule.Text.Replace("|", string.Empty);
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
                    BackgroundInputTypeParser.AddValuesToAttributes(ETableStyle.Channel, DataProvider.NodeDao.TableName, PublishmentSystemInfo, relatedIdentities, Request.Form, extendedAttributes.GetExtendedAttributes());
                    var attributes = extendedAttributes.GetExtendedAttributes();
                    foreach (string key in attributes)
                    {
                        nodeInfo.Additional.SetExtendedAttribute(key, attributes[key]);
                    }

                    nodeInfo.NodeName = TbNodeName.Text;
                    nodeInfo.NodeIndexName = TbNodeIndexName.Text;
                    nodeInfo.FilePath = TbFilePath.Text;
                    nodeInfo.ChannelFilePathRule = TbChannelFilePathRule.Text;
                    nodeInfo.ContentFilePathRule = TbContentFilePathRule.Text;

                    var list = new ArrayList();
                    foreach (ListItem item in CblNodeGroupNameCollection.Items)
                    {
                        if (item.Selected)
                        {
                            list.Add(item.Value);
                        }
                    }
                    nodeInfo.NodeGroupNameCollection = TranslateUtils.ObjectCollectionToString(list);
                    nodeInfo.ImageUrl = TbImageUrl.Text;
                    nodeInfo.Content = StringUtility.TextEditorContentEncode(Request.Form[NodeAttribute.Content], PublishmentSystemInfo, PublishmentSystemInfo.Additional.IsSaveImageInTextEditor);

                    nodeInfo.Keywords = TbKeywords.Text;
                    nodeInfo.Description = TbDescription.Text;

                    nodeInfo.Additional.IsChannelAddable = TranslateUtils.ToBool(RblIsChannelAddable.SelectedValue);
                    nodeInfo.Additional.IsContentAddable = TranslateUtils.ToBool(RblIsContentAddable.SelectedValue);

                    nodeInfo.LinkUrl = TbLinkUrl.Text;
                    nodeInfo.LinkType = ELinkTypeUtils.GetEnumType(DdlLinkType.SelectedValue);
                    nodeInfo.Additional.DefaultTaxisType = ETaxisTypeUtils.GetValue(ETaxisTypeUtils.GetEnumType(DdlTaxisType.SelectedValue));
                    nodeInfo.ChannelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                    nodeInfo.ContentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;

                    DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"栏目修改失败：{ex.Message}");
                    LogUtils.AddErrorLog(ex);
                    return;
                }

                CreateManager.CreateChannel(PublishmentSystemId, nodeInfo.NodeId);

                Body.AddSiteLog(PublishmentSystemId, "修改栏目", $"栏目:{TbNodeName.Text}");

                SuccessMessage("栏目修改成功！");
                PageUtils.Redirect(ReturnUrl);
            }
        }

        public string ReturnUrl { get; private set; }
    }
}
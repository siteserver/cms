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
    public class PageChannelAdd : BasePageCms
    {
        public DropDownList DdlParentNodeId;
        public TextBox TbNodeName;
        public TextBox TbNodeIndexName;
        public DropDownList DdlContentModelId;
        public PlaceHolder PhPlugins;
        public CheckBoxList CblPlugins;
        public TextBox TbLinkUrl;
        public CheckBoxList CblNodeGroupNameCollection;
        public DropDownList DdlLinkType;
        public DropDownList DdlChannelTemplateId;
        public DropDownList DdlContentTemplateId;
        public RadioButtonList RblIsChannelAddable;
        public RadioButtonList RblIsContentAddable;
        public TextBox TbNavigationPicPath;
        public TextBox TbFilePath;
        public TextBox TbChannelFilePathRule;
        public TextBox TbContentFilePathRule;

        public TextEditorControl TbContent;
        public TextBox TbKeywords;
        public TextBox TbDescription;

        public ChannelAuxiliaryControl CacChannelControl;

        public Button BtnCreateChannelRule;
        public Button BtnCreateContentRule;
        public Button BtnSelectImage;
        public Button BtnUploadImage;

        private int _nodeId;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString() },
                {"NodeID", nodeId.ToString() },
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl) }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl");
            _nodeId = Body.GetQueryInt("NodeID");
            ReturnUrl = StringUtils.ValueFromUrl(PageUtils.FilterSqlAndXss(Body.GetQueryString("ReturnUrl")));
            //if (!base.HasChannelPermissions(this.nodeID, AppManager.CMS.Permission.Channel.ChannelAdd))
            //{
            //    PageUtils.RedirectToErrorPage("您没有添加栏目的权限！");
            //    return;
            //}

            var parentNodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            if (parentNodeInfo.Additional.IsChannelAddable == false)
            {
                PageUtils.RedirectToErrorPage("此栏目不能添加子栏目！");
                return;
            }

            //CacChannelControl = FindControl("CacChannelControl") as ChannelAuxiliaryControl;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "添加栏目", string.Empty);

                NodeManager.AddListItems(DdlParentNodeId.Items, PublishmentSystemInfo, true, true, Body.AdministratorName);
                ControlUtils.SelectListItems(DdlParentNodeId, _nodeId.ToString());

                DdlContentModelId.Items.Add(new ListItem("<默认>", string.Empty));
                var contentTables = PluginCache.GetEnabledPluginMetadatas<IContentTable>();
                foreach (var contentTable in contentTables)
                {
                    DdlContentModelId.Items.Add(new ListItem($"插件：{contentTable.DisplayName}", contentTable.Id));
                }
                ControlUtils.SelectListItems(DdlContentModelId, parentNodeInfo.ContentModelId);

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

                CacChannelControl.SetParameters(null, false, IsPostBack);

                TbNavigationPicPath.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", PublishmentSystemInfo.PublishmentSystemUrl));

                var showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, true, TbChannelFilePathRule.ClientID);
                BtnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, false, TbContentFilePathRule.ClientID);
                BtnCreateContentRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, TbNavigationPicPath.ClientID);
                BtnSelectImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImage.GetOpenWindowString(PublishmentSystemId, TbNavigationPicPath.ClientID);
                BtnUploadImage.Attributes.Add("onclick", showPopWinString);

                RblIsChannelAddable.Items[0].Value = true.ToString();
                RblIsChannelAddable.Items[1].Value = false.ToString();
                RblIsContentAddable.Items[0].Value = true.ToString();
                RblIsContentAddable.Items[1].Value = false.ToString();

                ELinkTypeUtils.AddListItems(DdlLinkType);

                CblNodeGroupNameCollection.DataSource = DataProvider.NodeGroupDao.GetDataSource(PublishmentSystemId);
                DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
                DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);

                DataBind();

                DdlChannelTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlChannelTemplateId.Items[0].Selected = true;

                DdlContentTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlContentTemplateId.Items[0].Selected = true;
                TbContent.SetParameters(PublishmentSystemInfo, NodeAttribute.Content, null, false, IsPostBack);
            }
            else
            {
                CacChannelControl.SetParameters(Request.Form, false, IsPostBack);
            }
        }

        public void DdlParentNodeId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var theNodeId = TranslateUtils.ToInt(DdlParentNodeId.SelectedValue);
            if (theNodeId == 0)
            {
                theNodeId = _nodeId;
            }
            PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, theNodeId, Body.GetQueryString("ReturnUrl")));
        }

        public string PreviewImageSrc
        {
            get
            {
                if (string.IsNullOrEmpty(TbNavigationPicPath.Text)) return SiteServerAssets.GetIconUrl("empty.gif");

                var extension = PathUtils.GetExtension(TbNavigationPicPath.Text);
                if (EFileSystemTypeUtils.IsImage(extension))
                {
                    return PageUtility.ParseNavigationUrl(PublishmentSystemInfo, TbNavigationPicPath.Text);
                }
                if (EFileSystemTypeUtils.IsFlash(extension))
                {
                    return SiteServerAssets.GetIconUrl("flash.jpg");
                }
                if (EFileSystemTypeUtils.IsPlayer(extension))
                {
                    return SiteServerAssets.GetIconUrl("player.gif");
                }
                return SiteServerAssets.GetIconUrl("empty.gif");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            int insertNodeId;
            try
            {
                var nodeId = Body.GetQueryInt("NodeID");
                var nodeInfo = new NodeInfo
                {
                    ParentId = nodeId,
                    ContentModelId = DdlContentModelId.SelectedValue
                };

                nodeInfo.Additional.PluginIds = ControlUtils.GetSelectedListControlValueCollection(CblPlugins);

                if (TbNodeIndexName.Text.Length != 0)
                {
                    var nodeIndexNameArrayList = DataProvider.NodeDao.GetNodeIndexNameList(PublishmentSystemId);
                    if (nodeIndexNameArrayList.IndexOf(TbNodeIndexName.Text) != -1)
                    {
                        FailMessage("栏目添加失败，栏目索引已存在！");
                        return;
                    }
                }

                if (TbFilePath.Text.Length != 0)
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
                        FailMessage("栏目添加失败，栏目页面路径已存在！");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(TbChannelFilePathRule.Text))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(TbChannelFilePathRule.Text))
                    {
                        FailMessage("栏目页面命名规则不符合系统要求！");
                        return;
                    }
                    if (PathUtils.IsDirectoryPath(TbChannelFilePathRule.Text))
                    {
                        FailMessage("栏目页面命名规则必须包含生成文件的后缀！");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(TbContentFilePathRule.Text))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(TbContentFilePathRule.Text))
                    {
                        FailMessage("内容页面命名规则不符合系统要求！");
                        return;
                    }
                    if (PathUtils.IsDirectoryPath(TbContentFilePathRule.Text))
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
                nodeInfo.ImageUrl = TbNavigationPicPath.Text;
                nodeInfo.Content = StringUtility.TextEditorContentEncode(Request.Form[NodeAttribute.Content], PublishmentSystemInfo, PublishmentSystemInfo.Additional.IsSaveImageInTextEditor);
                nodeInfo.Keywords = TbKeywords.Text;
                nodeInfo.Description = TbDescription.Text;
                nodeInfo.Additional.IsChannelAddable = TranslateUtils.ToBool(RblIsChannelAddable.SelectedValue);
                nodeInfo.Additional.IsContentAddable = TranslateUtils.ToBool(RblIsContentAddable.SelectedValue);

                nodeInfo.LinkUrl = TbLinkUrl.Text;
                nodeInfo.LinkType = ELinkTypeUtils.GetEnumType(DdlLinkType.SelectedValue);
                nodeInfo.ChannelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                nodeInfo.ContentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;

                nodeInfo.AddDate = DateTime.Now;
                insertNodeId = DataProvider.NodeDao.InsertNodeInfo(nodeInfo);
                //栏目选择投票样式后，内容
            }
            catch (Exception ex)
            {
                FailMessage(ex, $"栏目添加失败：{ex.Message}");
                LogUtils.AddErrorLog(ex);
                return;
            }

            CreateManager.CreateChannel(PublishmentSystemId, insertNodeId);

            Body.AddSiteLog(PublishmentSystemId, "添加栏目", $"栏目:{TbNodeName.Text}");

            SuccessMessage("栏目添加成功！");
            AddWaitAndRedirectScript(ReturnUrl);
        }

        public string ReturnUrl { get; private set; }
    }
}

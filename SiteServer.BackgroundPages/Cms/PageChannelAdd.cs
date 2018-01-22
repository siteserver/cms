using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelAdd : BasePageCms
    {
        public DropDownList DdlParentNodeId;
        public TextBox TbNodeName;
        public TextBox TbNodeIndexName;
        public DropDownList DdlContentModelPluginId;
        public PlaceHolder PhContentRelatedPluginIds;
        public CheckBoxList CblContentRelatedPluginIds;
        public TextBox TbLinkUrl;
        public CheckBoxList CblNodeGroupNameCollection;
        public DropDownList DdlLinkType;
        public DropDownList DdlTaxisType;
        public DropDownList DdlChannelTemplateId;
        public DropDownList DdlContentTemplateId;
        public RadioButtonList RblIsChannelAddable;
        public RadioButtonList RblIsContentAddable;
        public TextBox TbImageUrl;
        public TextBox TbFilePath;
        public TextBox TbChannelFilePathRule;
        public TextBox TbContentFilePathRule;

        public TextEditorControl TbContent;
        public TextBox TbKeywords;
        public TextBox TbDescription;

        public ChannelAuxiliaryControl CacAttributes;

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

            CacAttributes.PublishmentSystemInfo = PublishmentSystemInfo;
            CacAttributes.NodeId = _nodeId;

            if (!IsPostBack)
            {
                NodeManager.AddListItems(DdlParentNodeId.Items, PublishmentSystemInfo, true, true, Body.AdminName);
                ControlUtils.SelectSingleItem(DdlParentNodeId, _nodeId.ToString());

                DdlContentModelPluginId.Items.Add(new ListItem("<默认>", string.Empty));
                var contentTables = PluginManager.GetContentModelPlugins();
                foreach (var contentTable in contentTables)
                {
                    DdlContentModelPluginId.Items.Add(new ListItem(contentTable.Title, contentTable.Id));
                }
                ControlUtils.SelectSingleItem(DdlContentModelPluginId, parentNodeInfo.ContentModelPluginId);

                var plugins = PluginManager.GetAllContentRelatedPlugins(false);
                if (plugins.Count > 0)
                {
                    foreach (var pluginMetadata in plugins)
                    {
                        CblContentRelatedPluginIds.Items.Add(new ListItem(pluginMetadata.Title, pluginMetadata.Id));
                    }
                }
                else
                {
                    PhContentRelatedPluginIds.Visible = false;
                }

                CacAttributes.Attributes = new ExtendedAttributes();

                TbImageUrl.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", PublishmentSystemInfo.Additional.WebUrl));

                var showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, true, TbChannelFilePathRule.ClientID);
                BtnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, false, TbContentFilePathRule.ClientID);
                BtnCreateContentRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, TbImageUrl.ClientID);
                BtnSelectImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImage.GetOpenWindowString(PublishmentSystemId, TbImageUrl.ClientID);
                BtnUploadImage.Attributes.Add("onclick", showPopWinString);

                ELinkTypeUtils.AddListItems(DdlLinkType);

                ETaxisTypeUtils.AddListItemsForChannelEdit(DdlTaxisType);
                ControlUtils.SelectSingleItem(DdlTaxisType, ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc));

                CblNodeGroupNameCollection.DataSource = DataProvider.NodeGroupDao.GetDataSource(PublishmentSystemId);
                DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
                DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);

                DataBind();

                DdlChannelTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlChannelTemplateId.Items[0].Selected = true;

                DdlContentTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlContentTemplateId.Items[0].Selected = true;
                TbContent.SetParameters(PublishmentSystemInfo, NodeAttribute.Content, string.Empty);
            }
            else
            {
                CacAttributes.Attributes = new ExtendedAttributes(Request.Form);
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
                if (string.IsNullOrEmpty(TbImageUrl.Text)) return SiteServerAssets.GetIconUrl("empty.gif");

                var extension = PathUtils.GetExtension(TbImageUrl.Text);
                if (EFileSystemTypeUtils.IsImage(extension))
                {
                    return PageUtility.ParseNavigationUrl(PublishmentSystemInfo, TbImageUrl.Text, true);
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
                    ContentModelPluginId = DdlContentModelPluginId.SelectedValue,
                    ContentRelatedPluginIds =
                        ControlUtils.GetSelectedListControlValueCollection(CblContentRelatedPluginIds)
                };

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
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.NodeDao.TableName, relatedIdentities);
                BackgroundInputTypeParser.SaveAttributes(extendedAttributes, PublishmentSystemInfo, styleInfoList, Request.Form, null);
                var attributes = extendedAttributes.ToNameValueCollection();
                nodeInfo.Additional.Load(attributes);
                //foreach (string key in attributes)
                //{
                //    nodeInfo.Additional.SetExtendedAttribute(key, attributes[key]);
                //}

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
                nodeInfo.Content = ContentUtility.TextEditorContentEncode(PublishmentSystemInfo, Request.Form[NodeAttribute.Content]);
                nodeInfo.Keywords = TbKeywords.Text;
                nodeInfo.Description = TbDescription.Text;
                nodeInfo.Additional.IsChannelAddable = TranslateUtils.ToBool(RblIsChannelAddable.SelectedValue);
                nodeInfo.Additional.IsContentAddable = TranslateUtils.ToBool(RblIsContentAddable.SelectedValue);

                nodeInfo.LinkUrl = TbLinkUrl.Text;
                nodeInfo.LinkType = DdlLinkType.SelectedValue;

                nodeInfo.Additional.DefaultTaxisType = ETaxisTypeUtils.GetValue(ETaxisTypeUtils.GetEnumType(DdlTaxisType.SelectedValue));

                nodeInfo.ChannelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                nodeInfo.ContentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;

                nodeInfo.AddDate = DateTime.Now;
                insertNodeId = DataProvider.NodeDao.InsertNodeInfo(nodeInfo);
                //栏目选择投票样式后，内容
            }
            catch (Exception ex)
            {
                LogUtils.AddSystemErrorLog(ex);
                FailMessage(ex, $"栏目添加失败：{ex.Message}");
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

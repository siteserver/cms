using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelAdd : BasePageCms
    {
        public DropDownList DdlParentChannelId;
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

        private int _channelId;

        public static string GetRedirectUrl(int siteId, int channelId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelAdd), new NameValueCollection
            {
                {"channelId", channelId.ToString() },
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl) }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId", "ReturnUrl");
            _channelId = AuthRequest.GetQueryInt("channelId");
            ReturnUrl = StringUtils.ValueFromUrl(AttackUtils.FilterSqlAndXss(AuthRequest.GetQueryString("ReturnUrl")));
            //if (!base.HasChannelPermissions(this.channelId, AppManager.CMS.Permission.Channel.ChannelAdd))
            //{
            //    PageUtils.RedirectToErrorPage("您没有添加栏目的权限！");
            //    return;
            //}

            var parentNodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            if (parentNodeInfo.Additional.IsChannelAddable == false)
            {
                PageUtils.RedirectToErrorPage("此栏目不能添加子栏目！");
                return;
            }

            CacAttributes.SiteInfo = SiteInfo;
            CacAttributes.ChannelId = _channelId;

            if (!IsPostBack)
            {
                ChannelManager.AddListItems(DdlParentChannelId.Items, SiteInfo, true, true, AuthRequest.AdminPermissionsImpl);
                ControlUtils.SelectSingleItem(DdlParentChannelId, _channelId.ToString());

                DdlContentModelPluginId.Items.Add(new ListItem("<默认>", string.Empty));
                var contentTables = PluginContentManager.GetContentModelPlugins();
                foreach (var contentTable in contentTables)
                {
                    DdlContentModelPluginId.Items.Add(new ListItem(contentTable.Title, contentTable.Id));
                }
                ControlUtils.SelectSingleItem(DdlContentModelPluginId, parentNodeInfo.ContentModelPluginId);

                var plugins = PluginContentManager.GetAllContentRelatedPlugins(false);
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

                CacAttributes.Attributes = new AttributesImpl();

                TbImageUrl.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", SiteInfo.Additional.WebUrl));

                var showPopWinString = ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, true, TbChannelFilePathRule.ClientID);
                BtnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, false, TbContentFilePathRule.ClientID);
                BtnCreateContentRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectImage.GetOpenWindowString(SiteInfo, TbImageUrl.ClientID);
                BtnSelectImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImage.GetOpenWindowString(SiteId, TbImageUrl.ClientID);
                BtnUploadImage.Attributes.Add("onclick", showPopWinString);

                ELinkTypeUtils.AddListItems(DdlLinkType);

                ETaxisTypeUtils.AddListItemsForChannelEdit(DdlTaxisType);
                ControlUtils.SelectSingleItem(DdlTaxisType, ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc));

                ControlUtils.AddListControlItems(CblNodeGroupNameCollection, ChannelGroupManager.GetGroupNameList(SiteId));
                //CblNodeGroupNameCollection.DataSource = DataProvider.ChannelGroupDao.GetDataSource(SiteId);

                DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ChannelTemplate);
                DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ContentTemplate);

                DataBind();

                DdlChannelTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlChannelTemplateId.Items[0].Selected = true;

                DdlContentTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlContentTemplateId.Items[0].Selected = true;
                TbContent.SetParameters(SiteInfo, ChannelAttribute.Content, string.Empty);
            }
            else
            {
                CacAttributes.Attributes = new AttributesImpl(Request.Form);
            }
        }

        public void DdlParentChannelId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var theChannelId = TranslateUtils.ToInt(DdlParentChannelId.SelectedValue);
            if (theChannelId == 0)
            {
                theChannelId = _channelId;
            }
            PageUtils.Redirect(GetRedirectUrl(SiteId, theChannelId, AuthRequest.GetQueryString("ReturnUrl")));
        }

        public string PreviewImageSrc
        {
            get
            {
                if (string.IsNullOrEmpty(TbImageUrl.Text)) return SiteServerAssets.GetIconUrl("empty.gif");

                var extension = PathUtils.GetExtension(TbImageUrl.Text);
                if (EFileSystemTypeUtils.IsImage(extension))
                {
                    return PageUtility.ParseNavigationUrl(SiteInfo, TbImageUrl.Text, true);
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

            int insertChannelId;
            try
            {
                var channelId = AuthRequest.GetQueryInt("ChannelId");
                var nodeInfo = new ChannelInfo
                {
                    SiteId = SiteId,
                    ParentId = channelId,
                    ContentModelPluginId = DdlContentModelPluginId.SelectedValue,
                    ContentRelatedPluginIds =
                        ControlUtils.GetSelectedListControlValueCollection(CblContentRelatedPluginIds)
                };

                if (TbNodeIndexName.Text.Length != 0)
                {
                    var nodeIndexNameArrayList = DataProvider.ChannelDao.GetIndexNameList(SiteId);
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

                    var filePathArrayList = DataProvider.ChannelDao.GetAllFilePathBySiteId(SiteId);
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

                var parentNodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
                var styleInfoList = TableStyleManager.GetChannelStyleInfoList(parentNodeInfo);
                var extendedAttributes = BackgroundInputTypeParser.SaveAttributes(SiteInfo, styleInfoList, Request.Form, null);
                nodeInfo.Additional.Load(extendedAttributes);
                //foreach (string key in attributes)
                //{
                //    nodeInfo.Additional.SetExtendedAttribute(key, attributes[key]);
                //}

                nodeInfo.ChannelName = TbNodeName.Text;
                nodeInfo.IndexName = TbNodeIndexName.Text;
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
                nodeInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);
                nodeInfo.ImageUrl = TbImageUrl.Text;
                nodeInfo.Content = ContentUtility.TextEditorContentEncode(SiteInfo, Request.Form[ChannelAttribute.Content]);
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
                insertChannelId = DataProvider.ChannelDao.Insert(nodeInfo);
                //栏目选择投票样式后，内容
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                FailMessage(ex, $"栏目添加失败：{ex.Message}");
                return;
            }

            CreateManager.CreateChannel(SiteId, insertChannelId);

            AuthRequest.AddSiteLog(SiteId, "添加栏目", $"栏目:{TbNodeName.Text}");

            SuccessMessage("栏目添加成功！");
            AddWaitAndRedirectScript(ReturnUrl);
        }

        public string ReturnUrl { get; private set; }
    }
}

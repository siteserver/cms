using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelEdit : BasePageCms
    {
        public TextBox TbNodeName;
        public TextBox TbNodeIndexName;
        public DropDownList DdlContentModelPluginId;
        public PlaceHolder PhContentRelatedPluginIds;
        public CheckBoxList CblContentRelatedPluginIds;
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
        public TextEditorControl TbContent;
        public TextBox TbKeywords;
        public TextBox TbDescription;
        public ChannelAuxiliaryControl CacAttributes;
        public Button BtnCreateChannelRule;
        public Button BtnCreateContentRule;
        public Button BtnSelectImage;
        public Button BtnUploadImage;
        public Button BtnSubmit;

        private int _channelId;

        public static string GetRedirectUrl(int siteId, int channelId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelEdit), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId", "ReturnUrl");

            _channelId = AuthRequest.GetQueryInt("channelId");
            ReturnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            if (AuthRequest.GetQueryString("CanNotEdit") == null && AuthRequest.GetQueryString("UncheckedChannel") == null)
            {
                if (!HasChannelPermissions(_channelId, ConfigManager.ChannelPermissions.ChannelEdit))
                {
                    PageUtils.RedirectToErrorPage("您没有修改栏目的权限！");
                    return;
                }
            }
            if (AuthRequest.IsQueryExists("CanNotEdit"))
            {
                BtnSubmit.Visible = false;
            }

            var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            if (channelInfo == null) return;
            
            CacAttributes.SiteInfo = SiteInfo;
            CacAttributes.ChannelId = _channelId;

            if (!IsPostBack)
            {
                DdlContentModelPluginId.Items.Add(new ListItem("<默认>", string.Empty));
                var contentTables = PluginContentManager.GetContentModelPlugins();
                foreach (var contentTable in contentTables)
                {
                    DdlContentModelPluginId.Items.Add(new ListItem(contentTable.Title, contentTable.Id));
                }
                ControlUtils.SelectSingleItem(DdlContentModelPluginId, channelInfo.ContentModelPluginId);

                var plugins = PluginContentManager.GetAllContentRelatedPlugins(false);
                if (plugins.Count > 0)
                {
                    var relatedPluginIds =
                        TranslateUtils.StringCollectionToStringList(channelInfo.ContentRelatedPluginIds);
                    foreach (var pluginMetadata in plugins)
                    {
                        CblContentRelatedPluginIds.Items.Add(new ListItem(pluginMetadata.Title, pluginMetadata.Id)
                        {
                            Selected = relatedPluginIds.Contains(pluginMetadata.Id)
                        });
                    }
                }
                else
                {
                    PhContentRelatedPluginIds.Visible = false;
                }

                CacAttributes.Attributes = channelInfo.ToDictionary();

                TbImageUrl.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", SiteInfo.WebUrl));

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

                ControlUtils.AddListControlItems(CblNodeGroupNameCollection, ChannelGroupManager.GetGroupNameList(SiteId));
                //CblNodeGroupNameCollection.DataSource = DataProvider.ChannelGroup.GetDataSource(SiteId);

                DdlChannelTemplateId.DataSource = DataProvider.Template.GetDataSourceByType(SiteId, TemplateType.ChannelTemplate);

                DdlContentTemplateId.DataSource = DataProvider.Template.GetDataSourceByType(SiteId, TemplateType.ContentTemplate);

                DataBind();

                DdlChannelTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                ControlUtils.SelectSingleItem(DdlChannelTemplateId, channelInfo.ChannelTemplateId.ToString());

                DdlContentTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                ControlUtils.SelectSingleItem(DdlContentTemplateId, channelInfo.ContentTemplateId.ToString());

                TbNodeName.Text = channelInfo.ChannelName;
                TbNodeIndexName.Text = channelInfo.IndexName;
                TbLinkUrl.Text = channelInfo.LinkUrl;

                foreach (ListItem item in CblNodeGroupNameCollection.Items)
                {
                    item.Selected = StringUtils.In(channelInfo.GroupNameCollection, item.Value);
                }
                TbFilePath.Text = channelInfo.FilePath;
                TbChannelFilePathRule.Text = channelInfo.ChannelFilePathRule;
                TbContentFilePathRule.Text = channelInfo.ContentFilePathRule;

                ControlUtils.SelectSingleItem(DdlLinkType, channelInfo.LinkType);
                ControlUtils.SelectSingleItem(DdlTaxisType, channelInfo.DefaultTaxisType);
                ControlUtils.SelectSingleItem(RblIsChannelAddable, channelInfo.IsChannelAddable.ToString());
                ControlUtils.SelectSingleItem(RblIsContentAddable, channelInfo.IsContentAddable.ToString());

                TbImageUrl.Text = channelInfo.ImageUrl;

                TbContent.SetParameters(SiteInfo, ChannelAttribute.Content, channelInfo.Content);

                TbKeywords.Text = channelInfo.Keywords;
                TbDescription.Text = channelInfo.Description;

                //this.Content.SiteId = base.SiteId;
                //this.Content.Text = StringUtility.TextEditorContentDecode(nodeInfo.Content, ConfigUtils.Instance.ApplicationPath, base.SiteInfo.SiteUrl);
            }
            else
            {
                CacAttributes.Attributes = TranslateUtils.ToDictionary(Request.Form);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            ChannelInfo channelInfo;
            try
            {
                channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                if (!channelInfo.IndexName.Equals(TbNodeIndexName.Text) && TbNodeIndexName.Text.Length != 0)
                {
                    var nodeIndexNameList = DataProvider.Channel.GetIndexNameList(SiteId);
                    if (nodeIndexNameList.IndexOf(TbNodeIndexName.Text) != -1)
                    {
                        FailMessage("栏目属性修改失败，栏目索引已存在！");
                        return;
                    }
                }

                if (channelInfo.ContentModelPluginId != DdlContentModelPluginId.SelectedValue)
                {
                    channelInfo.ContentModelPluginId = DdlContentModelPluginId.SelectedValue;
                }

                channelInfo.ContentRelatedPluginIds = ControlUtils.GetSelectedListControlValueCollection(CblContentRelatedPluginIds);

                TbFilePath.Text = TbFilePath.Text.Trim();
                if (!channelInfo.FilePath.Equals(TbFilePath.Text) && TbFilePath.Text.Length != 0)
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

                    var filePathArrayList = DataProvider.Channel.GetAllFilePathBySiteId(SiteId);
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

                var styleInfoList = TableStyleManager.GetChannelStyleInfoList(channelInfo);
                var extendedAttributes = BackgroundInputTypeParser.SaveAttributes(SiteInfo, styleInfoList, Request.Form, null);
                foreach (var extendedAttribute in extendedAttributes)
                {
                    channelInfo.Set(extendedAttribute.Key, extendedAttribute.Value);
                }

                channelInfo.ChannelName = TbNodeName.Text;
                channelInfo.IndexName = TbNodeIndexName.Text;
                channelInfo.FilePath = TbFilePath.Text;
                channelInfo.ChannelFilePathRule = TbChannelFilePathRule.Text;
                channelInfo.ContentFilePathRule = TbContentFilePathRule.Text;

                var list = new ArrayList();
                foreach (ListItem item in CblNodeGroupNameCollection.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }
                channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);
                channelInfo.ImageUrl = TbImageUrl.Text;
                channelInfo.Content = ContentUtility.TextEditorContentEncode(SiteInfo, Request.Form[ChannelAttribute.Content]);

                channelInfo.Keywords = TbKeywords.Text;
                channelInfo.Description = TbDescription.Text;

                channelInfo.IsChannelAddable = TranslateUtils.ToBool(RblIsChannelAddable.SelectedValue);
                channelInfo.IsContentAddable = TranslateUtils.ToBool(RblIsContentAddable.SelectedValue);

                channelInfo.LinkUrl = TbLinkUrl.Text;
                channelInfo.LinkType = DdlLinkType.SelectedValue;
                channelInfo.DefaultTaxisType = ETaxisTypeUtils.GetValue(ETaxisTypeUtils.GetEnumType(DdlTaxisType.SelectedValue));
                channelInfo.ChannelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                channelInfo.ContentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;

                DataProvider.Channel.Update(channelInfo);
            }
            catch (Exception ex)
            {
                FailMessage(ex, $"栏目修改失败：{ex.Message}");
                LogUtils.AddErrorLog(ex);
                return;
            }

            CreateManager.CreateChannel(SiteId, channelInfo.Id);

            AuthRequest.AddSiteLog(SiteId, "修改栏目", $"栏目:{TbNodeName.Text}");

            SuccessMessage("栏目修改成功！");
            PageUtils.Redirect(ReturnUrl);
        }

        public string ReturnUrl { get; private set; }
    }
}
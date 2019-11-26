using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
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
                if (!HasChannelPermissions(_channelId, Constants.ChannelPermissions.ChannelEdit))
                {
                    PageUtils.RedirectToErrorPage("您没有修改栏目的权限！");
                    return;
                }
            }
            if (AuthRequest.IsQueryExists("CanNotEdit"))
            {
                BtnSubmit.Visible = false;
            }

            var channelInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
            if (channelInfo == null) return;

            CacAttributes.Site = Site;
            CacAttributes.ChannelId = _channelId;

            if (!IsPostBack)
            {
                DdlContentModelPluginId.Items.Add(new ListItem("<默认>", string.Empty));
                var contentTables = PluginContentManager.GetContentModelPluginsAsync().GetAwaiter().GetResult();
                foreach (var contentTable in contentTables)
                {
                    DdlContentModelPluginId.Items.Add(new ListItem(contentTable.Title, contentTable.Id));
                }
                ControlUtils.SelectSingleItem(DdlContentModelPluginId, channelInfo.ContentModelPluginId);

                var plugins = PluginContentManager.GetAllContentRelatedPluginsAsync(false).GetAwaiter().GetResult();
                if (plugins.Count > 0)
                {
                    var relatedPluginIds = new List<string>(channelInfo.ContentRelatedPluginIdList);
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

                TbImageUrl.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", Site.WebUrl));

                var showPopWinString = ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, true, TbChannelFilePathRule.ClientID);
                BtnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, false, TbContentFilePathRule.ClientID);
                BtnCreateContentRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectImage.GetOpenWindowString(Site, TbImageUrl.ClientID);
                BtnSelectImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImage.GetOpenWindowString(SiteId, TbImageUrl.ClientID);
                BtnUploadImage.Attributes.Add("onclick", showPopWinString);

                ELinkTypeUtils.AddListItems(DdlLinkType);
                ETaxisTypeUtils.AddListItemsForChannelEdit(DdlTaxisType);

                ControlUtils.AddListControlItems(CblNodeGroupNameCollection, ChannelGroupManager.GetGroupNameListAsync(SiteId).GetAwaiter().GetResult());
                //CblNodeGroupNameCollection.DataSource = DataProvider.ChannelGroupDao.GetDataSource(SiteId);

                DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetTemplateListByTypeAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();

                DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetTemplateListByTypeAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();

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
                    item.Selected = StringUtils.ContainsIgnoreCase(channelInfo.GroupNames, item.Value);
                }
                TbFilePath.Text = channelInfo.FilePath;
                TbChannelFilePathRule.Text = channelInfo.ChannelFilePathRule;
                TbContentFilePathRule.Text = channelInfo.ContentFilePathRule;

                ControlUtils.SelectSingleItem(DdlLinkType, channelInfo.LinkType);
                ControlUtils.SelectSingleItem(DdlTaxisType, channelInfo.DefaultTaxisType);
                ControlUtils.SelectSingleItem(RblIsChannelAddable, channelInfo.IsChannelAddable.ToString());
                ControlUtils.SelectSingleItem(RblIsContentAddable, channelInfo.IsContentAddable.ToString());

                TbImageUrl.Text = channelInfo.ImageUrl;

                TbContent.SetParameters(Site, nameof(Channel.Content), channelInfo.Content);

                TbKeywords.Text = channelInfo.Keywords;
                TbDescription.Text = channelInfo.Description;

                //this.Body.SiteId = base.SiteId;
                //this.Body.Text = StringUtility.TextEditorContentDecode(channel.Body, ConfigUtils.Instance.ApplicationPath, base.Site.SiteUrl);
            }
            else
            {
                CacAttributes.Attributes = TranslateUtils.NameValueCollectionToDictionary(Request.Form);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            Channel channel;
            try
            {
                var channelName = TbNodeName.Text;
                var indexName = TbNodeIndexName.Text;
                var filePath = TbFilePath.Text;
                var channelFilePathRule = TbChannelFilePathRule.Text;
                var contentFilePathRule = TbContentFilePathRule.Text;
                var contentModelPluginId = DdlContentModelPluginId.SelectedValue;
                var contentRelatedPluginIds = ControlUtils.GetSelectedListControlValueStringList(CblContentRelatedPluginIds);
                var groupNames = ControlUtils.GetSelectedListControlValueStringList(CblNodeGroupNameCollection);
                var imageUrl = TbImageUrl.Text;
                var content = ContentUtility.TextEditorContentEncodeAsync(Site, Request.Form[nameof(Channel.Content)]).GetAwaiter().GetResult();
                var keywords = TbKeywords.Text;
                var description = TbDescription.Text;
                var isChannelAddable = TranslateUtils.ToBool(RblIsChannelAddable.SelectedValue);
                var isContentAddable = TranslateUtils.ToBool(RblIsContentAddable.SelectedValue);
                var linkUrl = TbLinkUrl.Text;
                var linkType = DdlLinkType.SelectedValue;
                var defaultTaxisType = ETaxisTypeUtils.GetValue(ETaxisTypeUtils.GetEnumType(DdlTaxisType.SelectedValue));
                var channelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                var contentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;

                channel = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
                if (!channel.IndexName.Equals(indexName) && !string.IsNullOrEmpty(indexName))
                {
                    var indexNameList = DataProvider.ChannelDao.GetIndexNameListAsync(SiteId).GetAwaiter().GetResult();
                    if (indexNameList.Contains(indexName))
                    {
                        FailMessage("栏目属性修改失败，栏目索引已存在！");
                        return;
                    }
                }

                if (channel.ContentModelPluginId != contentModelPluginId)
                {
                    channel.ContentModelPluginId = contentModelPluginId;
                }
                channel.ContentRelatedPluginIdList = contentRelatedPluginIds;

                filePath = filePath.Trim();
                if (!channel.FilePath.Equals(filePath) && !string.IsNullOrEmpty(filePath))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(filePath))
                    {
                        FailMessage("栏目页面路径不符合系统要求！");
                        return;
                    }

                    if (PathUtils.IsDirectoryPath(filePath))
                    {
                        filePath = PageUtils.Combine(filePath, "index.html");
                    }

                    var filePathList = DataProvider.ChannelDao.GetAllFilePathBySiteIdAsync(SiteId).GetAwaiter().GetResult();
                    if (filePathList.Contains(filePath))
                    {
                        FailMessage("栏目修改失败，栏目页面路径已存在！");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(channelFilePathRule))
                {
                    var filePathRule = channelFilePathRule.Replace("|", string.Empty);
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

                if (!string.IsNullOrEmpty(contentFilePathRule))
                {
                    var filePathRule = contentFilePathRule.Replace("|", string.Empty);
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

                var styleList = TableStyleManager.GetChannelStyleListAsync(channel).GetAwaiter().GetResult();
                var extendedAttributes = BackgroundInputTypeParser.SaveAttributesAsync(Site, styleList, Request.Form, null).GetAwaiter().GetResult();

                foreach (var extendedAttribute in extendedAttributes)
                {
                    channel.Set(extendedAttribute.Key, extendedAttribute.Value);
                }

                channel.ChannelName = channelName;
                channel.IndexName = indexName;
                channel.FilePath = filePath;
                channel.ChannelFilePathRule = channelFilePathRule;
                channel.ContentFilePathRule = contentFilePathRule;

                channel.GroupNames = groupNames;
                channel.ImageUrl = imageUrl;
                channel.Content = content;

                channel.Keywords = keywords;
                channel.Description = description;

                channel.IsChannelAddable = isChannelAddable;
                channel.IsContentAddable = isContentAddable;

                channel.LinkUrl = linkUrl;
                channel.LinkType = linkType;
                channel.DefaultTaxisType = defaultTaxisType;
                channel.ChannelTemplateId = channelTemplateId;
                channel.ContentTemplateId = contentTemplateId;

                DataProvider.ChannelDao.UpdateAsync(channel).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                FailMessage(ex, $"栏目修改失败：{ex.Message}");
                LogUtils.AddErrorLogAsync(ex).GetAwaiter().GetResult();
                return;
            }

            CreateManager.CreateChannelAsync(SiteId, channel.Id).GetAwaiter().GetResult();

            AuthRequest.AddSiteLogAsync(SiteId, "修改栏目", $"栏目:{TbNodeName.Text}").GetAwaiter().GetResult();

            SuccessMessage("栏目修改成功！");
            PageUtils.Redirect(ReturnUrl);
        }

        public string ReturnUrl { get; private set; }
    }
}
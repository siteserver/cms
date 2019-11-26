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
using SiteServer.CMS.Context.Enumerations;
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

            var parentNodeInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
            if (parentNodeInfo.IsChannelAddable == false)
            {
                PageUtils.RedirectToErrorPage("此栏目不能添加子栏目！");
                return;
            }

            CacAttributes.Site = Site;
            CacAttributes.ChannelId = _channelId;

            if (!IsPostBack)
            {
                ChannelManager.AddListItemsAsync(DdlParentChannelId.Items, Site, true, true, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
                ControlUtils.SelectSingleItem(DdlParentChannelId, _channelId.ToString());

                DdlContentModelPluginId.Items.Add(new ListItem("<默认>", string.Empty));
                var contentTables = PluginContentManager.GetContentModelPluginsAsync().GetAwaiter().GetResult();
                foreach (var contentTable in contentTables)
                {
                    DdlContentModelPluginId.Items.Add(new ListItem(contentTable.Title, contentTable.Id));
                }
                ControlUtils.SelectSingleItem(DdlContentModelPluginId, parentNodeInfo.ContentModelPluginId);

                var plugins = PluginContentManager.GetAllContentRelatedPluginsAsync(false).GetAwaiter().GetResult();
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

                CacAttributes.Attributes = new Dictionary<string, object>();

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
                ControlUtils.SelectSingleItem(DdlTaxisType, ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc));

                ControlUtils.AddListControlItems(CblNodeGroupNameCollection, ChannelGroupManager.GetGroupNameListAsync(SiteId).GetAwaiter().GetResult());
                //CblNodeGroupNameCollection.DataSource = DataProvider.ChannelGroupDao.GetDataSource(SiteId);

                DdlChannelTemplateId.DataSource = DataProvider.TemplateDao
                    .GetTemplateListByTypeAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
                DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetTemplateListByTypeAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();

                DataBind();

                DdlChannelTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlChannelTemplateId.Items[0].Selected = true;

                DdlContentTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
                DdlContentTemplateId.Items[0].Selected = true;
                TbContent.SetParameters(Site, nameof(Channel.Content), string.Empty);
            }
            else
            {
                CacAttributes.Attributes = TranslateUtils.NameValueCollectionToDictionary(Request.Form);
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
                    return PageUtility.ParseNavigationUrl(Site, TbImageUrl.Text, true);
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

                var contentModelPluginId = DdlContentModelPluginId.SelectedValue;
                var contentRelatedPluginIds =
                    ControlUtils.GetSelectedListControlValueStringList(CblContentRelatedPluginIds);
                var channelName = TbNodeName.Text;
                var indexName = TbNodeIndexName.Text;
                var filePath = TbFilePath.Text;
                var channelFilePathRule = TbChannelFilePathRule.Text;
                var contentFilePathRule = TbContentFilePathRule.Text;
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

                var channelInfo = new Channel
                {
                    SiteId = SiteId,
                    ParentId = channelId,
                    ContentModelPluginId = contentModelPluginId,
                    ContentRelatedPluginIdList = contentRelatedPluginIds
                };

                if (!string.IsNullOrEmpty(indexName))
                {
                    var indexNameList = DataProvider.ChannelDao.GetIndexNameListAsync(SiteId).GetAwaiter().GetResult();
                    if (indexNameList.Contains(indexName))
                    {
                        FailMessage("栏目添加失败，栏目索引已存在！");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(filePath))
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
                        FailMessage("栏目添加失败，栏目页面路径已存在！");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(channelFilePathRule))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(channelFilePathRule))
                    {
                        FailMessage("栏目页面命名规则不符合系统要求！");
                        return;
                    }
                    if (PathUtils.IsDirectoryPath(channelFilePathRule))
                    {
                        FailMessage("栏目页面命名规则必须包含生成文件的后缀！");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(contentFilePathRule))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(contentFilePathRule))
                    {
                        FailMessage("内容页面命名规则不符合系统要求！");
                        return;
                    }
                    if (PathUtils.IsDirectoryPath(contentFilePathRule))
                    {
                        FailMessage("内容页面命名规则必须包含生成文件的后缀！");
                        return;
                    }
                }

                var parentChannelInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
                var styleList = TableStyleManager.GetChannelStyleListAsync(parentChannelInfo).GetAwaiter().GetResult();
                var extendedAttributes = BackgroundInputTypeParser.SaveAttributesAsync(Site, styleList, Request.Form, null).GetAwaiter().GetResult();
                foreach (var extendedAttribute in extendedAttributes)
                {
                    channelInfo.Set(extendedAttribute.Key, extendedAttribute.Value);
                }
                //foreach (string key in attributes)
                //{
                //    channel.SetExtendedAttribute(key, attributes[key]);
                //}

                channelInfo.ChannelName = channelName;
                channelInfo.IndexName = indexName;
                channelInfo.FilePath = filePath;
                channelInfo.ChannelFilePathRule = channelFilePathRule;
                channelInfo.ContentFilePathRule = contentFilePathRule;

                channelInfo.GroupNames = groupNames;
                channelInfo.ImageUrl = imageUrl;
                channelInfo.Content = content;
                channelInfo.Keywords = keywords;
                channelInfo.Description = description;
                channelInfo.IsChannelAddable = isChannelAddable;
                channelInfo.IsContentAddable = isContentAddable;
                channelInfo.LinkUrl = linkUrl;
                channelInfo.LinkType = linkType;
                channelInfo.DefaultTaxisType = defaultTaxisType;
                channelInfo.ChannelTemplateId = channelTemplateId;
                channelInfo.ContentTemplateId = contentTemplateId;

                channelInfo.AddDate = DateTime.Now;
                insertChannelId = DataProvider.ChannelDao.InsertAsync(channelInfo).GetAwaiter().GetResult();
                //栏目选择投票样式后，内容
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLogAsync(ex).GetAwaiter().GetResult();
                FailMessage(ex, $"栏目添加失败：{ex.Message}");
                return;
            }

            CreateManager.CreateChannelAsync(SiteId, insertChannelId).GetAwaiter().GetResult();

            AuthRequest.AddSiteLogAsync(SiteId, "添加栏目", $"栏目:{TbNodeName.Text}").GetAwaiter().GetResult();

            SuccessMessage("栏目添加成功！");
            AddWaitAndRedirectScript(ReturnUrl);
        }

        public string ReturnUrl { get; private set; }
    }
}

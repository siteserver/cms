using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalChannelEdit : BasePageCms
    {
        public PlaceHolder PhFilePath;
        public PlaceHolder PhLinkUrl;
        public PlaceHolder PhLinkType;
        public PlaceHolder PhChannelTemplateId;

        public TextBox TbNodeName;
        public TextBox TbNodeIndexName;
        public TextBox TbLinkUrl;
        public CheckBoxList CblNodeGroupNameCollection;
        public DropDownList DdlLinkType;
        public DropDownList DdlTaxisType;
        public DropDownList DdlChannelTemplateId;
        public DropDownList DdlContentTemplateId;
        public TextBox TbImageUrl;
        public Literal LtlImageUrlButtonGroup;
        public TextBox TbFilePath;
        public TextBox TbKeywords;
        public TextBox TbDescription;

        public TextEditorControl TbContent;

        public ChannelAuxiliaryControl CacAttributes;

        public Button BtnSubmit;

        private int _channelId;
        private string _returnUrl;

        public static string GetOpenWindowString(int siteId, int channelId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("快速修改栏目", PageUtils.GetCmsUrl(siteId, nameof(ModalChannelEdit), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }));
        }

        public static string GetRedirectUrl(int siteId, int channelId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalChannelEdit), new NameValueCollection
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
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            CacAttributes.Site = Site;
            CacAttributes.ChannelId = _channelId;

            if (!IsPostBack)
            {
                if (!HasChannelPermissions(_channelId, Constants.ChannelPermissions.ChannelEdit))
                {
                    PageUtils.RedirectToErrorPage("您没有修改栏目的权限！");
                    return;
                }

                var nodeInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
                if (nodeInfo == null) return;

                if (nodeInfo.ParentId == 0)
                {
                    PhLinkUrl.Visible = false;
                    PhLinkType.Visible = false;
                    PhChannelTemplateId.Visible = false;
                    PhFilePath.Visible = false;
                }

                BtnSubmit.Attributes.Add("onclick", $"if (UE && UE.getEditor('Body', {UEditorUtils.ConfigValues})){{ UE.getEditor('Body', {UEditorUtils.ConfigValues}).sync(); }}");

                CacAttributes.Attributes = nodeInfo.ToDictionary();

                if (PhLinkType.Visible)
                {
                    ELinkTypeUtilsExtensions.AddListItems(DdlLinkType);
                }

                ETaxisTypeUtilsExtensions.AddListItemsForChannelEdit(DdlTaxisType);

                ControlUtils.AddListControlItems(CblNodeGroupNameCollection, DataProvider.ChannelGroupRepository.GetGroupNameListAsync(SiteId).GetAwaiter().GetResult());
                //CblNodeGroupNameCollection.DataSource = DataProvider.ChannelGroupRepository.GetDataSource(SiteId);

                if (PhChannelTemplateId.Visible)
                {
                    DdlChannelTemplateId.DataSource = DataProvider.TemplateRepository
                        .GetTemplateListByTypeAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
                }
                DdlContentTemplateId.DataSource = DataProvider.TemplateRepository.GetTemplateListByTypeAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();

                DataBind();

                if (PhChannelTemplateId.Visible)
                {
                    DdlChannelTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                    ControlUtils.SelectSingleItem(DdlChannelTemplateId, nodeInfo.ChannelTemplateId.ToString());
                }

                DdlContentTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                ControlUtils.SelectSingleItem(DdlContentTemplateId, nodeInfo.ContentTemplateId.ToString());

                TbNodeName.Text = nodeInfo.ChannelName;
                TbNodeIndexName.Text = nodeInfo.IndexName;
                if (PhLinkUrl.Visible)
                {
                    TbLinkUrl.Text = nodeInfo.LinkUrl;
                }

                foreach (ListItem item in CblNodeGroupNameCollection.Items)
                {
                    item.Selected = StringUtils.ContainsIgnoreCase(nodeInfo.GroupNames, item.Value);
                }
                if (PhFilePath.Visible)
                {
                    TbFilePath.Text = nodeInfo.FilePath;
                }

                if (PhLinkType.Visible)
                {
                    ControlUtils.SelectSingleItem(DdlLinkType, nodeInfo.LinkType);
                }
                ControlUtils.SelectSingleItem(DdlTaxisType, nodeInfo.DefaultTaxisType);

                TbImageUrl.Text = nodeInfo.ImageUrl;
                LtlImageUrlButtonGroup.Text = WebUtils.GetImageUrlButtonGroupHtml(Site, TbImageUrl.ClientID);
                TbContent.SetParameters(Site, nameof(Channel.Content), nodeInfo.Content);
                if (TbKeywords.Visible)
                {
                    TbKeywords.Text = nodeInfo.Keywords;
                }
                if (TbDescription.Visible)
                {
                    TbDescription.Text = nodeInfo.Description;
                }
            }
            else
            {
                CacAttributes.Attributes = TranslateUtils.NameValueCollectionToDictionary(Request.Form);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isChanged = false;

            try
            {
                var nodeInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();

                if (!nodeInfo.IndexName.Equals(TbNodeIndexName.Text) && !string.IsNullOrEmpty(TbNodeIndexName.Text))
                {
                    var nodeIndexNameList = DataProvider.ChannelRepository.GetIndexNameListAsync(SiteId).GetAwaiter().GetResult();
                    if (nodeIndexNameList.Contains(TbNodeIndexName.Text))
                    {
                        FailMessage("栏目修改失败，栏目索引已存在！");
                        return;
                    }
                }

                if (PhFilePath.Visible)
                {
                    TbFilePath.Text = TbFilePath.Text.Trim();
                    if (!nodeInfo.FilePath.Equals(TbFilePath.Text) && !string.IsNullOrEmpty(TbFilePath.Text))
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

                        var filePathArrayList = DataProvider.ChannelRepository.GetAllFilePathBySiteIdAsync(SiteId).GetAwaiter().GetResult();
                        if (filePathArrayList.Contains(TbFilePath.Text))
                        {
                            FailMessage("栏目修改失败，栏目页面路径已存在！");
                            return;
                        }
                    }
                }

                var styleList = TableStyleManager.GetChannelStyleListAsync(nodeInfo).GetAwaiter().GetResult();

                var extendedAttributes = BackgroundInputTypeParser.SaveAttributesAsync(Site, styleList, Request.Form, null).GetAwaiter().GetResult();
                if (extendedAttributes.Count > 0)
                {
                    foreach (var extendedAttribute in extendedAttributes)
                    {
                        nodeInfo.Set(extendedAttribute.Key, extendedAttribute.Value);
                    }
                }

                nodeInfo.ChannelName = TbNodeName.Text;
                nodeInfo.IndexName = TbNodeIndexName.Text;
                if (PhFilePath.Visible)
                {
                    nodeInfo.FilePath = TbFilePath.Text;
                }

                var list = new List<string>();
                foreach (ListItem item in CblNodeGroupNameCollection.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }

                nodeInfo.GroupNames = list;
                nodeInfo.ImageUrl = TbImageUrl.Text;
                nodeInfo.Content = ContentUtility.TextEditorContentEncodeAsync(Site, Request.Form[nameof(Channel.Content)]).GetAwaiter().GetResult();
                if (TbKeywords.Visible)
                {
                    nodeInfo.Keywords = TbKeywords.Text;
                }
                if (TbDescription.Visible)
                {
                    nodeInfo.Description = TbDescription.Text;
                }

                if (PhLinkUrl.Visible)
                {
                    nodeInfo.LinkUrl = TbLinkUrl.Text;
                }
                if (PhLinkType.Visible)
                {
                    nodeInfo.LinkType = DdlLinkType.SelectedValue;
                }
                nodeInfo.DefaultTaxisType = ETaxisTypeUtils.GetValue(ETaxisTypeUtils.GetEnumType(DdlTaxisType.SelectedValue));
                if (PhChannelTemplateId.Visible)
                {
                    nodeInfo.ChannelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                }
                nodeInfo.ContentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;

                DataProvider.ChannelRepository.UpdateAsync(nodeInfo).GetAwaiter().GetResult();

                AuthRequest.AddSiteLogAsync(SiteId, _channelId, 0, "修改栏目", $"栏目:{nodeInfo.ChannelName}").GetAwaiter().GetResult();

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, $"栏目修改失败：{ex.Message}");
                LogUtils.AddErrorLogAsync(ex).GetAwaiter().GetResult();
            }

            if (isChanged)
            {
                CreateManager.CreateChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(_returnUrl))
                {
                    LayerUtils.Close(Page);
                }
                else
                {
                    LayerUtils.CloseAndRedirect(Page, _returnUrl);
                }
            }
        }
    }
}

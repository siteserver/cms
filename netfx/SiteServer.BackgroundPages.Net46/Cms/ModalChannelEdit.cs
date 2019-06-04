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
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

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
            return LayerUtils.GetOpenScript("快速修改栏目", PageUtilsEx.GetCmsUrl(siteId, nameof(ModalChannelEdit), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }));
        }

        public static string GetRedirectUrl(int siteId, int channelId, string returnUrl)
        {
            return PageUtilsEx.GetCmsUrl(siteId, nameof(ModalChannelEdit), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            FxUtils.CheckRequestParameter("siteId", "channelId", "ReturnUrl");
            _channelId = AuthRequest.GetQueryInt("channelId");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            CacAttributes.SiteInfo = SiteInfo;
            CacAttributes.ChannelId = _channelId;

            if (!IsPostBack)
            {
                if (!HasChannelPermissions(_channelId, ConfigManager.ChannelPermissions.ChannelEdit))
                {
                    FxUtils.Page.RedirectToErrorPage("您没有修改栏目的权限！");
                    return;
                }

                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
                if (nodeInfo == null) return;

                if (nodeInfo.ParentId == 0)
                {
                    PhLinkUrl.Visible = false;
                    PhLinkType.Visible = false;
                    PhChannelTemplateId.Visible = false;
                    PhFilePath.Visible = false;
                }

                BtnSubmit.Attributes.Add("onclick", $"if (UE && UE.getEditor('Content', {UEditorUtils.ConfigValues})){{ UE.getEditor('Content', {UEditorUtils.ConfigValues}).sync(); }}");

                CacAttributes.Attributes = nodeInfo.ToDictionary();

                if (PhLinkType.Visible)
                {
                    ControlUtils.LinkTypeUI.AddListItems(DdlLinkType);
                }

                ControlUtils.TaxisTypeUI.AddListItemsForChannelEdit(DdlTaxisType);

                ControlUtils.AddListControlItems(CblNodeGroupNameCollection, ChannelGroupManager.GetGroupNameList(SiteId));
                //CblNodeGroupNameCollection.DataSource = DataProvider.ChannelGroupDao.GetDataSource(SiteId);

                var templateInfoList = TemplateManager.GetTemplateInfoList(SiteId, TemplateType.ContentTemplate);
                templateInfoList.ForEach(templateInfo =>
                {
                    DdlContentTemplateId.Items.Add(new ListItem(templateInfo.TemplateName, templateInfo.Id.ToString()));
                });

                if (PhChannelTemplateId.Visible)
                {
                    templateInfoList = TemplateManager.GetTemplateInfoList(SiteId, TemplateType.ChannelTemplate);
                    templateInfoList.ForEach(templateInfo =>
                    {
                        DdlChannelTemplateId.Items.Add(new ListItem(templateInfo.TemplateName, templateInfo.Id.ToString()));
                    });
                }

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
                    item.Selected = StringUtils.In(nodeInfo.GroupNameCollection, item.Value);
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
                LtlImageUrlButtonGroup.Text = WebUtils.GetImageUrlButtonGroupHtml(SiteInfo, TbImageUrl.ClientID);
                TbContent.SetParameters(SiteInfo, ChannelAttribute.Content, nodeInfo.Content);
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
                CacAttributes.Attributes = TranslateUtils.ToDictionary(Request.Form);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isChanged = false;

            try
            {
                var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                if (!channelInfo.IndexName.Equals(TbNodeIndexName.Text) && TbNodeIndexName.Text.Length != 0)
                {
                    var nodeIndexNameList = DataProvider.ChannelDao.GetIndexNameList(SiteId);
                    if (nodeIndexNameList.IndexOf(TbNodeIndexName.Text) != -1)
                    {
                        FailMessage("栏目修改失败，栏目索引已存在！");
                        return;
                    }
                }

                if (PhFilePath.Visible)
                {
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

                        var filePathArrayList = DataProvider.ChannelDao.GetAllFilePathBySiteId(SiteId);
                        if (filePathArrayList.IndexOf(TbFilePath.Text) != -1)
                        {
                            FailMessage("栏目修改失败，栏目页面路径已存在！");
                            return;
                        }
                    }
                }

                var styleInfoList = TableStyleManager.GetChannelStyleInfoList(channelInfo);

                var extendedAttributes = BackgroundInputTypeParser.SaveAttributes(SiteInfo, styleInfoList, Request.Form, null);
                if (extendedAttributes.Count > 0)
                {
                    foreach (var extendedAttribute in extendedAttributes)
                    {
                        channelInfo.Set(extendedAttribute.Key, extendedAttribute.Value);
                    }
                }

                channelInfo.ChannelName = TbNodeName.Text;
                channelInfo.IndexName = TbNodeIndexName.Text;
                if (PhFilePath.Visible)
                {
                    channelInfo.FilePath = TbFilePath.Text;
                }

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
                if (TbKeywords.Visible)
                {
                    channelInfo.Keywords = TbKeywords.Text;
                }
                if (TbDescription.Visible)
                {
                    channelInfo.Description = TbDescription.Text;
                }

                if (PhLinkUrl.Visible)
                {
                    channelInfo.LinkUrl = TbLinkUrl.Text;
                }
                if (PhLinkType.Visible)
                {
                    channelInfo.LinkType = DdlLinkType.SelectedValue;
                }
                channelInfo.DefaultTaxisType = ETaxisTypeUtils.GetValue(ETaxisTypeUtils.GetEnumType(DdlTaxisType.SelectedValue));
                if (PhChannelTemplateId.Visible)
                {
                    channelInfo.ChannelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                }
                channelInfo.ContentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;

                DataProvider.ChannelDao.Update(channelInfo);

                AuthRequest.AddChannelLog(SiteId, _channelId, "修改栏目", $"栏目:{channelInfo.ChannelName}");

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, $"栏目修改失败：{ex.Message}");
                LogUtils.AddErrorLog(ex);
            }

            if (isChanged)
            {
                CreateManager.CreateChannel(SiteId, _channelId);

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

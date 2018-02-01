using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalChannelEdit : BasePageCms
    {
        public PlaceHolder PhNodeName;
        public PlaceHolder PhNodeIndexName;
        public PlaceHolder PhFilePath;
        public PlaceHolder PhImageUrl;
        public PlaceHolder PhContent;
        public PlaceHolder PhKeywords;
        public PlaceHolder PhDescription;
        public PlaceHolder PhLinkUrl;
        public PlaceHolder PhLinkType;
        public PlaceHolder PhTaxisType;
        public PlaceHolder PhChannelTemplateId;
        public PlaceHolder PhContentTemplateId;
        public PlaceHolder PhNodeGroupNameCollection;

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
            _channelId = Body.GetQueryInt("channelId");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            CacAttributes.SiteInfo = SiteInfo;
            CacAttributes.ChannelId = _channelId;

            if (!IsPostBack)
            {
                if (!HasChannelPermissions(_channelId, ConfigManager.Permissions.Channel.ChannelEdit))
                {
                    PageUtils.RedirectToErrorPage("您没有修改栏目的权限！");
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

                BtnSubmit.Attributes.Add("onclick", "if (UE && UE.getEditor('Content', {{allowDivTransToP: false}})){ UE.getEditor('Content', {{allowDivTransToP: false}}).sync(); }");

                if (!string.IsNullOrEmpty(SiteInfo.Additional.ChannelEditAttributes))
                {
                    var channelEditAttributes = TranslateUtils.StringCollectionToStringList(SiteInfo.Additional.ChannelEditAttributes);
                    if (channelEditAttributes.Count > 0)
                    {
                        PhNodeName.Visible = PhNodeIndexName.Visible = PhLinkUrl.Visible = PhNodeGroupNameCollection.Visible = PhLinkType.Visible = PhTaxisType.Visible = PhChannelTemplateId.Visible = PhContentTemplateId.Visible = PhImageUrl.Visible = PhFilePath.Visible = PhContent.Visible = PhKeywords.Visible = PhDescription.Visible = false;
                        foreach (var attribute in channelEditAttributes)
                        {
                            if (attribute == ChannelAttribute.ChannelName)
                            {
                                PhNodeName.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.ChannelIndex)
                            {
                                PhNodeIndexName.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.LinkUrl)
                            {
                                PhLinkUrl.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.ChannelGroupNameCollection)
                            {
                                PhNodeGroupNameCollection.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.LinkType)
                            {
                                PhLinkType.Visible = true;
                            }
                            else if (attribute == nameof(ChannelInfoExtend.DefaultTaxisType))
                            {
                                PhTaxisType.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.ChannelTemplateId)
                            {
                                PhChannelTemplateId.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.ContentTemplateId)
                            {
                                PhContentTemplateId.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.ImageUrl)
                            {
                                PhImageUrl.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.FilePath)
                            {
                                PhFilePath.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.Content)
                            {
                                PhContent.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.Keywords)
                            {
                                PhKeywords.Visible = true;
                            }
                            else if (attribute == ChannelAttribute.Description)
                            {
                                PhDescription.Visible = true;
                            }
                        }
                    }
                }

                CacAttributes.Attributes = nodeInfo.Additional;

                if (PhLinkType.Visible)
                {
                    ELinkTypeUtils.AddListItems(DdlLinkType);
                }

                if (PhTaxisType.Visible)
                {
                    ETaxisTypeUtils.AddListItemsForChannelEdit(DdlTaxisType);
                }

                if (PhNodeGroupNameCollection.Visible)
                {
                    CblNodeGroupNameCollection.DataSource = DataProvider.ChannelGroupDao.GetDataSource(SiteId);
                }
                if (PhChannelTemplateId.Visible)
                {
                    DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ChannelTemplate);
                }
                if (PhContentTemplateId.Visible)
                {
                    DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ContentTemplate);
                }

                DataBind();

                if (PhChannelTemplateId.Visible)
                {
                    DdlChannelTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                    ControlUtils.SelectSingleItem(DdlChannelTemplateId, nodeInfo.ChannelTemplateId.ToString());
                }

                if (PhContentTemplateId.Visible)
                {
                    DdlContentTemplateId.Items.Insert(0, new ListItem("<未设置>", "0"));
                    ControlUtils.SelectSingleItem(DdlContentTemplateId, nodeInfo.ContentTemplateId.ToString());
                }

                if (PhNodeName.Visible)
                {
                    TbNodeName.Text = nodeInfo.ChannelName;
                }
                if (PhNodeIndexName.Visible)
                {
                    TbNodeIndexName.Text = nodeInfo.IndexName;
                }
                if (PhLinkUrl.Visible)
                {
                    TbLinkUrl.Text = nodeInfo.LinkUrl;
                }

                if (PhNodeGroupNameCollection.Visible)
                {
                    foreach (ListItem item in CblNodeGroupNameCollection.Items)
                    {
                        item.Selected = CompareUtils.Contains(nodeInfo.GroupNameCollection, item.Value);
                    }
                }
                if (PhFilePath.Visible)
                {
                    TbFilePath.Text = nodeInfo.FilePath;
                }

                if (PhLinkType.Visible)
                {
                    ControlUtils.SelectSingleItem(DdlLinkType, nodeInfo.LinkType);
                }
                if (PhTaxisType.Visible)
                {
                    ControlUtils.SelectSingleItem(DdlTaxisType, nodeInfo.Additional.DefaultTaxisType);
                }

                if (PhImageUrl.Visible)
                {
                    TbImageUrl.Text = nodeInfo.ImageUrl;
                    LtlImageUrlButtonGroup.Text = WebUtils.GetImageUrlButtonGroupHtml(SiteInfo, TbImageUrl.ClientID);
                }
                if (PhContent.Visible)
                {
                    TbContent.SetParameters(SiteInfo, ChannelAttribute.Content, nodeInfo.Content);

                    //this.Content.SiteId = base.SiteId;
                    //this.Content.Text = StringUtility.TextEditorContentDecode(nodeInfo.Content, ConfigUtils.Instance.ApplicationPath, base.SiteInfo.SiteUrl);
                }
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
                CacAttributes.Attributes = new ExtendedAttributes(Request.Form);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isChanged = false;

            try
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                if (PhNodeIndexName.Visible)
                {
                    if (!nodeInfo.IndexName.Equals(TbNodeIndexName.Text) && TbNodeIndexName.Text.Length != 0)
                    {
                        var nodeIndexNameList = DataProvider.ChannelDao.GetIndexNameList(SiteId);
                        if (nodeIndexNameList.IndexOf(TbNodeIndexName.Text) != -1)
                        {
                            FailMessage("栏目修改失败，栏目索引已存在！");
                            return;
                        }
                    }
                }

                if (PhFilePath.Visible)
                {
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

                        var filePathArrayList = DataProvider.ChannelDao.GetAllFilePathBySiteId(SiteId);
                        if (filePathArrayList.IndexOf(TbFilePath.Text) != -1)
                        {
                            FailMessage("栏目修改失败，栏目页面路径已存在！");
                            return;
                        }
                    }
                }

                var extendedAttributes = new ExtendedAttributes();
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _channelId);
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.ChannelDao.TableName,
                    relatedIdentities);
                BackgroundInputTypeParser.SaveAttributes(extendedAttributes, SiteInfo, styleInfoList, Request.Form, null);
                if (extendedAttributes.ToNameValueCollection().Count > 0)
                {
                    nodeInfo.Additional.Load(extendedAttributes.ToNameValueCollection());
                }

                if (PhNodeName.Visible)
                {
                    nodeInfo.ChannelName = TbNodeName.Text;
                }
                if (PhNodeIndexName.Visible)
                {
                    nodeInfo.IndexName = TbNodeIndexName.Text;
                }
                if (PhFilePath.Visible)
                {
                    nodeInfo.FilePath = TbFilePath.Text;
                }

                if (PhNodeGroupNameCollection.Visible)
                {
                    var list = new ArrayList();
                    foreach (ListItem item in CblNodeGroupNameCollection.Items)
                    {
                        if (item.Selected)
                        {
                            list.Add(item.Value);
                        }
                    }
                    nodeInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);
                }
                if (PhImageUrl.Visible)
                {
                    nodeInfo.ImageUrl = TbImageUrl.Text;
                }
                if (PhContent.Visible)
                {
                    nodeInfo.Content = ContentUtility.TextEditorContentEncode(SiteInfo, Request.Form[ChannelAttribute.Content]);
                }
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
                if (PhTaxisType.Visible)
                {
                    nodeInfo.Additional.DefaultTaxisType = ETaxisTypeUtils.GetValue(ETaxisTypeUtils.GetEnumType(DdlTaxisType.SelectedValue));
                }
                if (PhChannelTemplateId.Visible)
                {
                    nodeInfo.ChannelTemplateId = DdlChannelTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue) : 0;
                }
                if (PhContentTemplateId.Visible)
                {
                    nodeInfo.ContentTemplateId = DdlContentTemplateId.Items.Count > 0 ? TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue) : 0;
                }

                DataProvider.ChannelDao.Update(nodeInfo);

                Body.AddSiteLog(SiteId, _channelId, 0, "修改栏目", $"栏目:{nodeInfo.ChannelName}");

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, $"栏目修改失败：{ex.Message}");
                LogUtils.AddSystemErrorLog(ex);
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

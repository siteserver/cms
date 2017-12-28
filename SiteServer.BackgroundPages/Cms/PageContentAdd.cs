using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Table;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Controllers.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public TextBox TbTitle;
        public Literal LtlTitleHtml;
        public AuxiliaryControl AcAttributes;
        public CheckBoxList CblContentAttributes;
        public CheckBoxList CblContentGroups;
        public Button BtnContentGroupAdd;
        public DropDownList DdlContentLevel;
        public TextBox TbTags;
        public Literal LtlTags;
        public PlaceHolder PhTranslate;
        public Button BtnTranslate;
        public DropDownList DdlTranslateType;
        public PlaceHolder PhStatus;
        public TextBox TbLinkUrl;
        public DateTimeTextBox TbAddDate;
        public Button BtnSubmit;

        private NodeInfo _nodeInfo;
        private List<TableStyleInfo> _styleInfoList;
        private string _tableName;
        private Dictionary<string, IContentRelated> _plugins;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrlOfAdd(int publishmentSystemId, int channelId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrlOfEdit(int publishmentSystemId, int channelId, int id, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"id", id.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("publishmentSystemId", "channelId");

            var channelId = Body.GetQueryInt("channelId");
            var contentId = Body.GetQueryInt("id");
            ReturnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("returnUrl"));
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = PageContent.GetRedirectUrl(PublishmentSystemId, channelId);
            }

            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, channelId);
            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, channelId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _plugins = PluginManager.GetContentRelatedFeatures(_nodeInfo);
            ContentInfo contentInfo = null;
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, relatedIdentities);

            if (!IsPermissions(contentId)) return;

            if (contentId > 0)
            {
                contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
            }

            var titleFormat = IsPostBack ? Request.Form[ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)] : contentInfo?.GetString(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title));
            LtlTitleHtml.Text = ContentUtility.GetTitleHtml(titleFormat, AjaxCmsService.GetTitlesUrl(PublishmentSystemId, _nodeInfo.NodeId));

            AcAttributes.PublishmentSystemInfo = PublishmentSystemInfo;
            AcAttributes.NodeId = _nodeInfo.NodeId;
            AcAttributes.StyleInfoList = _styleInfoList;
            AcAttributes.Plugins = _plugins;

            if (!IsPostBack)
            {
                var pageTitle = contentId == 0 ? "添加内容" : "编辑内容";

                LtlPageTitle.Text = pageTitle;
                LtlPageTitle.Text += $@"
<script language=""javascript"" type=""text/javascript"">
var previewUrl = '{PreviewApi.GetContentUrl(PublishmentSystemId, _nodeInfo.NodeId, contentId)}';
</script>
";

                if (HasChannelPermissions(_nodeInfo.NodeId, AppManager.Permissions.Channel.ContentTranslate))
                {
                    PhTranslate.Visible = true;
                    BtnTranslate.Attributes.Add("onclick", ModalChannelMultipleSelect.GetOpenWindowString(PublishmentSystemId, true));

                    ETranslateContentTypeUtils.AddListItems(DdlTranslateType, true);
                    ControlUtils.SelectSingleItem(DdlTranslateType, ETranslateContentTypeUtils.GetValue(ETranslateContentType.Copy));
                }
                else
                {
                    PhTranslate.Visible = false;
                }

                CblContentAttributes.Items.Add(new ListItem("置顶", ContentAttribute.IsTop));
                CblContentAttributes.Items.Add(new ListItem("推荐", ContentAttribute.IsRecommend));
                CblContentAttributes.Items.Add(new ListItem("热点", ContentAttribute.IsHot));
                CblContentAttributes.Items.Add(new ListItem("醒目", ContentAttribute.IsColor));
                TbAddDate.DateTime = DateTime.Now;
                TbAddDate.Now = true;

                var contentGroupNameList = DataProvider.ContentGroupDao.GetContentGroupNameList(PublishmentSystemId);
                foreach (var groupName in contentGroupNameList)
                {
                    var item = new ListItem(groupName, groupName);
                    CblContentGroups.Items.Add(item);
                }
                
                BtnContentGroupAdd.Attributes.Add("onclick", ModalContentGroupAdd.GetOpenWindowString(PublishmentSystemId));

                LtlTags.Text = ContentUtility.GetTagsHtml(AjaxCmsService.GetTagsUrl(PublishmentSystemId));

                if (HasChannelPermissions(_nodeInfo.NodeId, AppManager.Permissions.Channel.ContentCheck))
                {
                    PhStatus.Visible = true;
                    int checkedLevel;
                    var isChecked = CheckManager.GetUserCheckLevel(Body.AdminName, PublishmentSystemInfo, _nodeInfo.NodeId, out checkedLevel);
                    if (Body.IsQueryExists("contentLevel"))
                    {
                        checkedLevel = TranslateUtils.ToIntWithNagetive(Body.GetQueryString("contentLevel"));
                        if (checkedLevel != CheckManager.LevelInt.NotChange)
                        {
                            isChecked = checkedLevel >= PublishmentSystemInfo.CheckContentLevel;
                        }
                    }

                    CheckManager.LoadContentLevelToEdit(DdlContentLevel, PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo, isChecked, checkedLevel);
                }
                else
                {
                    PhStatus.Visible = false;
                }

                BtnSubmit.Attributes.Add("onclick", InputParserUtils.GetValidateSubmitOnClickScript("myForm", true, "autoCheckKeywords()"));
                //自动检测敏感词
                ClientScriptRegisterStartupScript("autoCheckKeywords", WebUtils.GetAutoCheckKeywordsScript(PublishmentSystemInfo));

                if (contentId == 0)
                {
                    var attributes = TableStyleManager.GetDefaultAttributes(_styleInfoList);

                    if (Body.IsQueryExists("isUploadWord"))
                    {
                        var isFirstLineTitle = Body.GetQueryBool("isFirstLineTitle");
                        var isFirstLineRemove = Body.GetQueryBool("isFirstLineRemove");
                        var isClearFormat = Body.GetQueryBool("isClearFormat");
                        var isFirstLineIndent = Body.GetQueryBool("isFirstLineIndent");
                        var isClearFontSize = Body.GetQueryBool("isClearFontSize");
                        var isClearFontFamily = Body.GetQueryBool("isClearFontFamily");
                        var isClearImages = Body.GetQueryBool("isClearImages");
                        var contentLevel = Body.GetQueryInt("contentLevel");
                        var fileName = Body.GetQueryString("fileName");

                        var formCollection = WordUtils.GetWordNameValueCollection(PublishmentSystemId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, contentLevel, fileName);
                        attributes.Load(formCollection);
                    }

                    AcAttributes.Attributes = attributes;
                }
                else if (contentInfo != null)
                {
                    TbTitle.Text = contentInfo.Title;

                    TbTags.Text = contentInfo.Tags;

                    var list = new List<string>();
                    if (contentInfo.IsTop)
                    {
                        list.Add(ContentAttribute.IsTop);
                    }
                    if (contentInfo.IsRecommend)
                    {
                        list.Add(ContentAttribute.IsRecommend);
                    }
                    if (contentInfo.IsHot)
                    {
                        list.Add(ContentAttribute.IsHot);
                    }
                    if (contentInfo.IsColor)
                    {
                        list.Add(ContentAttribute.IsColor);
                    }
                    ControlUtils.SelectMultiItems(CblContentAttributes, list);
                    TbLinkUrl.Text = contentInfo.LinkUrl;
                    TbAddDate.DateTime = contentInfo.AddDate;
                    ControlUtils.SelectMultiItems(CblContentGroups, TranslateUtils.StringCollectionToStringList(contentInfo.ContentGroupNameCollection));

                    AcAttributes.Attributes = contentInfo;
                }
            }
            else
            {
                AcAttributes.Attributes = new ExtendedAttributes(Request.Form);

                if (Body.GetQueryBool("isPreview"))
                {
                    string errorMessage;
                    var previewId = SaveContentInfo(true, out errorMessage);
                    Response.Clear();
                    Response.Write($"{{previewId:{previewId} }}");
                    Response.Flush();
                    Response.End();
                }
            }
            //DataBind();
        }

        private int SaveContentInfo(bool isPreview, out string errorMessage)
        {
            int savedContentId;
            errorMessage = string.Empty;
            var contentId = 0;
            string redirectUrl;
            if (!isPreview)
            {
                contentId = Body.GetQueryInt("id");
            }

            if (contentId == 0)
            {
                var contentInfo = new ContentInfo();
                try
                {
                    contentInfo.NodeId = _nodeInfo.NodeId;
                    contentInfo.PublishmentSystemId = PublishmentSystemId;
                    contentInfo.AddUserName = Body.AdminName;
                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = DateTime.Now;

                    BackgroundInputTypeParser.SaveAttributes(contentInfo, PublishmentSystemInfo, _styleInfoList, Request.Form, ContentAttribute.AllAttributesLowercase);

                    contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items);
                    var tagCollection = TagUtils.ParseTagsString(TbTags.Text);

                    contentInfo.Title = TbTitle.Text;
                    var formatString = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatU"]);
                    var formatColor = Request.Form[ContentAttribute.Title + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);
                    contentInfo.Set(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title), theFormatString);
                    foreach (ListItem listItem in CblContentAttributes.Items)
                    {
                        var value = listItem.Selected.ToString();
                        var attributeName = listItem.Value;
                        contentInfo.Set(attributeName, value);
                    }
                    contentInfo.LinkUrl = TbLinkUrl.Text;
                    contentInfo.AddDate = TbAddDate.DateTime;
                    if (contentInfo.AddDate.Year <= DateUtils.SqlMinValue.Year)
                    {
                        contentInfo.AddDate = DateTime.Now;
                    }

                    contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue);
                    contentInfo.IsChecked = contentInfo.CheckedLevel >= PublishmentSystemInfo.CheckContentLevel;
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    foreach (var pluginId in _plugins.Keys)
                    {
                        var pluginChannel = _plugins[pluginId];
                        pluginChannel.ContentFormSubmited?.Invoke(PublishmentSystemId, _nodeInfo.NodeId, contentInfo, Request.Form);
                    }

                    if (isPreview)
                    {
                        savedContentId = DataProvider.ContentDao.InsertPreview(_tableName, PublishmentSystemInfo, _nodeInfo, contentInfo);
                    }
                    else
                    {
                        savedContentId = DataProvider.ContentDao.Insert(_tableName, PublishmentSystemInfo, contentInfo);
                        //判断是不是有审核权限
                        int checkedLevelOfUser;
                        var isCheckedOfUser = CheckManager.GetUserCheckLevel(Body.AdminName, PublishmentSystemInfo, contentInfo.NodeId, out checkedLevelOfUser);
                        if (CheckManager.IsCheckable(PublishmentSystemInfo, contentInfo.NodeId, contentInfo.IsChecked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                        {
                            //添加审核记录
                            BaiRongDataProvider.ContentDao.UpdateIsChecked(_tableName, PublishmentSystemId, contentInfo.NodeId, new List<int> { savedContentId }, 0, true, Body.AdminName, contentInfo.IsChecked, contentInfo.CheckedLevel, "");
                        }

                        TagUtils.AddTags(tagCollection, PublishmentSystemId, savedContentId);
                    }

                    contentInfo.Id = savedContentId;
                }
                catch (Exception ex)
                {
                    LogUtils.AddSystemErrorLog(ex);
                    errorMessage = $"内容添加失败：{ex.Message}";
                    return 0;
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id);
                }

                Body.AddSiteLog(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id, "添加内容",
                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdminName);

                redirectUrl = PageContentAddAfter.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id,
                    ReturnUrl);
            }
            else
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
                try
                {
                    var tagsLast = contentInfo.Tags;

                    contentInfo.LastEditUserName = Body.AdminName;
                    contentInfo.LastEditDate = DateTime.Now;

                    BackgroundInputTypeParser.SaveAttributes(contentInfo, PublishmentSystemInfo, _styleInfoList, Request.Form, ContentAttribute.AllAttributesLowercase);

                    contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items);
                    var tagCollection = TagUtils.ParseTagsString(TbTags.Text);

                    contentInfo.Title = TbTitle.Text;
                    var formatString = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatU"]);
                    var formatColor = Request.Form[ContentAttribute.Title + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);
                    contentInfo.Set(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title), theFormatString);
                    foreach (ListItem listItem in CblContentAttributes.Items)
                    {
                        var value = listItem.Selected.ToString();
                        var attributeName = listItem.Value;
                        contentInfo.Set(attributeName, value);
                    }
                    contentInfo.LinkUrl = TbLinkUrl.Text;
                    contentInfo.AddDate = TbAddDate.DateTime;

                    var checkedLevel = TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue);
                    if (checkedLevel != CheckManager.LevelInt.NotChange)
                    {
                        contentInfo.IsChecked = checkedLevel >= PublishmentSystemInfo.CheckContentLevel;
                        contentInfo.CheckedLevel = checkedLevel;
                    }
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    foreach (var pluginId in _plugins.Keys)
                    {
                        var pluginChannel = _plugins[pluginId];
                        pluginChannel.ContentFormSubmited?.Invoke(PublishmentSystemId, _nodeInfo.NodeId, contentInfo, Request.Form);
                    }

                    DataProvider.ContentDao.Update(_tableName, PublishmentSystemInfo, contentInfo);

                    TagUtils.UpdateTags(tagsLast, contentInfo.Tags, tagCollection, PublishmentSystemId, contentId);

                    ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdminName);

                    //更新引用该内容的信息
                    //如果不是异步自动保存，那么需要将引用此内容的content修改
                    //var sourceContentIdList = new List<int>
                    //{
                    //    contentInfo.Id
                    //};
                    //var tableList = BaiRongDataProvider.TableCollectionDao.GetTableCollectionInfoListCreatedInDb();
                    //foreach (var table in tableList)
                    //{
                    //    var targetContentIdList = BaiRongDataProvider.ContentDao.GetReferenceIdList(table.TableEnName, sourceContentIdList);
                    //    foreach (var targetContentId in targetContentIdList)
                    //    {
                    //        var targetContentInfo = DataProvider.ContentDao.GetContentInfo(table.TableEnName, targetContentId);
                    //        if (targetContentInfo == null || targetContentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString()) continue;

                    //        contentInfo.Id = targetContentId;
                    //        contentInfo.PublishmentSystemId = targetContentInfo.PublishmentSystemId;
                    //        contentInfo.NodeId = targetContentInfo.NodeId;
                    //        contentInfo.SourceId = targetContentInfo.SourceId;
                    //        contentInfo.ReferenceId = targetContentInfo.ReferenceId;
                    //        contentInfo.Taxis = targetContentInfo.Taxis;
                    //        contentInfo.Set(ContentAttribute.TranslateContentType, targetContentInfo.GetString(ContentAttribute.TranslateContentType));
                    //        BaiRongDataProvider.ContentDao.Update(table.TableEnName, contentInfo);

                    //        //资源：图片，文件，视频
                    //        var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetContentInfo.PublishmentSystemId);
                    //        var bgContentInfo = contentInfo as BackgroundContentInfo;
                    //        var bgTargetContentInfo = targetContentInfo as BackgroundContentInfo;
                    //        if (bgTargetContentInfo != null && bgContentInfo != null)
                    //        {
                    //            if (bgContentInfo.ImageUrl != bgTargetContentInfo.ImageUrl)
                    //            {
                    //                //修改图片
                    //                var sourceImageUrl = PathUtility.MapPath(PublishmentSystemInfo, bgContentInfo.ImageUrl);
                    //                CopyReferenceFiles(targetPublishmentSystemInfo, sourceImageUrl);
                    //            }
                    //            else if (bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)) != bgTargetContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)))
                    //            {
                    //                var sourceImageUrls = TranslateUtils.StringCollectionToStringList(bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)));

                    //                foreach (string imageUrl in sourceImageUrls)
                    //                {
                    //                    var sourceImageUrl = PathUtility.MapPath(PublishmentSystemInfo, imageUrl);
                    //                    CopyReferenceFiles(targetPublishmentSystemInfo, sourceImageUrl);
                    //                }
                    //            }
                    //            if (bgContentInfo.FileUrl != bgTargetContentInfo.FileUrl)
                    //            {
                    //                //修改附件
                    //                var sourceFileUrl = PathUtility.MapPath(PublishmentSystemInfo, bgContentInfo.FileUrl);
                    //                CopyReferenceFiles(targetPublishmentSystemInfo, sourceFileUrl);

                    //            }
                    //            else if (bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)) != bgTargetContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)))
                    //            {
                    //                var sourceFileUrls = TranslateUtils.StringCollectionToStringList(bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)));

                    //                foreach (var fileUrl in sourceFileUrls)
                    //                {
                    //                    var sourceFileUrl = PathUtility.MapPath(PublishmentSystemInfo, fileUrl);
                    //                    CopyReferenceFiles(targetPublishmentSystemInfo, sourceFileUrl);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    LogUtils.AddSystemErrorLog(ex);
                    errorMessage = $"内容修改失败：{ex.Message}";
                    return 0;
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentId);
                }

                Body.AddSiteLog(PublishmentSystemId, _nodeInfo.NodeId, contentId, "修改内容",
                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                redirectUrl = ReturnUrl;
                savedContentId = contentId;
            }

            if (!isPreview)
            {
                PageUtils.Redirect(redirectUrl);
            }

            return savedContentId;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            string errorMessage;
            var savedContentId = SaveContentInfo(false, out errorMessage);
            if (savedContentId == 0)
            {
                FailMessage(errorMessage);
            }
        }

        private bool IsPermissions(int contentId)
        {
            if (contentId == 0)
            {
                if (_nodeInfo == null || _nodeInfo.Additional.IsContentAddable == false)
                {
                    PageUtils.RedirectToErrorPage("此栏目不能添加内容！");
                    return false;
                }

                if (!HasChannelPermissions(_nodeInfo.NodeId, AppManager.Permissions.Channel.ContentAdd))
                {
                    if (!Body.IsAdminLoggin)
                    {
                        PageUtils.RedirectToLoginPage();
                        return false;
                    }

                    PageUtils.RedirectToErrorPage("您无此栏目的添加内容权限！");
                    return false;
                }
            }
            else
            {
                if (!HasChannelPermissions(_nodeInfo.NodeId, AppManager.Permissions.Channel.ContentEdit))
                {
                    if (!Body.IsAdminLoggin)
                    {
                        PageUtils.RedirectToLoginPage();
                        return false;
                    }

                    PageUtils.RedirectToErrorPage("您无此栏目的修改内容权限！");
                    return false;
                }
            }
            return true;
        }

        public string ReturnUrl { get; private set; }
    }
}

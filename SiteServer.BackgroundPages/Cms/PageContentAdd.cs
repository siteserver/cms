using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
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
using SiteServer.Plugin;

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

        private ChannelInfo _nodeInfo;
        private List<TableStyleInfo> _styleInfoList;
        private string _tableName;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrlOfAdd(int siteId, int channelId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentAdd), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrlOfEdit(int siteId, int channelId, int id, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentAdd), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"id", id.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId");

            var channelId = Body.GetQueryInt("channelId");
            var contentId = Body.GetQueryInt("id");
            ReturnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("returnUrl"));
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = PageContent.GetRedirectUrl(SiteId, channelId);
            }

            _nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, channelId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _nodeInfo);
            ContentInfo contentInfo = null;
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, relatedIdentities);

            if (!IsPermissions(contentId)) return;

            if (contentId > 0)
            {
                contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
            }

            var titleFormat = IsPostBack ? Request.Form[ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)] : contentInfo?.GetString(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title));
            LtlTitleHtml.Text = ContentUtility.GetTitleHtml(titleFormat, AjaxCmsService.GetTitlesUrl(SiteId, _nodeInfo.Id));

            AcAttributes.SiteInfo = SiteInfo;
            AcAttributes.ChannelId = _nodeInfo.Id;
            AcAttributes.StyleInfoList = _styleInfoList;

            if (!IsPostBack)
            {
                var pageTitle = contentId == 0 ? "添加内容" : "编辑内容";

                LtlPageTitle.Text = pageTitle;
                LtlPageTitle.Text += $@"
<script language=""javascript"" type=""text/javascript"">
var previewUrl = '{ApiRoutePreview.GetContentUrl(SiteId, _nodeInfo.Id, contentId)}?';
</script>
";

                if (HasChannelPermissions(_nodeInfo.Id, ConfigManager.Permissions.Channel.ContentTranslate))
                {
                    PhTranslate.Visible = true;
                    BtnTranslate.Attributes.Add("onclick", ModalChannelMultipleSelect.GetOpenWindowString(SiteId, true));

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

                var contentGroupNameList = DataProvider.ContentGroupDao.GetGroupNameList(SiteId);
                foreach (var groupName in contentGroupNameList)
                {
                    var item = new ListItem(groupName, groupName);
                    CblContentGroups.Items.Add(item);
                }
                
                BtnContentGroupAdd.Attributes.Add("onclick", ModalContentGroupAdd.GetOpenWindowString(SiteId));

                LtlTags.Text = ContentUtility.GetTagsHtml(AjaxCmsService.GetTagsUrl(SiteId));

                if (HasChannelPermissions(_nodeInfo.Id, ConfigManager.Permissions.Channel.ContentCheck))
                {
                    PhStatus.Visible = true;
                    int checkedLevel;
                    var isChecked = CheckManager.GetUserCheckLevel(Body.AdminName, SiteInfo, _nodeInfo.Id, out checkedLevel);
                    if (Body.IsQueryExists("contentLevel"))
                    {
                        checkedLevel = TranslateUtils.ToIntWithNagetive(Body.GetQueryString("contentLevel"));
                        if (checkedLevel != CheckManager.LevelInt.NotChange)
                        {
                            isChecked = checkedLevel >= SiteInfo.Additional.CheckContentLevel;
                        }
                    }

                    CheckManager.LoadContentLevelToEdit(DdlContentLevel, SiteInfo, _nodeInfo.Id, contentInfo, isChecked, checkedLevel);
                }
                else
                {
                    PhStatus.Visible = false;
                }

                BtnSubmit.Attributes.Add("onclick", InputParserUtils.GetValidateSubmitOnClickScript("myForm", true, "autoCheckKeywords()"));
                //自动检测敏感词
                ClientScriptRegisterStartupScript("autoCheckKeywords", WebUtils.GetAutoCheckKeywordsScript(SiteInfo));

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

                        var formCollection = WordUtils.GetWordNameValueCollection(SiteId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, contentLevel, fileName);
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
                    ControlUtils.SelectMultiItems(CblContentGroups, TranslateUtils.StringCollectionToStringList(contentInfo.GroupNameCollection));

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
                    contentInfo.ChannelId = _nodeInfo.Id;
                    contentInfo.SiteId = SiteId;
                    contentInfo.AddUserName = Body.AdminName;
                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = DateTime.Now;

                    BackgroundInputTypeParser.SaveAttributes(contentInfo, SiteInfo, _styleInfoList, Request.Form, ContentAttribute.AllAttributesLowercase);

                    contentInfo.GroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items);
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
                    contentInfo.IsChecked = contentInfo.CheckedLevel >= SiteInfo.Additional.CheckContentLevel;
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    foreach (var service in PluginManager.Services)
                    {
                        try
                        {
                            service.OnContentFormSubmit(new ContentFormSubmitEventArgs(SiteId, _nodeInfo.Id,
                                contentInfo, new ExtendedAttributes(Request.Form)));
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddPluginErrorLog(service.PluginId, ex, nameof(IService.ContentFormSubmit));
                        }
                    }

                    if (isPreview)
                    {
                        savedContentId = DataProvider.ContentDao.InsertPreview(_tableName, SiteInfo, _nodeInfo, contentInfo);
                    }
                    else
                    {
                        savedContentId = DataProvider.ContentDao.Insert(_tableName, SiteInfo, contentInfo);
                        //判断是不是有审核权限
                        int checkedLevelOfUser;
                        var isCheckedOfUser = CheckManager.GetUserCheckLevel(Body.AdminName, SiteInfo, contentInfo.ChannelId, out checkedLevelOfUser);
                        if (CheckManager.IsCheckable(SiteInfo, contentInfo.ChannelId, contentInfo.IsChecked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                        {
                            //添加审核记录
                            DataProvider.ContentDao.UpdateIsChecked(_tableName, SiteId, contentInfo.ChannelId, new List<int> { savedContentId }, 0, true, Body.AdminName, contentInfo.IsChecked, contentInfo.CheckedLevel, "");
                        }

                        TagUtils.AddTags(tagCollection, SiteId, savedContentId);
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
                    CreateManager.CreateContentAndTrigger(SiteId, _nodeInfo.Id, contentInfo.Id);
                }

                Body.AddSiteLog(SiteId, _nodeInfo.Id, contentInfo.Id, "添加内容",
                    $"栏目:{ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

                ContentUtility.Translate(SiteInfo, _nodeInfo.Id, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdminName);

                redirectUrl = PageContentAddAfter.GetRedirectUrl(SiteId, _nodeInfo.Id, contentInfo.Id,
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

                    BackgroundInputTypeParser.SaveAttributes(contentInfo, SiteInfo, _styleInfoList, Request.Form, ContentAttribute.AllAttributesLowercase);

                    contentInfo.GroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items);
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
                        contentInfo.IsChecked = checkedLevel >= SiteInfo.Additional.CheckContentLevel;
                        contentInfo.CheckedLevel = checkedLevel;
                    }
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    foreach (var service in PluginManager.Services)
                    {
                        try
                        {
                            service.OnContentFormSubmit(new ContentFormSubmitEventArgs(SiteId, _nodeInfo.Id,
                                contentInfo, new ExtendedAttributes(Request.Form)));
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddPluginErrorLog(service.PluginId, ex, nameof(IService.ContentFormSubmit));
                        }
                    }

                    DataProvider.ContentDao.Update(_tableName, SiteInfo, contentInfo);

                    TagUtils.UpdateTags(tagsLast, contentInfo.Tags, tagCollection, SiteId, contentId);

                    ContentUtility.Translate(SiteInfo, _nodeInfo.Id, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdminName);

                    //更新引用该内容的信息
                    //如果不是异步自动保存，那么需要将引用此内容的content修改
                    //var sourceContentIdList = new List<int>
                    //{
                    //    contentInfo.Id
                    //};
                    //var tableList = DataProvider.TableDao.GetTableCollectionInfoListCreatedInDb();
                    //foreach (var table in tableList)
                    //{
                    //    var targetContentIdList = DataProvider.ContentDao.GetReferenceIdList(table.TableName, sourceContentIdList);
                    //    foreach (var targetContentId in targetContentIdList)
                    //    {
                    //        var targetContentInfo = DataProvider.ContentDao.GetContentInfo(table.TableName, targetContentId);
                    //        if (targetContentInfo == null || targetContentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString()) continue;

                    //        contentInfo.Id = targetContentId;
                    //        contentInfo.SiteId = targetContentInfo.SiteId;
                    //        contentInfo.ChannelId = targetContentInfo.ChannelId;
                    //        contentInfo.SourceId = targetContentInfo.SourceId;
                    //        contentInfo.ReferenceId = targetContentInfo.ReferenceId;
                    //        contentInfo.Taxis = targetContentInfo.Taxis;
                    //        contentInfo.Set(ContentAttribute.TranslateContentType, targetContentInfo.GetString(ContentAttribute.TranslateContentType));
                    //        DataProvider.ContentDao.Update(table.TableName, contentInfo);

                    //        //资源：图片，文件，视频
                    //        var targetSiteInfo = SiteManager.GetSiteInfo(targetContentInfo.SiteId);
                    //        var bgContentInfo = contentInfo as BackgroundContentInfo;
                    //        var bgTargetContentInfo = targetContentInfo as BackgroundContentInfo;
                    //        if (bgTargetContentInfo != null && bgContentInfo != null)
                    //        {
                    //            if (bgContentInfo.ImageUrl != bgTargetContentInfo.ImageUrl)
                    //            {
                    //                //修改图片
                    //                var sourceImageUrl = PathUtility.MapPath(SiteInfo, bgContentInfo.ImageUrl);
                    //                CopyReferenceFiles(targetSiteInfo, sourceImageUrl);
                    //            }
                    //            else if (bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)) != bgTargetContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)))
                    //            {
                    //                var sourceImageUrls = TranslateUtils.StringCollectionToStringList(bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)));

                    //                foreach (string imageUrl in sourceImageUrls)
                    //                {
                    //                    var sourceImageUrl = PathUtility.MapPath(SiteInfo, imageUrl);
                    //                    CopyReferenceFiles(targetSiteInfo, sourceImageUrl);
                    //                }
                    //            }
                    //            if (bgContentInfo.FileUrl != bgTargetContentInfo.FileUrl)
                    //            {
                    //                //修改附件
                    //                var sourceFileUrl = PathUtility.MapPath(SiteInfo, bgContentInfo.FileUrl);
                    //                CopyReferenceFiles(targetSiteInfo, sourceFileUrl);

                    //            }
                    //            else if (bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)) != bgTargetContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)))
                    //            {
                    //                var sourceFileUrls = TranslateUtils.StringCollectionToStringList(bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)));

                    //                foreach (var fileUrl in sourceFileUrls)
                    //                {
                    //                    var sourceFileUrl = PathUtility.MapPath(SiteInfo, fileUrl);
                    //                    CopyReferenceFiles(targetSiteInfo, sourceFileUrl);
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
                    CreateManager.CreateContentAndTrigger(SiteId, _nodeInfo.Id, contentId);
                }

                Body.AddSiteLog(SiteId, _nodeInfo.Id, contentId, "修改内容",
                    $"栏目:{ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

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

                if (!HasChannelPermissions(_nodeInfo.Id, ConfigManager.Permissions.Channel.ContentAdd))
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
                if (!HasChannelPermissions(_nodeInfo.Id, ConfigManager.Permissions.Channel.ContentEdit))
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
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

        private ChannelInfo _channelInfo;
        private List<TableStyleInfo> _styleInfoList;
        private string _tableName;

        protected override bool IsSinglePage => true;

        public string PageContentAddHandlerUrl => PageContentAddHandler.GetRedirectUrl(SiteId, AuthRequest.GetQueryInt("channelId"), AuthRequest.GetQueryInt("id"));

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

            var channelId = AuthRequest.GetQueryInt("channelId");
            var contentId = AuthRequest.GetQueryInt("id");
            ReturnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("returnUrl"));
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = CmsPages.GetContentsUrl(SiteId, channelId);
            }

            _channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);
            ContentInfo contentInfo = null;
            _styleInfoList = TableStyleManager.GetContentStyleInfoList(SiteInfo, _channelInfo);

            if (!IsPermissions(contentId)) return;

            if (contentId > 0)
            {
                contentInfo = ContentManager.GetContentInfo(SiteInfo, _channelInfo, contentId);
            }

            var titleFormat = IsPostBack ? Request.Form[ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)] : contentInfo?.GetString(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title));
            LtlTitleHtml.Text = ContentUtility.GetTitleHtml(titleFormat, AjaxCmsService.GetTitlesUrl(SiteId, _channelInfo.Id));

            AcAttributes.SiteInfo = SiteInfo;
            AcAttributes.ChannelId = _channelInfo.Id;
            AcAttributes.ContentId = contentId;
            AcAttributes.StyleInfoList = _styleInfoList;

            if (!IsPostBack)
            {
                var pageTitle = contentId == 0 ? "添加内容" : "编辑内容";

                LtlPageTitle.Text = pageTitle;

                if (HasChannelPermissions(_channelInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate))
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

                var contentGroupNameList = ContentGroupManager.GetGroupNameList(SiteId);
                foreach (var groupName in contentGroupNameList)
                {
                    var item = new ListItem(groupName, groupName);
                    CblContentGroups.Items.Add(item);
                }
                
                BtnContentGroupAdd.Attributes.Add("onclick", ModalContentGroupAdd.GetOpenWindowString(SiteId));

                LtlTags.Text = ContentUtility.GetTagsHtml(AjaxCmsService.GetTagsUrl(SiteId));

                if (HasChannelPermissions(_channelInfo.Id, ConfigManager.ChannelPermissions.ContentCheck))
                {
                    PhStatus.Visible = true;
                    int checkedLevel;
                    var isChecked = CheckManager.GetUserCheckLevel(AuthRequest.AdminPermissionsImpl, SiteInfo, _channelInfo.Id, out checkedLevel);
                    if (AuthRequest.IsQueryExists("contentLevel"))
                    {
                        checkedLevel = TranslateUtils.ToIntWithNagetive(AuthRequest.GetQueryString("contentLevel"));
                        if (checkedLevel != CheckManager.LevelInt.NotChange)
                        {
                            isChecked = checkedLevel >= SiteInfo.Additional.CheckContentLevel;
                        }
                    }

                    CheckManager.LoadContentLevelToEdit(DdlContentLevel, SiteInfo, contentInfo, isChecked, checkedLevel);
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

                    if (AuthRequest.IsQueryExists("isUploadWord"))
                    {
                        var isFirstLineTitle = AuthRequest.GetQueryBool("isFirstLineTitle");
                        var isFirstLineRemove = AuthRequest.GetQueryBool("isFirstLineRemove");
                        var isClearFormat = AuthRequest.GetQueryBool("isClearFormat");
                        var isFirstLineIndent = AuthRequest.GetQueryBool("isFirstLineIndent");
                        var isClearFontSize = AuthRequest.GetQueryBool("isClearFontSize");
                        var isClearFontFamily = AuthRequest.GetQueryBool("isClearFontFamily");
                        var isClearImages = AuthRequest.GetQueryBool("isClearImages");
                        var contentLevel = AuthRequest.GetQueryInt("contentLevel");
                        var fileName = AuthRequest.GetQueryString("fileName");

                        var formCollection = WordUtils.GetWordNameValueCollection(SiteId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, fileName);
                        attributes.Load(formCollection);

                        TbTitle.Text = formCollection[ContentAttribute.Title];
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
                AcAttributes.Attributes = new AttributesImpl(Request.Form);
            }
            //DataBind();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var contentId = AuthRequest.GetQueryInt("id");
            var redirectUrl = string.Empty;

            if (contentId == 0)
            {
                try
                {
                    var tagCollection = TagUtils.ParseTagsString(TbTags.Text);
                    var dict = BackgroundInputTypeParser.SaveAttributes(SiteInfo, _styleInfoList, Request.Form, ContentAttribute.AllAttributes.Value);

                    var contentInfo = new ContentInfo(dict)
                    {
                        ChannelId = _channelInfo.Id,
                        SiteId = SiteId,
                        AddUserName = AuthRequest.AdminName,
                        LastEditDate = DateTime.Now,
                        GroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items),
                        Title = TbTitle.Text
                    };

                    var formatString = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatU"]);
                    var formatColor = Request.Form[ContentAttribute.Title + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);
                    contentInfo.Set(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title), theFormatString);

                    contentInfo.LastEditUserName = contentInfo.AddUserName;

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
                            service.OnContentFormSubmit(new ContentFormSubmitEventArgs(SiteId, _channelInfo.Id,
                                contentInfo.Id, new AttributesImpl(Request.Form), contentInfo));
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(service.PluginId, ex, nameof(IService.ContentFormSubmit));
                        }
                    }

                    
                    //判断是不是有审核权限
                    int checkedLevelOfUser;
                    var isCheckedOfUser = CheckManager.GetUserCheckLevel(AuthRequest.AdminPermissionsImpl, SiteInfo, contentInfo.ChannelId, out checkedLevelOfUser);
                    if (CheckManager.IsCheckable(contentInfo.IsChecked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                    {
                        if (contentInfo.IsChecked)
                        {
                            contentInfo.CheckedLevel = 0;
                        }

                        contentInfo.Set(ContentAttribute.CheckUserName, AuthRequest.AdminName);
                        contentInfo.Set(ContentAttribute.CheckDate, DateUtils.GetDateAndTimeString(DateTime.Now));
                        contentInfo.Set(ContentAttribute.CheckReasons, string.Empty);
                    }

                    contentInfo.Id = DataProvider.ContentDao.Insert(_tableName, SiteInfo, _channelInfo, contentInfo);

                    TagUtils.AddTags(tagCollection, SiteId, contentInfo.Id);

                    CreateManager.CreateContent(SiteId, _channelInfo.Id, contentInfo.Id);
                    CreateManager.TriggerContentChangedEvent(SiteId, _channelInfo.Id);

                    AuthRequest.AddSiteLog(SiteId, _channelInfo.Id, contentInfo.Id, "添加内容",
                        $"栏目:{ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

                    ContentUtility.Translate(SiteInfo, _channelInfo.Id, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), AuthRequest.AdminName);

                    redirectUrl = PageContentAddAfter.GetRedirectUrl(SiteId, _channelInfo.Id, contentInfo.Id,
                        ReturnUrl);
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(ex);
                    FailMessage($"内容添加失败：{ex.Message}");
                }
            }
            else
            {
                var contentInfo = ContentManager.GetContentInfo(SiteInfo, _channelInfo, contentId);
                try
                {
                    var tagsLast = contentInfo.Tags;

                    contentInfo.LastEditUserName = AuthRequest.AdminName;
                    contentInfo.LastEditDate = DateTime.Now;

                    var dict = BackgroundInputTypeParser.SaveAttributes(SiteInfo, _styleInfoList, Request.Form, ContentAttribute.AllAttributes.Value);
                    contentInfo.Load(dict);

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
                            service.OnContentFormSubmit(new ContentFormSubmitEventArgs(SiteId, _channelInfo.Id,
                                contentInfo.Id, new AttributesImpl(Request.Form), contentInfo));
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(service.PluginId, ex, nameof(IService.ContentFormSubmit));
                        }
                    }

                    DataProvider.ContentDao.Update(SiteInfo, _channelInfo, contentInfo);

                    TagUtils.UpdateTags(tagsLast, contentInfo.Tags, tagCollection, SiteId, contentId);

                    ContentUtility.Translate(SiteInfo, _channelInfo.Id, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), AuthRequest.AdminName);

                    CreateManager.CreateContent(SiteId, _channelInfo.Id, contentId);
                    CreateManager.TriggerContentChangedEvent(SiteId, _channelInfo.Id);

                    AuthRequest.AddSiteLog(SiteId, _channelInfo.Id, contentId, "修改内容",
                        $"栏目:{ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

                    redirectUrl = ReturnUrl;

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
                    LogUtils.AddErrorLog(ex);
                    FailMessage($"内容修改失败：{ex.Message}");
                    return;
                }
            }

            PageUtils.Redirect(redirectUrl);
        }

        private bool IsPermissions(int contentId)
        {
            if (contentId == 0)
            {
                if (_channelInfo == null || _channelInfo.Additional.IsContentAddable == false)
                {
                    PageUtils.RedirectToErrorPage("此栏目不能添加内容！");
                    return false;
                }

                if (!HasChannelPermissions(_channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd))
                {
                    if (!AuthRequest.IsAdminLoggin)
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
                if (!HasChannelPermissions(_channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit))
                {
                    if (!AuthRequest.IsAdminLoggin)
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

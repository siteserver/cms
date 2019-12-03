using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;
using TableStyle = SiteServer.Abstractions.TableStyle;

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
        public RadioButtonList RblContentLevel;
        public TextBox TbTags;
        public Literal LtlTags;
        public PlaceHolder PhTranslate;
        public Button BtnTranslate;
        public DropDownList DdlTranslateType;
        public PlaceHolder PhStatus;
        public TextBox TbLinkUrl;
        public DateTimeTextBox TbAddDate;
        public Button BtnSubmit;

        private Channel _channel;
        private List<TableStyle> _styleList;

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

            _channel = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
            Content content = null;
            _styleList = TableStyleManager.GetContentStyleListAsync(Site, _channel).GetAwaiter().GetResult();

            if (!IsPermissions(contentId)) return;

            if (contentId > 0)
            {
                //content = ContentManager.GetContentInfo(Site, _channel, contentId);
                content = DataProvider.ContentRepository.GetAsync(Site, _channel, contentId).GetAwaiter().GetResult();
            }

            var titleFormat = IsPostBack ? Request.Form[ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)] : content?.Get<string>(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title));
            LtlTitleHtml.Text = ContentUtility.GetTitleHtml(titleFormat);

            AcAttributes.Site = Site;
            AcAttributes.ChannelId = _channel.Id;
            AcAttributes.ContentId = contentId;
            AcAttributes.StyleList = _styleList;

            if (!IsPostBack)
            {
                var pageTitle = contentId == 0 ? "添加内容" : "编辑内容";

                LtlPageTitle.Text = pageTitle;

                if (HasChannelPermissions(_channel.Id, Constants.ChannelPermissions.ContentTranslate))
                {
                    PhTranslate.Visible = true;
                    BtnTranslate.Attributes.Add("onclick", ModalChannelMultipleSelect.GetOpenWindowString(SiteId, true));

                    ETranslateContentTypeUtilsExtensions.AddListItems(DdlTranslateType, true);
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

                var contentGroupNameList = DataProvider.ContentGroupRepository.GetGroupNamesAsync(SiteId).GetAwaiter().GetResult();
                foreach (var groupName in contentGroupNameList)
                {
                    var item = new ListItem(groupName, groupName);
                    CblContentGroups.Items.Add(item);
                }
                
                BtnContentGroupAdd.Attributes.Add("onclick", ModalContentGroupAdd.GetOpenWindowString(SiteId));

                LtlTags.Text = ContentUtility.GetTagsHtml(AjaxCmsService.GetTagsUrl(SiteId));

                if (HasChannelPermissions(_channel.Id, Constants.ChannelPermissions.ContentCheck))
                {
                    PhStatus.Visible = true;
                    var (isChecked, checkedLevel) = CheckManager.GetUserCheckLevelAsync(AuthRequest.AdminPermissionsImpl, Site, _channel.Id).GetAwaiter().GetResult();
                    if (AuthRequest.IsQueryExists("contentLevel"))
                    {
                        checkedLevel = TranslateUtils.ToIntWithNagetive(AuthRequest.GetQueryString("contentLevel"));
                        isChecked = checkedLevel >= Site.CheckContentLevel;
                    }

                    CheckManager.LoadContentLevelToEdit(RblContentLevel, Site, content, isChecked, checkedLevel);
                }
                else
                {
                    PhStatus.Visible = false;
                }

                if (contentId == 0)
                {
                    var attributes = TableStyleManager.GetDefaultAttributes(_styleList);

                    if (AuthRequest.IsQueryExists("isUploadWord"))
                    {
                        var isFirstLineTitle = AuthRequest.GetQueryBool("isFirstLineTitle");
                        var isFirstLineRemove = AuthRequest.GetQueryBool("isFirstLineRemove");
                        var isClearFormat = AuthRequest.GetQueryBool("isClearFormat");
                        var isFirstLineIndent = AuthRequest.GetQueryBool("isFirstLineIndent");
                        var isClearFontSize = AuthRequest.GetQueryBool("isClearFontSize");
                        var isClearFontFamily = AuthRequest.GetQueryBool("isClearFontFamily");
                        var isClearImages = AuthRequest.GetQueryBool("isClearImages");
                        var fileName = AuthRequest.GetQueryString("fileName");

                        var formCollection = WordUtils.GetWordNameValueCollectionAsync(SiteId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, fileName).GetAwaiter().GetResult();

                        foreach (var key in formCollection.AllKeys)
                        {
                            attributes[key] = formCollection[key];
                        }

                        TbTitle.Text = formCollection[ContentAttribute.Title];
                    }

                    AcAttributes.Attributes = attributes;

                    //ControlUtils.SelectSingleItem(RblContentLevel, Site.CheckContentDefaultLevel.ToString());
                }
                else if (content != null)
                {
                    TbTitle.Text = content.Title;

                    TbTags.Text = content.Tags;

                    var list = new List<string>();
                    if (content.Top)
                    {
                        list.Add(ContentAttribute.IsTop);
                    }
                    if (content.Recommend)
                    {
                        list.Add(ContentAttribute.IsRecommend);
                    }
                    if (content.Hot)
                    {
                        list.Add(ContentAttribute.IsHot);
                    }
                    if (content.Color)
                    {
                        list.Add(ContentAttribute.IsColor);
                    }
                    ControlUtils.SelectMultiItems(CblContentAttributes, list);
                    TbLinkUrl.Text = content.LinkUrl;
                    if (content.AddDate.HasValue)
                    {
                        TbAddDate.DateTime = content.AddDate.Value;
                    }
                    
                    ControlUtils.SelectMultiItems(CblContentGroups, StringUtils.GetStringList(content.GroupNameCollection));

                    AcAttributes.Attributes = content.ToDictionary();

                    var checkedLevel = content.CheckedLevel;
                    if (content.Checked)
                    {
                        checkedLevel = Site.CheckContentLevel;
                    }
                    ControlUtils.SelectSingleItem(RblContentLevel, checkedLevel.ToString());
                }
            }
            else
            {
                AcAttributes.Attributes = TranslateUtils.NameValueCollectionToDictionary(Request.Form);
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
                    var dict = BackgroundInputTypeParser.SaveAttributesAsync(Site, _styleList, Request.Form, ContentAttribute.AllAttributes.Value).GetAwaiter().GetResult();

                    var contentInfo = new Content(dict)
                    {
                        ChannelId = _channel.Id,
                        SiteId = SiteId,
                        AddUserName = AuthRequest.AdminName,
                        AdminId = AuthRequest.AdminId,
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

                    contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(RblContentLevel.SelectedValue);
                    
                    contentInfo.Checked = contentInfo.CheckedLevel >= Site.CheckContentLevel;
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(ContentTagUtils.ParseTagsString(TbTags.Text), " ");

                    foreach (var service in PluginManager.GetServicesAsync().GetAwaiter().GetResult())
                    {
                        try
                        {
                            service.OnContentFormSubmit(new ContentFormSubmitEventArgs(SiteId, _channel.Id,
                                contentInfo.Id, TranslateUtils.ToDictionary(Request.Form), contentInfo));
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(IService.ContentFormSubmit)).GetAwaiter().GetResult();
                        }
                    }

                    
                    //判断是不是有审核权限
                    var (isCheckedOfUser, checkedLevelOfUser) = CheckManager.GetUserCheckLevelAsync(AuthRequest.AdminPermissionsImpl, Site, contentInfo.ChannelId).GetAwaiter().GetResult();
                    if (CheckManager.IsCheckable(contentInfo.Checked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                    {
                        if (contentInfo.Checked)
                        {
                            contentInfo.CheckedLevel = 0;
                        }

                        contentInfo.CheckUserName = AuthRequest.AdminName;
                        contentInfo.CheckDate = DateTime.Now;
                        contentInfo.CheckReasons = string.Empty;
                    }

                    contentInfo.Id = DataProvider.ContentRepository.InsertAsync(Site, _channel, contentInfo).GetAwaiter().GetResult();

                    ContentTagUtils.UpdateTagsAsync(string.Empty, TbTags.Text, SiteId, contentInfo.Id).GetAwaiter().GetResult();

                    CreateManager.CreateContentAsync(SiteId, _channel.Id, contentInfo.Id).GetAwaiter().GetResult();
                    CreateManager.TriggerContentChangedEventAsync(SiteId, _channel.Id).GetAwaiter().GetResult();

                    AuthRequest.AddSiteLogAsync(SiteId, _channel.Id, contentInfo.Id, "添加内容",
                        $"栏目:{ChannelManager.GetChannelNameNavigationAsync(SiteId, contentInfo.ChannelId).GetAwaiter().GetResult()},内容标题:{contentInfo.Title}").GetAwaiter().GetResult();

                    ContentUtility.TranslateAsync(Site, _channel.Id, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), AuthRequest.AdminName).GetAwaiter().GetResult();

                    redirectUrl = PageContentAddAfter.GetRedirectUrl(SiteId, _channel.Id, contentInfo.Id,
                        ReturnUrl);
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLogAsync(ex).GetAwaiter().GetResult();
                    FailMessage($"内容添加失败：{ex.Message}");
                }
            }
            else
            {
                var contentInfo = DataProvider.ContentRepository.GetAsync(Site, _channel, contentId).GetAwaiter().GetResult();
                try
                {
                    contentInfo.LastEditUserName = AuthRequest.AdminName;
                    contentInfo.LastEditDate = DateTime.Now;

                    var dict = BackgroundInputTypeParser.SaveAttributesAsync(Site, _styleList, Request.Form, ContentAttribute.AllAttributes.Value).GetAwaiter().GetResult();

                    foreach (var o in dict)
                    {
                        contentInfo.Set(o.Key, o.Value);
                    }

                    contentInfo.GroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items);

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

                    var checkedLevel = TranslateUtils.ToIntWithNagetive(RblContentLevel.SelectedValue);
                    contentInfo.Checked = checkedLevel >= Site.CheckContentLevel;
                    contentInfo.CheckedLevel = checkedLevel;

                    ContentTagUtils.UpdateTagsAsync(contentInfo.Tags, TbTags.Text, SiteId, contentId).GetAwaiter().GetResult();
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(ContentTagUtils.ParseTagsString(TbTags.Text), " ");

                    foreach (var service in PluginManager.GetServicesAsync().GetAwaiter().GetResult())
                    {
                        try
                        {
                            service.OnContentFormSubmit(new ContentFormSubmitEventArgs(SiteId, _channel.Id,
                                contentInfo.Id, TranslateUtils.ToDictionary(Request.Form), contentInfo));
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(IService.ContentFormSubmit)).GetAwaiter().GetResult();
                        }
                    }

                    DataProvider.ContentRepository.UpdateAsync(Site, _channel, contentInfo).GetAwaiter().GetResult();

                    ContentUtility.TranslateAsync(Site, _channel.Id, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), AuthRequest.AdminName).GetAwaiter().GetResult();

                    CreateManager.CreateContentAsync(SiteId, _channel.Id, contentId).GetAwaiter().GetResult();
                    CreateManager.TriggerContentChangedEventAsync(SiteId, _channel.Id).GetAwaiter().GetResult();

                    AuthRequest.AddSiteLogAsync(SiteId, _channel.Id, contentId, "修改内容",
                        $"栏目:{ChannelManager.GetChannelNameNavigationAsync(SiteId, contentInfo.ChannelId).GetAwaiter().GetResult()},内容标题:{contentInfo.Title}").GetAwaiter().GetResult();

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
                    //    var targetContentIdList = DataProvider.ContentRepository.GetReferenceIdList(table.TableName, sourceContentIdList);
                    //    foreach (var targetContentId in targetContentIdList)
                    //    {
                    //        var targetContentInfo = DataProvider.ContentRepository.GetContentInfo(table.TableName, targetContentId);
                    //        if (targetContentInfo == null || targetContentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString()) continue;

                    //        contentInfo.Id = targetContentId;
                    //        contentInfo.SiteId = targetContentInfo.SiteId;
                    //        contentInfo.ChannelId = targetContentInfo.ChannelId;
                    //        contentInfo.SourceId = targetContentInfo.SourceId;
                    //        contentInfo.ReferenceId = targetContentInfo.ReferenceId;
                    //        contentInfo.Taxis = targetContentInfo.Taxis;
                    //        contentInfo.Set(ContentAttribute.TranslateContentType, targetContentInfo.GetString(ContentAttribute.TranslateContentType));
                    //        DataProvider.ContentRepository.Update(table.TableName, contentInfo);

                    //        //资源：图片，文件，视频
                    //        var targetSite = DataProvider.SiteRepository.GetSite(targetContentInfo.SiteId);
                    //        var bgContentInfo = contentInfo as BackgroundContentInfo;
                    //        var bgTargetContentInfo = targetContentInfo as BackgroundContentInfo;
                    //        if (bgTargetContentInfo != null && bgContentInfo != null)
                    //        {
                    //            if (bgContentInfo.ImageUrl != bgTargetContentInfo.ImageUrl)
                    //            {
                    //                //修改图片
                    //                var sourceImageUrl = PathUtility.MapPath(Site, bgContentInfo.ImageUrl);
                    //                CopyReferenceFiles(targetSite, sourceImageUrl);
                    //            }
                    //            else if (bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)) != bgTargetContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)))
                    //            {
                    //                var sourceImageUrls = StringUtils.GetStringList(bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)));

                    //                foreach (string imageUrl in sourceImageUrls)
                    //                {
                    //                    var sourceImageUrl = PathUtility.MapPath(Site, imageUrl);
                    //                    CopyReferenceFiles(targetSite, sourceImageUrl);
                    //                }
                    //            }
                    //            if (bgContentInfo.FileUrl != bgTargetContentInfo.FileUrl)
                    //            {
                    //                //修改附件
                    //                var sourceFileUrl = PathUtility.MapPath(Site, bgContentInfo.FileUrl);
                    //                CopyReferenceFiles(targetSite, sourceFileUrl);

                    //            }
                    //            else if (bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)) != bgTargetContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)))
                    //            {
                    //                var sourceFileUrls = StringUtils.GetStringList(bgContentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)));

                    //                foreach (var fileUrl in sourceFileUrls)
                    //                {
                    //                    var sourceFileUrl = PathUtility.MapPath(Site, fileUrl);
                    //                    CopyReferenceFiles(targetSite, sourceFileUrl);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLogAsync(ex).GetAwaiter().GetResult();
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
                if (_channel == null || _channel.IsContentAddable == false)
                {
                    PageUtils.RedirectToErrorPage("此栏目不能添加内容！");
                    return false;
                }

                if (!HasChannelPermissions(_channel.Id, Constants.ChannelPermissions.ContentAdd))
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
                if (!HasChannelPermissions(_channel.Id, Constants.ChannelPermissions.ContentEdit))
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

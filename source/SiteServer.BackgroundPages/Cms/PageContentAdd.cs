using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public TextBox TbTitle;
        public Literal LtlTitleHtml;

        public AuxiliaryControl AcAttributes;

        public CheckBoxList CblContentAttributes;
        public PlaceHolder PhContentGroup;
        public CheckBoxList CblContentGroupNameCollection;
        public RadioButtonList RblContentLevel;
        public PlaceHolder PhTags;
        public TextBox TbTags;
        public Literal LtlTags;
        public PlaceHolder PhTranslate;
        public HtmlControl DivTranslateAdd;
        public DropDownList DdlTranslateType;
        public PlaceHolder PhStatus;
        public DateTimeTextBox TbAddDate;
        public Button BtnSubmit;

        private NodeInfo _nodeInfo;
        private List<int> _relatedIdentities;
        private ETableStyle _tableStyle;
        private string _tableName;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrlOfAdd(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrlOfEdit(int publishmentSystemId, int nodeId, int id, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ID", id.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl");

            var nodeId = Body.GetQueryInt("NodeID");
            var contentId = Body.GetQueryInt("ID");
            ReturnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);
            ContentInfo contentInfo = null;

            if (contentId == 0)
            {
                if (_nodeInfo != null && _nodeInfo.Additional.IsContentAddable == false)
                {
                    PageUtils.RedirectToErrorPage("此栏目不能添加内容！");
                    return;
                }

                if (!HasChannelPermissions(nodeId, AppManager.Permissions.Channel.ContentAdd))
                {
                    if (!Body.IsAdministratorLoggin)
                    {
                        PageUtils.RedirectToLoginPage();
                        return;
                    }
                    else
                    {
                        PageUtils.RedirectToErrorPage("您无此栏目的添加内容权限！");
                        return;
                    }
                }
            }
            else
            {
                contentInfo = DataProvider.ContentDao.GetContentInfo(_tableStyle, _tableName, contentId);
                if (!HasChannelPermissions(nodeId, AppManager.Permissions.Channel.ContentEdit))
                {
                    if (!Body.IsAdministratorLoggin)
                    {
                        PageUtils.RedirectToLoginPage();
                        return;
                    }
                    PageUtils.RedirectToErrorPage("您无此栏目的修改内容权限！");
                    return;
                }
            }

            LtlTitleHtml.Text = GetTitleHtml(contentInfo);

            if (!IsPostBack)
            {
                var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, _nodeInfo.NodeId);
                var pageTitle = contentId == 0 ? "添加内容" : "编辑内容";
                BreadCrumbWithTitle(AppManager.Cms.LeftMenu.IdContent, pageTitle, nodeNames, string.Empty);

                LtlPageTitle.Text = pageTitle;
                LtlPageTitle.Text += $@"
<script language=""javascript"" type=""text/javascript"">
var previewUrl = '{PagePreview.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, contentId, 0, 0)}';
</script>
";

                //转移
                if (AdminUtility.HasChannelPermissions(Body.AdministratorName, PublishmentSystemId, _nodeInfo.NodeId, AppManager.Permissions.Channel.ContentTranslate))
                {
                    PhTranslate.Visible = PublishmentSystemInfo.Additional.IsTranslate;
                    DivTranslateAdd.Attributes.Add("onclick", ModalChannelMultipleSelect.GetOpenWindowString(PublishmentSystemId, true));

                    ETranslateContentTypeUtils.AddListItems(DdlTranslateType, true);
                    ControlUtils.SelectListItems(DdlTranslateType, ETranslateContentTypeUtils.GetValue(ETranslateContentType.Copy));
                }
                else
                {
                    PhTranslate.Visible = false;
                }

                //内容属性
                CblContentAttributes.Items.Add(new ListItem("置顶", ContentAttribute.IsTop));
                CblContentAttributes.Items.Add(new ListItem("推荐", ContentAttribute.IsRecommend));
                CblContentAttributes.Items.Add(new ListItem("热点", ContentAttribute.IsHot));
                CblContentAttributes.Items.Add(new ListItem("醒目", ContentAttribute.IsColor));
                TbAddDate.DateTime = DateTime.Now;
                TbAddDate.Now = true;
                if (contentInfo != null)
                {
                    TbTitle.Text = contentInfo.Title;
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
                    ControlUtils.SelectListItems(CblContentAttributes, list);
                    TbAddDate.DateTime = contentInfo.AddDate;
                }

                //内容组
                var contentGroupNameList = DataProvider.ContentGroupDao.GetContentGroupNameList(PublishmentSystemId);

                if (!PublishmentSystemInfo.Additional.IsGroupContent || contentGroupNameList.Count == 0)
                {
                    PhContentGroup.Visible = false;
                }
                else
                {
                    foreach (var groupName in contentGroupNameList)
                    {
                        var item = new ListItem(groupName, groupName);
                        if (contentId > 0)
                        {
                            item.Selected = StringUtils.In(contentInfo?.ContentGroupNameCollection, groupName);
                        }
                        CblContentGroupNameCollection.Items.Add(item);
                    }
                }

                //标签
                if (!PublishmentSystemInfo.Additional.IsRelatedByTags)
                {
                    PhTags.Visible = false;
                }
                else
                {
                    var tagScript = @"
<script type=""text/javascript"">
function getTags(tag){
	$.get('[url]&tag=' + encodeURIComponent(tag) + '&r=' + Math.random(), function(data) {
		if(data !=''){
			var arr = data.split('|');
			var temp='';
			for(i=0;i<arr.length;i++)
			{
				temp += '<li><a>'+arr[i].replace(tag,'<b>' + tag + '</b>') + '</a></li>';
			}
			var myli='<ul>'+temp+'</ul>';
			$('#tagTips').html(myli);
			$('#tagTips').show();
		}else{
            $('#tagTips').hide();
        }
		$('#tagTips li').click(function () {
			var tag = $('#TbTags').val();
			var i = tag.lastIndexOf(' ');
			if (i > 0)
			{
				tag = tag.substring(0, i) + ' ' + $(this).text();
			}else{
				tag = $(this).text();	
			}
			$('#TbTags').val(tag);
			$('#tagTips').hide();
		})
	});	
}
$(document).ready(function () {
$('#TbTags').keyup(function (e) {
    if (e.keyCode != 40 && e.keyCode != 38) {
        var tag = $('#TbTags').val();
		var i = tag.lastIndexOf(' ');
		if (i > 0){ tag = tag.substring(i + 1);}
        if (tag != '' && tag != ' '){
            window.setTimeout(""getTags('"" + tag + ""');"", 200);
        }else{
            $('#tagTips').hide();
        }
    }
}).blur(function () {
	window.setTimeout(""$('#tagTips').hide();"", 200);
})});
</script>
<div id=""tagTips"" class=""inputTips""></div>
";
                    LtlTags.Text = tagScript.Replace("[url]", AjaxCmsService.GetTagsUrl(PublishmentSystemId));
                }

                if (contentId == 0)
                {
                    var formCollection = new NameValueCollection();
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

                        formCollection = WordUtils.GetWordNameValueCollection(PublishmentSystemId, _nodeInfo.ContentModelId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, contentLevel, fileName);
                    }

                    AcAttributes.SetParameters(formCollection, PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities, _tableStyle, _tableName, false, IsPostBack);
                }
                else
                {
                    AcAttributes.SetParameters(contentInfo?.GetExtendedAttributes(), PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities, _tableStyle, _tableName, true, IsPostBack);
                    TbTags.Text = contentInfo?.Tags;
                }

                if (HasChannelPermissions(nodeId, AppManager.Permissions.Channel.ContentCheck))
                {
                    PhStatus.Visible = true;
                    int checkedLevel;
                    var isChecked = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, _nodeInfo.NodeId, out checkedLevel);
                    if (Body.IsQueryExists("contentLevel"))
                    {
                        checkedLevel = TranslateUtils.ToIntWithNagetive(Body.GetQueryString("contentLevel"));
                        if (checkedLevel != LevelManager.LevelInt.NotChange)
                        {
                            isChecked = checkedLevel >= PublishmentSystemInfo.CheckContentLevel;
                        }
                    }

                    LevelManager.LoadContentLevelToEdit(RblContentLevel, PublishmentSystemInfo, nodeId, contentInfo, isChecked, checkedLevel);
                }
                else
                {
                    PhStatus.Visible = false;
                }

                BtnSubmit.Attributes.Add("onclick", InputParserUtils.GetValidateSubmitOnClickScript("myForm", true, "autoCheckKeywords()"));
                //自动检测敏感词
                ClientScriptRegisterStartupScript("autoCheckKeywords", WebUtils.GetAutoCheckKeywordsScript(PublishmentSystemInfo));
            }
            else
            {
                AcAttributes.SetParameters(Request.Form, PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities,
                    _tableStyle, _tableName, contentId != 0, IsPostBack);
            }
            DataBind();
        }

        private int SaveContentInfo(bool isPreview, out string errorMessage)
        {
            int savedContentId;
            errorMessage = string.Empty;
            var contentId = 0;
            if (!isPreview)
            {
                contentId = Body.GetQueryInt("ID");
            }

            if (contentId == 0)
            {
                var contentInfo = ContentUtility.GetContentInfo(_tableStyle);
                try
                {
                    contentInfo.NodeId = _nodeInfo.NodeId;
                    contentInfo.PublishmentSystemId = PublishmentSystemId;
                    contentInfo.AddUserName = Body.AdministratorName;
                    if (contentInfo.AddDate.Year <= DateUtils.SqlMinValue.Year)
                    {
                        contentInfo.AddDate = DateTime.Now;
                    }
                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = DateTime.Now;

                    //自动保存的时候，不保存编辑器的图片
                    BackgroundInputTypeParser.AddValuesToAttributes(_tableStyle, _tableName, PublishmentSystemInfo, _relatedIdentities, Request.Form, contentInfo.GetExtendedAttributes(), ContentAttribute.HiddenAttributes, true);

                    contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroupNameCollection.Items);
                    var tagCollection = TagUtils.ParseTagsString(TbTags.Text);

                    contentInfo.Title = TbTitle.Text;
                    var formatString = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatU"]);
                    var formatColor = Request.Form[ContentAttribute.Title + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);
                    contentInfo.SetExtendedAttribute(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title), theFormatString);
                    foreach (ListItem listItem in CblContentAttributes.Items)
                    {
                        var value = listItem.Selected.ToString();
                        var attributeName = listItem.Value;
                        contentInfo.SetExtendedAttribute(attributeName, value);
                    }
                    contentInfo.AddDate = TbAddDate.DateTime;

                    contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(RblContentLevel.SelectedValue);
                    contentInfo.IsChecked = contentInfo.CheckedLevel >= PublishmentSystemInfo.CheckContentLevel;
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    if (isPreview)
                    {
                        savedContentId = DataProvider.ContentDao.InsertPreview(_tableName, PublishmentSystemInfo, _nodeInfo, contentInfo);
                    }
                    else
                    {
                        savedContentId = DataProvider.ContentDao.Insert(_tableName, PublishmentSystemInfo, contentInfo);
                        //判断是不是有审核权限
                        int checkedLevelOfUser;
                        var isCheckedOfUser = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, contentInfo.NodeId, out checkedLevelOfUser);
                        if (LevelManager.IsCheckable(PublishmentSystemInfo, contentInfo.NodeId, contentInfo.IsChecked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                        {
                            //添加审核记录
                            BaiRongDataProvider.ContentDao.UpdateIsChecked(_tableName, PublishmentSystemId, contentInfo.NodeId, new List<int> { savedContentId }, 0, true, Body.AdministratorName, contentInfo.IsChecked, contentInfo.CheckedLevel, "");
                        }

                        if (PhTags.Visible)
                        {
                            TagUtils.AddTags(tagCollection, PublishmentSystemId, savedContentId);
                        }
                    }

                    contentInfo.Id = savedContentId;
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(ex);
                    errorMessage = $"内容添加失败：{ex.Message}";
                    return 0;
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id);
                }

                Body.AddSiteLog(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id, "添加内容",
                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdministratorName);

                PageUtils.Redirect(PageContentAddAfter.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id,
                        ReturnUrl));
            }
            else
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableStyle, _tableName, contentId);
                try
                {
                    var tagsLast = contentInfo.Tags;

                    contentInfo.LastEditUserName = Body.AdministratorName;
                    contentInfo.LastEditDate = DateTime.Now;

                    //自动保存的时候，不保存编辑器的图片
                    BackgroundInputTypeParser.AddValuesToAttributes(_tableStyle, _tableName, PublishmentSystemInfo, _relatedIdentities, Request.Form, contentInfo.GetExtendedAttributes(), ContentAttribute.HiddenAttributes, true);

                    contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroupNameCollection.Items);
                    var tagCollection = TagUtils.ParseTagsString(TbTags.Text);

                    contentInfo.Title = TbTitle.Text;
                    var formatString = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(Request.Form[ContentAttribute.Title + "_formatU"]);
                    var formatColor = Request.Form[ContentAttribute.Title + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);
                    contentInfo.SetExtendedAttribute(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title), theFormatString);
                    foreach (ListItem listItem in CblContentAttributes.Items)
                    {
                        var value = listItem.Selected.ToString();
                        var attributeName = listItem.Value;
                        contentInfo.SetExtendedAttribute(attributeName, value);
                    }
                    contentInfo.AddDate = TbAddDate.DateTime;

                    var checkedLevel = TranslateUtils.ToIntWithNagetive(RblContentLevel.SelectedValue);
                    if (checkedLevel != LevelManager.LevelInt.NotChange)
                    {
                        contentInfo.IsChecked = checkedLevel >= PublishmentSystemInfo.CheckContentLevel;
                        contentInfo.CheckedLevel = checkedLevel;
                    }
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    DataProvider.ContentDao.Update(_tableName, PublishmentSystemInfo, contentInfo);

                    if (PhTags.Visible)
                    {
                        TagUtils.UpdateTags(tagsLast, contentInfo.Tags, tagCollection, PublishmentSystemId, contentId);
                    }

                    ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdministratorName);

                    //更新引用该内容的信息
                    //如果不是异步自动保存，那么需要将引用此内容的content修改
                    var sourceContentIdList = new List<int>
                    {
                        contentInfo.Id
                    };
                    var tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.BackgroundContent);
                    foreach (var table in tableList)
                    {
                        var targetContentIdList = BaiRongDataProvider.ContentDao.GetReferenceIdList(table.TableEnName, sourceContentIdList);
                        foreach (var targetContentId in targetContentIdList)
                        {
                            var targetContentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyleUtils.GetEnumType(table.AuxiliaryTableType.ToString()), table.TableEnName, targetContentId);
                            if (targetContentInfo == null || targetContentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString()) continue;

                            contentInfo.Id = targetContentId;
                            contentInfo.PublishmentSystemId = targetContentInfo.PublishmentSystemId;
                            contentInfo.NodeId = targetContentInfo.NodeId;
                            contentInfo.SourceId = targetContentInfo.SourceId;
                            contentInfo.ReferenceId = targetContentInfo.ReferenceId;
                            contentInfo.Taxis = targetContentInfo.Taxis;
                            contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, targetContentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType));
                            BaiRongDataProvider.ContentDao.Update(table.TableEnName, contentInfo);

                            //资源：图片，文件，视频
                            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetContentInfo.PublishmentSystemId);
                            var bgContentInfo = contentInfo as BackgroundContentInfo;
                            var bgTargetContentInfo = targetContentInfo as BackgroundContentInfo;
                            if (bgTargetContentInfo != null && bgContentInfo != null)
                            {
                                if (bgContentInfo.ImageUrl != bgTargetContentInfo.ImageUrl)
                                {
                                    //修改图片
                                    var sourceImageUrl = PathUtility.MapPath(PublishmentSystemInfo, bgContentInfo.ImageUrl);
                                    CopyReferenceFiles(targetPublishmentSystemInfo, sourceImageUrl);
                                }
                                else if (bgContentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)) != bgTargetContentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)))
                                {
                                    var sourceImageUrls = TranslateUtils.StringCollectionToStringList(bgContentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)));

                                    foreach (string imageUrl in sourceImageUrls)
                                    {
                                        var sourceImageUrl = PathUtility.MapPath(PublishmentSystemInfo, imageUrl);
                                        CopyReferenceFiles(targetPublishmentSystemInfo, sourceImageUrl);
                                    }
                                }
                                if (bgContentInfo.FileUrl != bgTargetContentInfo.FileUrl)
                                {
                                    //修改附件
                                    var sourceFileUrl = PathUtility.MapPath(PublishmentSystemInfo, bgContentInfo.FileUrl);
                                    CopyReferenceFiles(targetPublishmentSystemInfo, sourceFileUrl);

                                }
                                else if (bgContentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)) != bgTargetContentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)))
                                {
                                    var sourceFileUrls = TranslateUtils.StringCollectionToStringList(bgContentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)));

                                    foreach (var fileUrl in sourceFileUrls)
                                    {
                                        var sourceFileUrl = PathUtility.MapPath(PublishmentSystemInfo, fileUrl);
                                        CopyReferenceFiles(targetPublishmentSystemInfo, sourceFileUrl);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(ex);
                    errorMessage = $"内容修改失败：{ex.Message}";
                    return 0;
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentId);
                }

                Body.AddSiteLog(PublishmentSystemId, _nodeInfo.NodeId, contentId, "修改内容",
                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                PageUtils.Redirect(ReturnUrl);
                savedContentId = contentId;
            }

            return savedContentId;
        }

        private string GetTitleHtml(ContentInfo contentInfo)
        {
            var builder = new StringBuilder();
            var isFormatted = false;
            var formatStrong = false;
            var formatEm = false;
            var formatU = false;
            var formatColor = string.Empty;
            if (IsPostBack)
            {
                if (Request.Form[ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)] != null)
                {
                    isFormatted = ContentUtility.SetTitleFormatControls(Request.Form[ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)], out formatStrong, out formatEm, out formatU, out formatColor);
                }
            }
            else if (contentInfo != null)
            {
                if (contentInfo.GetExtendedAttribute(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)) != null)
                {
                    isFormatted = ContentUtility.SetTitleFormatControls(contentInfo.GetExtendedAttribute(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)), out formatStrong, out formatEm, out formatU, out formatColor);
                }
            }

            builder.Append(string.Format(@"<a class=""btn"" href=""javascript:;"" onclick=""$('#div_{0}').toggle();return false;""><i class=""icon-text-height""></i></a>
<script type=""text/javascript"">
function {0}_strong(e){{
var e = $(e);
if ($('#{0}_formatStrong').val() == 'true'){{
$('#{0}_formatStrong').val('false');
e.removeClass('btn-success');
}}else{{
$('#{0}_formatStrong').val('true');
e.addClass('btn-success');
}}
}}
function {0}_em(e){{
var e = $(e);
if ($('#{0}_formatEM').val() == 'true'){{
$('#{0}_formatEM').val('false');
e.removeClass('btn-success');
}}else{{
$('#{0}_formatEM').val('true');
e.addClass('btn-success');
}}
}}
function {0}_u(e){{
var e = $(e);
if ($('#{0}_formatU').val() == 'true'){{
$('#{0}_formatU').val('false');
e.removeClass('btn-success');
}}else{{
$('#{0}_formatU').val('true');
e.addClass('btn-success');
}}
}}
function {0}_color(){{
if ($('#{0}_formatColor').val()){{
$('#{0}_colorBtn').css('color', $('#{0}_formatColor').val());
$('#{0}_colorBtn').addClass('btn-success');
}}else{{
$('#{0}_colorBtn').css('color', '');
$('#{0}_colorBtn').removeClass('btn-success');
}}
$('#{0}_colorContainer').hide();
}}
</script>
", ContentAttribute.Title));

            builder.Append(string.Format(@"
<div id=""div_{0}"" style=""display:{1};margin-top:5px;"">
<div class=""btn-group"" style=""float:left;"">
    <button class=""btn{5}"" style=""font-weight:bold;font-size:12px;"" onclick=""{0}_strong(this);return false;"">粗体</button>
    <button class=""btn{6}"" style=""font-style:italic;font-size:12px;"" onclick=""{0}_em(this);return false;"">斜体</button>
    <button class=""btn{7}"" style=""text-decoration:underline;font-size:12px;"" onclick=""{0}_u(this);return false;"">下划线</button>
    <button class=""btn{8}"" style=""font-size:12px;"" id=""{0}_colorBtn"" onclick=""$('#{0}_colorContainer').toggle();return false;"">颜色</button>
</div>
<div id=""{0}_colorContainer"" class=""input-append"" style=""float:left;display:none"">
    <input id=""{0}_formatColor"" name=""{0}_formatColor"" class=""input-mini color {{required:false}}"" type=""text"" value=""{9}"" placeholder=""颜色值"">
    <button class=""btn"" type=""button"" onclick=""Title_color();return false;"">确定</button>
</div>
<input id=""{0}_formatStrong"" name=""{0}_formatStrong"" type=""hidden"" value=""{2}"" />
<input id=""{0}_formatEM"" name=""{0}_formatEM"" type=""hidden"" value=""{3}"" />
<input id=""{0}_formatU"" name=""{0}_formatU"" type=""hidden"" value=""{4}"" />
</div>
", ContentAttribute.Title, isFormatted ? string.Empty : "none", formatStrong.ToString().ToLower(), formatEm.ToString().ToLower(), formatU.ToString().ToLower(), formatStrong ? @" btn-success" : string.Empty, formatEm ? " btn-success" : string.Empty, formatU ? " btn-success" : string.Empty, !string.IsNullOrEmpty(formatColor) ? " btn-success" : string.Empty, formatColor));

            builder.Append(@"
<script type=""text/javascript"">
function getTitles(title){
	$.get('[url]&title=' + encodeURIComponent(title) + '&channelID=' + $('#channelID').val() + '&r=' + Math.random(), function(data) {
		if(data !=''){
			var arr = data.split('|');
			var temp='';
			for(i=0;i<arr.length;i++)
			{
				temp += '<li><a>'+arr[i].replace(title,'<b>' + title + '</b>') + '</a></li>';
			}
			var myli='<ul>'+temp+'</ul>';
			$('#titleTips').html(myli);
			$('#titleTips').show();
		}else{
            $('#titleTips').hide();
        }
		$('#titleTips li').click(function () {
			$('#Title').val($(this).text());
			$('#titleTips').hide();
		})
	});	
}
$(document).ready(function () {
$('#Title').keyup(function (e) {
    if (e.keyCode != 40 && e.keyCode != 38) {
        var title = $('#Title').val();
        if (title != ''){
            window.setTimeout(""getTitles('"" + title + ""');"", 200);
        }else{
            $('#titleTips').hide();
        }
    }
}).blur(function () {
	window.setTimeout(""$('#titleTips').hide();"", 200);
})});
</script>
<div id=""titleTips"" class=""inputTips""></div>");
            builder.Replace("[url]", AjaxCmsService.GetTitlesUrl(PublishmentSystemId, _nodeInfo.NodeId));

            return builder.ToString();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                string errorMessage;
                var savedContentId = SaveContentInfo(false, out errorMessage);
                if (savedContentId == 0)
                {
                    FailMessage(errorMessage);
                }
            }
        }

        private void CopyReferenceFiles(PublishmentSystemInfo targetPublishmentSystemInfo, string sourceUrl)
        {
            var targetUrl = StringUtils.ReplaceFirst(PublishmentSystemInfo.PublishmentSystemDir, sourceUrl, targetPublishmentSystemInfo.PublishmentSystemDir);
            if (!FileUtils.IsFileExists(targetUrl))
            {
                FileUtils.CopyFile(sourceUrl, targetUrl, true);
            }
        }

        public string ReturnUrl { get; private set; }
    }
}

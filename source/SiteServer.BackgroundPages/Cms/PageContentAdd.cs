using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
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

        public AuxiliaryControl AcAttributes;
        public PlaceHolder PhContentAttributes;
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
        public Button BtnSubmit;

        private NodeInfo _nodeInfo;
        private List<int> _relatedIdentities;
        private bool _isAjaxSubmit;
        private bool _isPreview;
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
            _isAjaxSubmit = Body.GetQueryBool("isAjaxSubmit");
            _isPreview = Body.GetQueryBool("isPreview");

            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);
            ContentInfo contentInfo = null;

            if (_isAjaxSubmit == false)
            {
                if (contentId == 0)
                {
                    if (_nodeInfo != null && _nodeInfo.Additional.IsContentAddable == false)
                    {
                        PageUtils.RedirectToErrorPage("此栏目不能添加内容！");
                        return;
                    }

                    if (!HasChannelPermissions(nodeId, AppManager.Cms.Permission.Channel.ContentAdd))
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
                    if (!HasChannelPermissions(nodeId, AppManager.Cms.Permission.Channel.ContentEdit))
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

                if (!IsPostBack)
                {
                    var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, _nodeInfo.NodeId);
                    var pageTitle = (contentId == 0) ?
                        $"添加{ContentModelManager.GetContentModelInfo(PublishmentSystemInfo, _nodeInfo.ContentModelId).ModelName}"
                        : $"编辑{ContentModelManager.GetContentModelInfo(PublishmentSystemInfo, _nodeInfo.ContentModelId).ModelName}";
                    BreadCrumbWithItemTitle(AppManager.Cms.LeftMenu.IdContent, pageTitle, nodeNames, string.Empty);

                    LtlPageTitle.Text = pageTitle;
                    LtlPageTitle.Text += $@"
<script language=""javascript"" type=""text/javascript"">
var previewUrl = '{PagePreview.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, contentId, 0, 0)}';
</script>
";

                    if (PublishmentSystemInfo.Additional.IsAutoSaveContent && PublishmentSystemInfo.Additional.AutoSaveContentInterval > 0)
                    {
                        LtlPageTitle.Text += $@"
<input type=""hidden"" id=""savedContentID"" name=""savedContentID"" value=""{contentId}"">
<script language=""javascript"" type=""text/javascript"">setInterval(""autoSave()"",{PublishmentSystemInfo.Additional.AutoSaveContentInterval*1000});</script>
";
                    }

                    //转移
                    if (AdminUtility.HasChannelPermissions(Body.AdministratorName, PublishmentSystemId, _nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentTranslate))
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
                    var excludeAttributeNames = TableManager.GetExcludeAttributeNames(_tableStyle);
                    AcAttributes.AddExcludeAttributeNames(excludeAttributeNames);

                    if (excludeAttributeNames.Count == 0)
                    {
                        PhContentAttributes.Visible = false;
                    }
                    else
                    {
                        PhContentAttributes.Visible = true;
                        foreach (var attributeName in excludeAttributeNames)
                        {
                            var styleInfo = TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, attributeName, _relatedIdentities);
                            if (styleInfo.IsVisible)
                            {
                                var listItem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                                if (contentId > 0)
                                {
                                    listItem.Selected = TranslateUtils.ToBool(contentInfo?.GetExtendedAttribute(styleInfo.AttributeName));
                                }
                                else
                                {
                                    if (TranslateUtils.ToBool(styleInfo.DefaultValue))
                                    {
                                        listItem.Selected = true;
                                    }
                                }
                                CblContentAttributes.Items.Add(listItem);
                            }
                        }
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
                        AcAttributes.SetParameters(contentInfo?.Attributes, PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities, _tableStyle, _tableName, true, IsPostBack);
                        TbTags.Text = contentInfo?.Tags;
                    }

                    if (HasChannelPermissions(nodeId, AppManager.Cms.Permission.Channel.ContentCheck))
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
            else
            {
                var success = false;
                string errorMessage;
                var savedContentId = SaveContentInfo(true, _isPreview, out errorMessage);
                if (savedContentId > 0)
                {
                    success = true;
                }

                string jsonString = $@"{{success:'{success.ToString().ToLower()}',savedContentID:'{savedContentId}'}}";

                PageUtils.ResponseToJson(jsonString);
            }
        }

        private int SaveContentInfo(bool isAjaxSubmit, bool isPreview, out string errorMessage)
        {
            int savedContentId;
            errorMessage = string.Empty;
            var contentId = 0;
            if (!isPreview)
            {
                contentId = isAjaxSubmit ? TranslateUtils.ToInt(Request.Form["savedContentID"]) : Body.GetQueryInt("ID");
            }

            if (contentId == 0)
            {
                var contentInfo = ContentUtility.GetContentInfo(_tableStyle);
                try
                {
                    contentInfo.NodeId = _nodeInfo.NodeId;
                    contentInfo.PublishmentSystemId = PublishmentSystemId;
                    contentInfo.AddUserName = Body.AdministratorName;
                    if (contentInfo.AddDate.Year == DateUtils.SqlMinValue.Year)
                    {
                        errorMessage = $"内容添加失败：系统时间不能为{DateUtils.SqlMinValue.Year}年";
                        return 0;
                    }
                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = DateTime.Now;

                    //自动保存的时候，不保存编辑器的图片
                    InputTypeParser.AddValuesToAttributes(_tableStyle, _tableName, PublishmentSystemInfo, _relatedIdentities, Request.Form, contentInfo.Attributes, ContentAttribute.HiddenAttributes, !_isAjaxSubmit);

                    StringCollection tagCollection;

                    if (isAjaxSubmit)
                    {
                        contentInfo.ContentGroupNameCollection = Request.Form[ContentAttribute.ContentGroupNameCollection];
                        tagCollection = TagUtils.ParseTagsString(Request.Form[ContentAttribute.Tags]);

                        contentInfo.CheckedLevel = LevelManager.LevelInt.CaoGao;
                        contentInfo.IsChecked = false;
                    }
                    else
                    {
                        contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroupNameCollection.Items);
                        tagCollection = TagUtils.ParseTagsString(TbTags.Text);

                        if (PhContentAttributes.Visible)
                        {
                            foreach (ListItem listItem in CblContentAttributes.Items)
                            {
                                var value = listItem.Selected.ToString();
                                var attributeName = listItem.Value;
                                contentInfo.SetExtendedAttribute(attributeName, value);
                            }
                        }

                        contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(RblContentLevel.SelectedValue);
                        contentInfo.IsChecked = contentInfo.CheckedLevel >= PublishmentSystemInfo.CheckContentLevel;
                    }
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

                if (!isAjaxSubmit)
                {
                    if (contentInfo.IsChecked)
                    {
                        CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id);
                    }

                    Body.AddSiteLog(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id, "添加内容",
                        $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                    ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdministratorName);

                    PageUtils.Redirect(EContentModelTypeUtils.Equals(_nodeInfo.ContentModelId, EContentModelType.Photo)
                        ? PageContentPhotoUpload.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id,
                            Body.GetQueryString("ReturnUrl"))
                        : PageContentAddAfter.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id,
                            ReturnUrl));
                }
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
                    InputTypeParser.AddValuesToAttributes(_tableStyle, _tableName, PublishmentSystemInfo, _relatedIdentities, Request.Form, contentInfo.Attributes, ContentAttribute.HiddenAttributes, !_isAjaxSubmit);

                    StringCollection tagCollection;
                    if (isAjaxSubmit)
                    {
                        contentInfo.ContentGroupNameCollection = Request.Form[ContentAttribute.ContentGroupNameCollection];
                        tagCollection = TagUtils.ParseTagsString(Request.Form[ContentAttribute.Tags]);
                    }
                    else
                    {
                        contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroupNameCollection.Items);
                        tagCollection = TagUtils.ParseTagsString(TbTags.Text);

                        if (PhContentAttributes.Visible)
                        {
                            foreach (ListItem listItem in CblContentAttributes.Items)
                            {
                                var value = listItem.Selected.ToString();
                                var attributeName = listItem.Value;
                                contentInfo.SetExtendedAttribute(attributeName, value);
                            }
                        }

                        var checkedLevel = TranslateUtils.ToIntWithNagetive(RblContentLevel.SelectedValue);
                        if (checkedLevel != LevelManager.LevelInt.NotChange)
                        {
                            contentInfo.IsChecked = checkedLevel >= PublishmentSystemInfo.CheckContentLevel;
                            contentInfo.CheckedLevel = checkedLevel;
                        }
                    }
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    DataProvider.ContentDao.Update(_tableName, PublishmentSystemInfo, contentInfo);

                    if (PhTags.Visible)
                    {
                        TagUtils.UpdateTags(tagsLast, contentInfo.Tags, tagCollection, PublishmentSystemId, contentId);
                    }

                    if (!isAjaxSubmit)
                    {
                        ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(DdlTranslateType.SelectedValue), Body.AdministratorName);

                        if (EContentModelTypeUtils.Equals(_nodeInfo.ContentModelId, EContentModelType.Photo))
                        {
                            PageUtils.Redirect(PageContentPhotoUpload.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id, Body.GetQueryString("ReturnUrl")));
                        }

                        //更新引用该内容的信息
                        //如果不是异步自动保存，那么需要将引用此内容的content修改
                        var sourceContentIdList = new List<int>
                        {
                            contentInfo.Id
                        };
                        var tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.BackgroundContent, EAuxiliaryTableType.JobContent, EAuxiliaryTableType.VoteContent);
                        foreach (var table in tableList)
                        {
                            var targetContentIdList = BaiRongDataProvider.ContentDao.GetReferenceIdList(table.TableEnName, sourceContentIdList);
                            foreach (int targetContentId in targetContentIdList)
                            {
                                var targetContentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyleUtils.GetEnumType(table.AuxiliaryTableType.ToString()), table.TableEnName, targetContentId);
                                if (targetContentInfo != null && targetContentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) == ETranslateContentType.ReferenceContent.ToString())
                                {
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

                                            foreach (string fileUrl in sourceFileUrls)
                                            {
                                                var sourceFileUrl = PathUtility.MapPath(PublishmentSystemInfo, fileUrl);
                                                CopyReferenceFiles(targetPublishmentSystemInfo, sourceFileUrl);
                                            }
                                        }
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

                if (!isAjaxSubmit)
                {
                    if (contentInfo.IsChecked)
                    {
                        CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentId);
                    }

                    Body.AddSiteLog(PublishmentSystemId, _nodeInfo.NodeId, contentId, "修改内容",
                        $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                    PageUtils.Redirect(ReturnUrl);
                }
                savedContentId = contentId;
            }

            return savedContentId;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                string errorMessage;
                var savedContentId = SaveContentInfo(false, false, out errorMessage);
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

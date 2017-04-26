using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicContentAdd : BasePageGovPublic
    {
        public Literal ltlPageTitle;

        public AuxiliarySingleControl ascTitle;
        public TextBox tbIdentifier;
        public TextBox tbPublisher;
        public TextBox tbDocumentNo;
        public DateTimeTextBox dtbPublishDate;
        public TextBox tbKeywords;
        public DateTimeTextBox dtbEffectDate;
        public RadioButtonList rblIsAbolition;
        public DateTimeTextBox dtbAbolitionDate;
        public TextBox tbDescription;
        public HtmlControl divAddChannel;
        public HtmlControl divAddDepartment;
        public Literal ltlCategoryScript;

        public AuxiliaryControl acAttributes;
        public PlaceHolder phContentAttributes;
        public CheckBoxList ContentAttributes;
        public PlaceHolder phContentGroup;
        public CheckBoxList ContentGroupNameCollection;
        public RadioButtonList ContentLevel;
        public PlaceHolder phTags;
        public TextBox Tags;
        public Literal ltlTags;
        public PlaceHolder phTranslate;
        public HtmlControl divTranslateAdd;
        public DropDownList ddlTranslateType;
        public PlaceHolder phStatus;
        public Button Submit;

        private int _contentId;
        private NodeInfo _nodeInfo;
        private string _tableName;
        private ETableStyle _tableStyle;
        private List<int> _relatedIdentities;

        public static string GetRedirectUrlOfAdd(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrlOfEdit(int publishmentSystemId, int nodeId, int id, string returnUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ID", id.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);
            if (nodeId == 0)
            {
                nodeId = PublishmentSystemInfo.Additional.GovPublicNodeId;
            }
            _contentId = TranslateUtils.ToInt(Request.QueryString["ID"]);
            ReturnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);

            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);

            GovPublicContentInfo contentInfo = null;

            if (_contentId == 0)
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
                    PageUtils.RedirectToErrorPage("您无此栏目的添加内容权限！");
                    return;
                }
            }
            else
            {
                contentInfo = DataProvider.GovPublicContentDao.GetContentInfo(PublishmentSystemInfo, _contentId);
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
                var nodeNames = NodeManager.GetNodeNameNavigationByGovPublic(PublishmentSystemId, nodeId);
                var departmentId = 0;
                var departmentName = string.Empty;

                var pageTitle = _contentId == 0 ? "添加信息" : "修改信息";
                BreadCrumbWithItemTitle(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContent, pageTitle, nodeNames, AppManager.Wcm.Permission.WebSite.GovPublicContent);

                ltlPageTitle.Text = pageTitle;
                ltlPageTitle.Text += $@"
<script language=""javascript"" type=""text/javascript"">
var previewUrl = '{PagePreview.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId, _contentId, 0, 0)}';
</script>
";

                EBooleanUtils.AddListItems(rblIsAbolition);
                ControlUtils.SelectListItemsIgnoreCase(rblIsAbolition, false.ToString());

                //转移
                if (_tableStyle == ETableStyle.BackgroundContent && AdminUtility.HasChannelPermissions(Body.AdministratorName, PublishmentSystemId, _nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentTranslate))
                {
                    phTranslate.Visible = PublishmentSystemInfo.Additional.IsTranslate;
                    divTranslateAdd.Attributes.Add("onclick", ModalChannelMultipleSelect.GetOpenWindowString(PublishmentSystemId, true));

                    ETranslateContentTypeUtils.AddListItems(ddlTranslateType, true);
                    ControlUtils.SelectListItems(ddlTranslateType, ETranslateContentTypeUtils.GetValue(ETranslateContentType.Copy));
                }
                else
                {
                    phTranslate.Visible = false;
                }

                //内容属性
                var excludeAttributeNames = TableManager.GetExcludeAttributeNames(_tableStyle);
                acAttributes.AddExcludeAttributeNames(excludeAttributeNames);

                if (excludeAttributeNames.Count == 0)
                {
                    phContentAttributes.Visible = false;
                }
                else
                {
                    phContentAttributes.Visible = true;
                    foreach (var attributeName in GovPublicContentAttribute.CheckBoxAttributes)
                    {
                        var styleInfo = TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, attributeName, _relatedIdentities);
                        if (styleInfo.IsVisible)
                        {
                            var listItem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            if (_contentId > 0)
                            {
                                listItem.Selected = TranslateUtils.ToBool(contentInfo.GetExtendedAttribute(styleInfo.AttributeName));
                            }
                            ContentAttributes.Items.Add(listItem);
                        }
                    }
                }

                //内容组
                var contentGroupNameList = DataProvider.ContentGroupDao.GetContentGroupNameList(PublishmentSystemId);

                if (!PublishmentSystemInfo.Additional.IsGroupContent || contentGroupNameList.Count == 0)
                {
                    phContentGroup.Visible = false;
                }
                else
                {
                    foreach (var groupName in contentGroupNameList)
                    {
                        var item = new ListItem(groupName, groupName);
                        if (_contentId > 0)
                        {
                            item.Selected = StringUtils.In(contentInfo.ContentGroupNameCollection, groupName);
                        }
                        ContentGroupNameCollection.Items.Add(item);
                    }
                }

                if (!PublishmentSystemInfo.Additional.IsRelatedByTags || _tableStyle != ETableStyle.BackgroundContent)
                {
                    phTags.Visible = false;
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
			var tag = $('#Tags').val();
			var i = tag.lastIndexOf(' ');
			if (i > 0)
			{
				tag = tag.substring(0, i) + ' ' + $(this).text();
			}else{
				tag = $(this).text();	
			}
			$('#Tags').val(tag);
			$('#tagTips').hide();
		})
	});	
}
$(document).ready(function () {
$('#Tags').keyup(function (e) {
    if (e.keyCode != 40 && e.keyCode != 38) {
        var tag = $('#Tags').val();
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
                    ltlTags.Text = tagScript.Replace("[url]", AjaxCmsService.GetTagsUrl(PublishmentSystemId));
                }

                divAddChannel.Attributes.Add("onclick", ModalGovPublicCategoryChannelSelect.GetOpenWindowString(PublishmentSystemId));
                divAddDepartment.Attributes.Add("onclick", ModalGovPublicCategoryDepartmentSelect.GetOpenWindowString(PublishmentSystemId));

                var categoryBuilder = new StringBuilder();
                
                var categoryClassInfoArrayList = DataProvider.GovPublicCategoryClassDao.GetCategoryClassInfoArrayList(PublishmentSystemId, ETriState.False, ETriState.True);
                if (categoryClassInfoArrayList.Count > 0)
                {
                    var categoryIndex = 1;
                    foreach (GovPublicCategoryClassInfo categoryClassInfo in categoryClassInfoArrayList)
                    {
                        categoryIndex++;
                        if (categoryIndex % 2 == 0)
                        {
                            categoryBuilder.Append("<tr>");
                        }
                        categoryBuilder.Append(
                            $@"<td height=""30"">{categoryClassInfo.ClassName}分类：</td><td height=""30"">
<div class=""fill_box"" id=""category{categoryClassInfo.ClassCode}Container"" style=""display:none"">
                  <div class=""addr_base addr_normal""> <b id=""category{categoryClassInfo.ClassCode}Name""></b> <a class=""addr_del"" href=""javascript:;"" onClick=""showCategory{categoryClassInfo
                                .ClassCode}('', '0')""></a>
                    <input id=""category{categoryClassInfo.ClassCode}ID"" name=""category{categoryClassInfo.ClassCode}ID"" value=""0"" type=""hidden"">
                  </div>
                </div>
                <div ID=""divAdd{categoryClassInfo.ClassCode}"" class=""btn_pencil"" onclick=""{ModalGovPublicCategorySelect
                                .GetOpenWindowString(PublishmentSystemId, categoryClassInfo.ClassCode)}""><span class=""pencil""></span>　修改</div>
                <script language=""javascript"">
			  function showCategory{categoryClassInfo.ClassCode}({categoryClassInfo.ClassCode}Name, {categoryClassInfo.ClassCode}ID){{
				  $('#category{categoryClassInfo.ClassCode}Name').html({categoryClassInfo.ClassCode}Name);
				  $('#category{categoryClassInfo.ClassCode}ID').val({categoryClassInfo.ClassCode}ID);
				  if ({categoryClassInfo.ClassCode}ID == '0'){{
					$('#category{categoryClassInfo.ClassCode}Container').hide();
				  }}else{{
					  $('#category{categoryClassInfo.ClassCode}Container').show();
				  }}
			  }}
			  </script>
</td>");
                        if (categoryIndex % 2 == 1)
                        {
                            categoryBuilder.Append("</tr>");
                        }
                    }
                }

                if (_contentId == 0)
                {
                    var formCollection = new NameValueCollection();
                    if (Body.IsQueryExists("isUploadWord"))
                    {
                        var isFirstLineTitle = TranslateUtils.ToBool(Request.QueryString["isFirstLineTitle"]);
                        var isFirstLineRemove = TranslateUtils.ToBool(Request.QueryString["isFirstLineRemove"]);
                        var isClearFormat = TranslateUtils.ToBool(Request.QueryString["isClearFormat"]);
                        var isFirstLineIndent = TranslateUtils.ToBool(Request.QueryString["isFirstLineIndent"]);
                        var isClearFontSize = TranslateUtils.ToBool(Request.QueryString["isClearFontSize"]);
                        var isClearFontFamily = TranslateUtils.ToBool(Request.QueryString["isClearFontFamily"]);
                        var isClearImages = TranslateUtils.ToBool(Request.QueryString["isClearImages"]);
                        var contentLevel = TranslateUtils.ToInt(Request.QueryString["contentLevel"]);
                        var fileName = Request.QueryString["fileName"];

                        formCollection = WordUtils.GetWordNameValueCollection(PublishmentSystemId, _nodeInfo.ContentModelId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, contentLevel, fileName);
                    }

                    ascTitle.SetParameters(PublishmentSystemInfo, _nodeInfo.NodeId, _tableStyle, _tableName, ContentAttribute.Title, formCollection, false, IsPostBack);
                    acAttributes.SetParameters(formCollection, PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities, _tableStyle, _tableName, false, IsPostBack);
                }
                //else
                //{
                    
                //    this.Tags.Text = contentInfo.Tags;
                //}

                //if (contentID == 0)
                //{
                //    NameValueCollection formCollection = new NameValueCollection();
                //    this.ascContent.SetParameters(base.PublishmentSystemInfo, base.PublishmentSystemInfo.Additional.GovPublicNodeID, ETableStyle.GovPublicContent, base.PublishmentSystemInfo.AuxiliaryTableForGovPublic, GovPublicContentAttribute.Content, formCollection, false, base.IsPostBack);
                //}
                else
                {
                    departmentId = contentInfo.DepartmentId;
                    departmentName = DepartmentManager.GetDepartmentName(departmentId);

                    foreach (GovPublicCategoryClassInfo categoryClassInfo in categoryClassInfoArrayList)
                    {
                        var categoryId = TranslateUtils.ToInt(contentInfo.GetExtendedAttribute(categoryClassInfo.ContentAttributeName));
                        if (categoryId > 0)
                        {
                            var categoryName = DataProvider.GovPublicCategoryDao.GetCategoryName(categoryId);
                            categoryBuilder.Append(
                                $@"<script>showCategory{categoryClassInfo.ClassCode}('{categoryName}', '{categoryId}');</script>");
                        }
                    }

                    tbIdentifier.Text = contentInfo.Identifier;
                    tbPublisher.Text = contentInfo.Publisher;
                    tbDocumentNo.Text = contentInfo.DocumentNo;
                    dtbPublishDate.DateTime = contentInfo.PublishDate;
                    tbKeywords.Text = contentInfo.Keywords;
                    dtbEffectDate.DateTime = contentInfo.EffectDate;
                    ControlUtils.SelectListItemsIgnoreCase(rblIsAbolition, contentInfo.IsAbolition.ToString());
                    dtbAbolitionDate.DateTime = contentInfo.AbolitionDate;
                    tbDescription.Text = contentInfo.Description;

                    ascTitle.SetParameters(PublishmentSystemInfo, _nodeInfo.NodeId, _tableStyle, _tableName, ContentAttribute.Title, contentInfo.Attributes, true, IsPostBack);

                    acAttributes.SetParameters(contentInfo.Attributes, PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities, _tableStyle, _tableName, true, IsPostBack);
                    Tags.Text = contentInfo.Tags;
                }

                if (departmentId == 0)
                {
                    departmentId = BaiRongDataProvider.AdministratorDao.GetDepartmentId(Body.AdministratorName);
                    if (departmentId > 0)
                    {
                        departmentName = DepartmentManager.GetDepartmentName(departmentId);
                    }
                }
                
                categoryBuilder.Append(
                    $@"<script>showCategoryChannel('{nodeNames}', '{nodeId}');showCategoryDepartment('{departmentName}', '{departmentId}');</script>");

                ltlCategoryScript.Text = categoryBuilder.ToString();

                if (HasChannelPermissions(nodeId, AppManager.Cms.Permission.Channel.ContentCheck))
                {
                    phStatus.Visible = true;

                    int checkedLevel;
                    var isChecked = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, PublishmentSystemId, out checkedLevel);
                    LevelManager.LoadContentLevelToEdit(ContentLevel, PublishmentSystemInfo, nodeId, contentInfo, isChecked, checkedLevel);
                }
                else
                {
                    phStatus.Visible = false;
                }

                Submit.Attributes.Add("onclick", InputParserUtils.GetValidateSubmitOnClickScript("myForm"));

                if (PublishmentSystemInfo.Additional.GovPublicIsPublisherRelatedDepartmentId && string.IsNullOrEmpty(tbPublisher.Text))
                {
                    tbPublisher.Text = departmentName;
                }
            }
            else
            {
                if (_contentId == 0)
                {
                    ascTitle.SetParameters(PublishmentSystemInfo, _nodeInfo.NodeId, _tableStyle, _tableName, ContentAttribute.Title, Request.Form, false, IsPostBack);

                    acAttributes.SetParameters(Request.Form, PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities, _tableStyle, _tableName, false, IsPostBack);
                }
                else
                {
                    ascTitle.SetParameters(PublishmentSystemInfo, _nodeInfo.NodeId, _tableStyle, _tableName, ContentAttribute.Title, Request.Form, true, IsPostBack);

                    acAttributes.SetParameters(Request.Form, PublishmentSystemInfo, _nodeInfo.NodeId, _relatedIdentities, _tableStyle, _tableName, true, IsPostBack);
                }
                //this.ascContent.SetParameters(base.PublishmentSystemInfo, base.PublishmentSystemInfo.Additional.GovPublicNodeID, ETableStyle.GovPublicContent, base.PublishmentSystemInfo.AuxiliaryTableForGovPublic, GovPublicContentAttribute.Content, base.Request.Form, true, base.IsPostBack);
            }
            DataBind();
        }

        public bool IsPublisherRelatedDepartmentId => PublishmentSystemInfo.Additional.GovPublicIsPublisherRelatedDepartmentId;

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var categoryChannelId = TranslateUtils.ToInt(Request["categoryChannelID"]);
            var categoryDepartmentId = TranslateUtils.ToInt(Request["categoryDepartmentID"]);
            if (categoryChannelId == 0)
            {
                FailMessage("信息采集失败，必须要选择一个主题分类");
                return;
            }

            if (categoryDepartmentId == 0)
            {
                FailMessage("信息采集失败，必须要选择一个机构分类");
                return;
            }
                
            var categoryClassInfoArrayList = DataProvider.GovPublicCategoryClassDao.GetCategoryClassInfoArrayList(PublishmentSystemId, ETriState.False, ETriState.True);

            if (_contentId == 0)
            {
                var contentInfo = new GovPublicContentInfo();
                try
                {
                    InputTypeParser.AddValuesToAttributes(_tableStyle, _tableName, PublishmentSystemInfo, _relatedIdentities, Request.Form, contentInfo.Attributes, ContentAttribute.HiddenAttributes);

                    contentInfo.NodeId = categoryChannelId;
                    contentInfo.Description = tbDescription.Text;
                    contentInfo.PublishDate = dtbPublishDate.DateTime;
                    contentInfo.EffectDate = dtbEffectDate.DateTime;
                    contentInfo.IsAbolition = TranslateUtils.ToBool(rblIsAbolition.SelectedValue);
                    contentInfo.AbolitionDate = dtbAbolitionDate.DateTime;
                    contentInfo.DocumentNo = tbDocumentNo.Text;
                    contentInfo.Publisher = tbPublisher.Text;
                    contentInfo.Keywords = tbKeywords.Text;

                    contentInfo.DepartmentId = categoryDepartmentId;
                    SetCategoryAttributes(contentInfo, categoryClassInfoArrayList);
                    contentInfo.PublishmentSystemId = PublishmentSystemId;
                    contentInfo.AddUserName = Body.AdministratorName;
                    if (contentInfo.AddDate.Year == DateUtils.SqlMinValue.Year)
                    {
                        FailMessage($"内容添加失败：系统时间不能为{DateUtils.SqlMinValue.Year}年");
                        return;
                    }
                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = DateTime.Now;

                    if (phContentAttributes.Visible)
                    {
                        foreach (ListItem listItem in ContentAttributes.Items)
                        {
                            var value = listItem.Selected.ToString();
                            var attributeName = listItem.Value;
                            contentInfo.SetExtendedAttribute(attributeName, value);
                        }
                    }

                    contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(ContentLevel.SelectedValue);
                    contentInfo.IsChecked = contentInfo.CheckedLevel >= PublishmentSystemInfo.CheckContentLevel;

                    contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(ContentGroupNameCollection.Items);
                    var tagCollection = TagUtils.ParseTagsString(Tags.Text);
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    contentInfo.Identifier = GovPublicManager.GetIdentifier(PublishmentSystemInfo, categoryChannelId, categoryDepartmentId, contentInfo);
                    var contentId = DataProvider.ContentDao.Insert(_tableName, PublishmentSystemInfo, contentInfo);

                    //更新分类内容数
                    foreach (GovPublicCategoryClassInfo categoryClassInfo in categoryClassInfoArrayList)
                    {
                        var categoryId = TranslateUtils.ToInt(contentInfo.GetExtendedAttribute(categoryClassInfo.ContentAttributeName));
                        if (categoryId > 0)
                        {
                            DataProvider.GovPublicCategoryDao.UpdateContentNum(PublishmentSystemInfo, categoryClassInfo.ContentAttributeName, categoryId);
                        }
                    }

                    if (contentInfo.IsChecked)
                    {
                        CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id);
                    }

                    contentInfo.Id = contentId;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"内容添加失败：{ex.Message}");
                    LogUtils.AddErrorLog(ex);
                    return;
                }

                Body.AddSiteLog(PublishmentSystemId, categoryChannelId, contentInfo.Id, "添加内容",
                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(ddlTranslateType.SelectedValue), Body.AdministratorName);

                PageUtils.Redirect(PageGovPublicContentAddAfter.GetRedirectUrl(PublishmentSystemId, categoryChannelId, contentInfo.Id, Request.QueryString["ReturnUrl"]));
            }
            else
            {
                var contentInfo = DataProvider.GovPublicContentDao.GetContentInfo(PublishmentSystemInfo, _contentId);
                try
                {
                    var oldNodeId = 0;
                    if (contentInfo.NodeId != categoryChannelId)
                    {
                        oldNodeId = contentInfo.NodeId;
                        contentInfo.NodeId = categoryChannelId;
                    }

                    var identifier = contentInfo.Identifier;
                    InputTypeParser.AddValuesToAttributes(_tableStyle, _tableName, PublishmentSystemInfo, _relatedIdentities, Request.Form, contentInfo.Attributes, ContentAttribute.HiddenAttributes);

                    contentInfo.DepartmentId = categoryDepartmentId;
                    SetCategoryAttributes(contentInfo, categoryClassInfoArrayList);

                    contentInfo.Description = tbDescription.Text;
                    contentInfo.PublishDate = dtbPublishDate.DateTime;
                    contentInfo.EffectDate = dtbEffectDate.DateTime;
                    contentInfo.IsAbolition = TranslateUtils.ToBool(rblIsAbolition.SelectedValue);
                    contentInfo.AbolitionDate = dtbAbolitionDate.DateTime;
                    contentInfo.DocumentNo = tbDocumentNo.Text;
                    contentInfo.Publisher = tbPublisher.Text;
                    contentInfo.Keywords = tbKeywords.Text;
                        
                    var contentAttributeNameWithCategoryId = SetCategoryAttributes(contentInfo, categoryClassInfoArrayList);
                    contentInfo.LastEditUserName = Body.AdministratorName;
                    contentInfo.LastEditDate = DateTime.Now;

                    contentInfo.ContentGroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(ContentGroupNameCollection.Items);
                    var tagsLast = contentInfo.Tags;
                    var tagCollection = TagUtils.ParseTagsString(Tags.Text);
                    contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

                    if (phContentAttributes.Visible)
                    {
                        foreach (ListItem listItem in ContentAttributes.Items)
                        {
                            var value = listItem.Selected.ToString();
                            var attributeName = listItem.Value;
                            contentInfo.SetExtendedAttribute(attributeName, value);
                        }
                    }

                    var checkedLevel = TranslateUtils.ToIntWithNagetive(ContentLevel.SelectedValue);
                    if (checkedLevel != LevelManager.LevelInt.NotChange)
                    {
                        contentInfo.IsChecked = checkedLevel >= PublishmentSystemInfo.CheckContentLevel;
                        contentInfo.CheckedLevel = checkedLevel;
                    }

                    if (string.IsNullOrEmpty(identifier))
                    {
                        identifier = GovPublicManager.GetIdentifier(PublishmentSystemInfo, contentInfo.NodeId, contentInfo.DepartmentId, contentInfo);
                    }
                    else if (GovPublicManager.IsIdentifierChanged(categoryChannelId, categoryDepartmentId, dtbEffectDate.DateTime, contentInfo))
                    {
                        identifier = GovPublicManager.GetIdentifier(PublishmentSystemInfo, contentInfo.NodeId, contentInfo.DepartmentId, contentInfo);
                    }
                    contentInfo.Identifier = identifier;

                    DataProvider.ContentDao.Update(_tableName, PublishmentSystemInfo, contentInfo);

                    if (phTags.Visible)
                    {
                        TagUtils.UpdateTags(tagsLast, contentInfo.Tags, tagCollection, PublishmentSystemId, _contentId);
                    }

                    ContentUtility.Translate(PublishmentSystemInfo, _nodeInfo.NodeId, contentInfo.Id, Request.Form["translateCollection"], ETranslateContentTypeUtils.GetEnumType(ddlTranslateType.SelectedValue), Body.AdministratorName);

                    //更新分类内容数
                    foreach (GovPublicCategoryClassInfo categoryClassInfo in categoryClassInfoArrayList)
                    {
                        if (!string.IsNullOrEmpty(contentAttributeNameWithCategoryId[categoryClassInfo.ContentAttributeName]))
                        {
                            var oldCategoryId = TranslateUtils.ToInt(contentAttributeNameWithCategoryId[categoryClassInfo.ContentAttributeName]);
                            var newCategoryId = TranslateUtils.ToInt(contentInfo.GetExtendedAttribute(categoryClassInfo.ContentAttributeName));

                            if (oldCategoryId > 0)
                            {
                                DataProvider.GovPublicCategoryDao.UpdateContentNum(PublishmentSystemInfo, categoryClassInfo.ContentAttributeName, oldCategoryId);
                            }
                            if (newCategoryId > 0)
                            {
                                DataProvider.GovPublicCategoryDao.UpdateContentNum(PublishmentSystemInfo, categoryClassInfo.ContentAttributeName, newCategoryId);
                            }
                        }
                    }

                    if (oldNodeId > 0)
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo, oldNodeId, true);
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo, categoryChannelId, true);
                    }
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"内容修改失败：{ex.Message}");
                    LogUtils.AddErrorLog(ex);
                    return;
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(PublishmentSystemId, categoryChannelId, _contentId);
                }

                Body.AddSiteLog(PublishmentSystemId, categoryChannelId, _contentId, "修改内容",
                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},内容标题:{contentInfo.Title}");

                PageUtils.Redirect(ReturnUrl);
            }
        }

        private NameValueCollection SetCategoryAttributes(GovPublicContentInfo contentInfo, ArrayList categoryClassInfoArrayList)
        {
            var contentAttributeNameWithCategoryId = new NameValueCollection();
            foreach (GovPublicCategoryClassInfo categoryClassInfo in categoryClassInfoArrayList)
            {
                var oldCategoryId = TranslateUtils.ToInt(contentInfo.GetExtendedAttribute(categoryClassInfo.ContentAttributeName));
                var newCategoryId = TranslateUtils.ToInt(Request[$"category{categoryClassInfo.ClassCode}ID"]);
                if (oldCategoryId != newCategoryId)
                {
                    contentAttributeNameWithCategoryId.Add(categoryClassInfo.ContentAttributeName, oldCategoryId.ToString());
                    contentInfo.SetExtendedAttribute(categoryClassInfo.ContentAttributeName, newCategoryId.ToString());
                }
            }
            return contentAttributeNameWithCategoryId;
        }

        public string ReturnUrl { get; private set; }
    }
}

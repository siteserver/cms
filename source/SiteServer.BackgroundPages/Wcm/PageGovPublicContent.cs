using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.User;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicContent : BasePageGovPublic
    {
        public Repeater rptContents;
        public SqlPager spContents;
        public Literal ltlColumnHeadRows;

        public PlaceHolder phContentButtons;
        public Literal ltlContentButtons;

        public DateTimeTextBox DateFrom;
        public DropDownList SearchType;
        public TextBox Keyword;

        private int _departmentId;
        private string _classCode;
        private int _categoryId;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;

        private readonly Hashtable _valueHashtable = new Hashtable();

        public static string GetRedirectUrl(int publishmentSystemId, string classCode, int categoryId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ClassCode", classCode},
                {"CategoryID", categoryId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int departmentId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"DepartmentID", departmentId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var permissioins = PermissionsManager.GetPermissions(Body.AdministratorName);

            _departmentId = TranslateUtils.ToInt(Request.QueryString["DepartmentID"]);
            _classCode = Request.QueryString["ClassCode"];
            _categoryId = TranslateUtils.ToInt(Request.QueryString["CategoryID"]);

            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.GovPublicContent, PublishmentSystemInfo.AuxiliaryTableForGovPublic, relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId));

            spContents.ControlToPaginate = rptContents;
            rptContents.ItemDataBound += rptContents_ItemDataBound;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            
            if (Body.IsQueryExists("SearchType"))
            {
                var owningNodeIdList = new List<int>
                {
                    PublishmentSystemInfo.Additional.GovPublicNodeId
                };
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(ETableStyle.GovPublicContent, PublishmentSystemInfo.AuxiliaryTableForGovPublic, PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId, permissioins.IsSystemAdministrator, owningNodeIdList, Request.QueryString["SearchType"], Request.QueryString["Keyword"], Request.QueryString["DateFrom"], string.Empty, false, ETriState.All, false, false);
            }
            else
            {                
                if (_departmentId > 0)
                {
                    spContents.SelectCommand = DataProvider.GovPublicContentDao.GetSelectCommendByDepartmentId(PublishmentSystemInfo, _departmentId);
                }
                else if (!string.IsNullOrEmpty(_classCode) && _categoryId > 0)
                {
                    spContents.SelectCommand = DataProvider.GovPublicContentDao.GetSelectCommendByCategoryId(PublishmentSystemInfo, _classCode, _categoryId);
                }
            }

            spContents.SortField = ContentAttribute.Id;
            spContents.SortMode = SortMode.DESC;

			if(!IsPostBack)
			{
                var nodeName = string.Empty;
                if (_departmentId > 0)
                {
                    nodeName = DepartmentManager.GetDepartmentName(_departmentId);
                }
                else if (!string.IsNullOrEmpty(_classCode) && _categoryId > 0)
                {
                    nodeName = DataProvider.GovPublicCategoryDao.GetCategoryName(_categoryId);
                }

                BreadCrumbWithItemTitle(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContent, "信息管理", nodeName, AppManager.Wcm.Permission.WebSite.GovPublicContent);
                
                spContents.DataBind();

                if (_styleInfoList != null)
                {
                    foreach (var styleInfo in _styleInfoList)
                    {
                        if (styleInfo.IsVisible)
                        {
                            var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            SearchType.Items.Add(listitem);
                        }
                    }
                }

                //添加隐藏属性
                SearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                SearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                SearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));
                SearchType.Items.Add(new ListItem("内容组", ContentAttribute.ContentGroupNameCollection));

                if (Body.IsQueryExists("SearchType"))
                {
                    DateFrom.Text = Request.QueryString["DateFrom"];
                    ControlUtils.SelectListItems(SearchType, Request.QueryString["SearchType"]);
                    Keyword.Text = Request.QueryString["Keyword"];
                    ltlContentButtons.Text += @"
<script>
$(document).ready(function() {
	$('#contentSearch').show();
});
</script>
";
                }

                ltlColumnHeadRows.Text = ContentUtility.GetColumnHeadRowsHtml(_styleInfoList, _attributesOfDisplay, ETableStyle.GovPublicContent, PublishmentSystemInfo);
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlItemTitle = e.Item.FindControl("ltlItemTitle") as Literal;
                var ltlColumnItemRows = e.Item.FindControl("ltlColumnItemRows") as Literal;
                var ltlItemStatus = e.Item.FindControl("ltlItemStatus") as Literal;
                var ltlItemEditUrl = e.Item.FindControl("ltlItemEditUrl") as Literal;

                var contentInfo = new ContentInfo(e.Item.DataItem);

                ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

                var showPopWinString = ModalCheckState.GetOpenWindowString(PublishmentSystemId, contentInfo, PageUrl);
                ltlItemStatus.Text =
                    $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{showPopWinString}"">{LevelManager.GetCheckState(
                        PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

                if (HasChannelPermissions(contentInfo.NodeId, AppManager.Cms.Permission.Channel.ContentEdit) || Body.AdministratorName == contentInfo.AddUserName)
                {
                    ltlItemEditUrl.Text =
                        $"<a href=\"{PageGovPublicContentAdd.GetRedirectUrlOfEdit(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, PageUrl)}\">编辑</a>";
                }

                ltlColumnItemRows.Text = TextUtility.GetColumnItemRowsHtml(_styleInfoList, _attributesOfDisplay, _valueHashtable, ETableStyle.GovPublicContent, PublishmentSystemInfo, contentInfo);
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    if (_departmentId > 0)
                    {
                        _pageUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicContent), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"DepartmentID", _departmentId.ToString()},
                            {"page", TranslateUtils.ToInt(Request.QueryString["page"], 1).ToString()}
                        });
                    }
                    else if (!string.IsNullOrEmpty(_classCode) && _categoryId > 0)
                    {
                        _pageUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicContent), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"ClassCode", _classCode},
                            {"CategoryID", _categoryId.ToString()},
                            {"page", TranslateUtils.ToInt(Request.QueryString["page"], 1).ToString()}
                        });
                    }
                }
                return _pageUrl;
            }
        }
	}
}

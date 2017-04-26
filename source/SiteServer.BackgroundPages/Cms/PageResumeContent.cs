using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageResumeContent : BasePageCms
    {
        public Repeater rptContents;
        public SqlPager spContents;

		public Button Delete;
        public Button SetIsView;
        public Button SetNotView;
        public Button Return;

        private int _jobContentId;
        private string _returnUrl;
        private readonly Hashtable _hashtable = new Hashtable();

        public static string GetRedirectUrl(int publishmentSystemId, int jobContentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageResumeContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"JobContentID", jobContentId.ToString()},
                {"ReturnUrl", returnUrl}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var pageTitle = string.Empty;
            if (Body.IsQueryExists("JobContentID"))
            {
                _jobContentId = Body.GetQueryInt("JobContentID");
                _returnUrl = Body.GetQueryString("ReturnUrl");
                if (_jobContentId == 0) return;

                pageTitle = BaiRongDataProvider.ContentDao.GetValue(PublishmentSystemInfo.AuxiliaryTableForJob, _jobContentId, ContentAttribute.Title);
                Return.Visible = true;
            }

            if (Body.IsQueryExists("Delete"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                if (arraylist.Count > 0)
                {
                    try
                    {
                        DataProvider.ResumeContentDao.Delete(arraylist);
                        Body.AddSiteLog(PublishmentSystemId, "删除简历");
                        SuccessMessage("删除成功！");
                        PageUtils.Redirect(PageUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }
            else if (Body.IsQueryExists("SetView"))
            {
                var isView = Body.GetQueryBool("IsView");
                var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                if (arraylist.Count > 0)
                {
                    try
                    {
                        DataProvider.ResumeContentDao.SetIsView(arraylist, isView);
                        SuccessMessage("设置成功！");
                        PageUtils.Redirect(PageUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "设置失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SelectCommand = DataProvider.ResumeContentDao.GetSelectStringOfId(PublishmentSystemId, _jobContentId, string.Empty);
            spContents.SortField = DataProvider.ResumeContentDao.GetSortFieldName();
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdResume,
                    $"{pageTitle}({spContents.TotalCount})", AppManager.Cms.Permission.WebSite.Resume);

                spContents.DataBind();

                Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Delete=True", "ContentIDCollection", "ContentIDCollection", "请选择需要删除的简历！", "此操作将删除所选内容，确定吗？"));

                SetIsView.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUrl + "&SetView=True&IsView=True", "ContentIDCollection", "ContentIDCollection", "请选择需要设置的简历！"));

                SetNotView.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUrl + "&SetView=True&IsView=False", "ContentIDCollection", "ContentIDCollection", "请选择需要设置的简历！"));
			}			
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var contentID = SqlUtils.EvalInt(e.Item.DataItem, "ID");
                var contentInfo = DataProvider.ResumeContentDao.GetContentInfo(contentID);

                var ltlTr = e.Item.FindControl("ltlTr") as Literal;
                var ltlRealName = e.Item.FindControl("ltlRealName") as Literal;
                var ltlGender = e.Item.FindControl("ltlGender") as Literal;
                var ltlMobilePhone = e.Item.FindControl("ltlMobilePhone") as Literal;
                var ltlEmail = e.Item.FindControl("ltlEmail") as Literal;
                var ltlEducation = e.Item.FindControl("ltlEducation") as Literal;
                var ltlJobTitle = e.Item.FindControl("ltlJobTitle") as Literal;
                var ltlLastSchoolName = e.Item.FindControl("ltlLastSchoolName") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlViewUrl = e.Item.FindControl("ltlViewUrl") as Literal;

                ltlTr.Text = $@"<tr style=""height:25px;font-weight:{(contentInfo.IsView ? "normal" : "bold")}"">";

                ltlRealName.Text = contentInfo.GetExtendedAttribute(ResumeContentAttribute.RealName);
                if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(ResumeContentAttribute.ImageUrl)))
                {
                    var imageUrl = PageUtility.ParseNavigationUrl(PublishmentSystemInfo,
                        contentInfo.GetExtendedAttribute(ResumeContentAttribute.ImageUrl));
                    ltlRealName.Text +=
                        $@"&nbsp;<a id=""preview_{contentID}"" href=""{imageUrl}"">预览相片</a>
<script type=""text/javascript"">
$(document).ready(function() {{
	$(""#preview_{contentID}"").fancybox();
}});
</script>";
                }
                ltlGender.Text = contentInfo.GetExtendedAttribute(ResumeContentAttribute.Gender);
                ltlMobilePhone.Text = contentInfo.GetExtendedAttribute(ResumeContentAttribute.MobilePhone);
                ltlEmail.Text = contentInfo.GetExtendedAttribute(ResumeContentAttribute.Email);
                ltlEducation.Text = contentInfo.GetExtendedAttribute(ResumeContentAttribute.Education);
                if (contentInfo.JobContentId > 0)
                {
                    var title = _hashtable[contentInfo.JobContentId] as string;
                    if (title == null)
                    {
                        title = BaiRongDataProvider.ContentDao.GetValue(PublishmentSystemInfo.AuxiliaryTableForJob, contentInfo.JobContentId, ContentAttribute.Title);
                        _hashtable[contentInfo.JobContentId] = title;
                    }
                    ltlJobTitle.Text = title;
                }
                ltlLastSchoolName.Text = contentInfo.GetExtendedAttribute(ResumeContentAttribute.LastSchoolName);
                ltlAddDate.Text = DateUtils.GetDateString(contentInfo.AddDate);

                ltlViewUrl.Text =
                    $@"<a href=""{PageResumeView.GetRedirectUrl(PublishmentSystemId, contentID)}"" target=""_blank"" onclick=""this.parentNode.parentNode.style.fontWeight='normal';"">查看</a>";
            }
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = GetRedirectUrl(PublishmentSystemId, _jobContentId, _returnUrl);
                }
                return _pageUrl;
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(StringUtils.ValueFromUrl(_returnUrl));
        }
	}
}

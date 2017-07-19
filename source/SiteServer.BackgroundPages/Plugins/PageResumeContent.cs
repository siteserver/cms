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

namespace SiteServer.BackgroundPages.Plugins
{
	public class PageResumeContent : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

		public Button BtnDelete;
        public Button BtnSetIsView;
        public Button BtnSetNotView;
        public Button BtnReturn;

        private int _jobContentId;
        private string _returnUrl;
        private readonly Hashtable _hashtable = new Hashtable();

        public override int PublishmentSystemId => Body.GetQueryInt("siteId");

        public static string GetRedirectUrl(int siteId, int contentId, string returnUrl)
        {
            return PageUtils.GetPluginsUrl(nameof(PageResumeContent), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"contentId", contentId.ToString()},
                {"returnUrl", returnUrl}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _jobContentId = Body.GetQueryInt("contentID");
            _returnUrl = Body.GetQueryString("returnUrl");

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

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            SpContents.SelectCommand = DataProvider.ResumeContentDao.GetSelectStringOfId(PublishmentSystemId, _jobContentId, string.Empty);
            SpContents.SortField = DataProvider.ResumeContentDao.GetSortFieldName();
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

			if(!IsPostBack)
			{
                SpContents.DataBind();

                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Delete=True", "ContentIDCollection", "ContentIDCollection", "请选择需要删除的简历！", "此操作将删除所选内容，确定吗？"));

                BtnSetIsView.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUrl + "&SetView=True&IsView=True", "ContentIDCollection", "ContentIDCollection", "请选择需要设置的简历！"));

                BtnSetNotView.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUrl + "&SetView=True&IsView=False", "ContentIDCollection", "ContentIDCollection", "请选择需要设置的简历！"));
			}			
		}

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentId = SqlUtils.EvalInt(e.Item.DataItem, "ID");
            var contentInfo = DataProvider.ResumeContentDao.GetContentInfo(contentId);

            var ltlTr = (Literal)e.Item.FindControl("ltlTr");
            var ltlRealName = (Literal)e.Item.FindControl("ltlRealName");
            var ltlGender = (Literal)e.Item.FindControl("ltlGender");
            var ltlMobilePhone = (Literal)e.Item.FindControl("ltlMobilePhone");
            var ltlEmail = (Literal)e.Item.FindControl("ltlEmail");
            var ltlEducation = (Literal)e.Item.FindControl("ltlEducation");
            var ltlJobTitle = (Literal)e.Item.FindControl("ltlJobTitle");
            var ltlLastSchoolName = (Literal)e.Item.FindControl("ltlLastSchoolName");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlViewUrl = (Literal)e.Item.FindControl("ltlViewUrl");

            ltlTr.Text = $@"<tr style=""height:25px;font-weight:{(contentInfo.IsView ? "normal" : "bold")}"">";

            ltlRealName.Text = contentInfo.GetExtendedAttribute(ResumeContentAttribute.RealName);
            if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(ResumeContentAttribute.ImageUrl)))
            {
                var imageUrl = PageUtility.ParseNavigationUrl(PublishmentSystemInfo,
                    contentInfo.GetExtendedAttribute(ResumeContentAttribute.ImageUrl));
                ltlRealName.Text +=
                    $@"&nbsp;<a id=""preview_{contentId}"" href=""{imageUrl}"">预览相片</a>
<script type=""text/javascript"">
$(document).ready(function() {{
	$(""#preview_{contentId}"").fancybox();
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
                $@"<a href=""{PageResumeView.GetRedirectUrl(PublishmentSystemId, contentId)}"" target=""_blank"" onclick=""this.parentNode.parentNode.style.fontWeight='normal';"">查看</a>";
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

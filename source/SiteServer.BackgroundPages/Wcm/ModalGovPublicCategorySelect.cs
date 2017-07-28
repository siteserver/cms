using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicCategorySelect : BasePageCms
	{
        public Literal ltlCategoryName;
        public Repeater rptCategory;

        private string _classCode = string.Empty;

	    public static string GetOpenWindowString(int publishmentSystemId, string classCode, int categoryId)
	    {
	        return PageUtils.GetOpenWindowString("选择分类",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicCategorySelect), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"ClassCode", classCode},
	                {"CategoryID", categoryId.ToString()}
	            }), 500, 360);
	    }

	    public static string GetOpenWindowString(int publishmentSystemId, string classCode)
	    {
	        return PageUtils.GetOpenWindowString("设置分类",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicCategorySelect), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"ClassCode", classCode}
	            }), 460, 360, true);
	    }

        public static string GetRedirectUrl(int publishmentSystemId, string classCode, int categoryId)
        {
            return PageUtils.GetWcmUrl(nameof(ModalGovPublicCategorySelect), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"ClassCode", classCode},
                    {"CategoryID", categoryId.ToString()}
                });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _classCode = Request.QueryString["ClassCode"];

			if (!IsPostBack)
			{
                var categoryClassInfo = DataProvider.GovPublicCategoryClassDao.GetCategoryClassInfo(_classCode, PublishmentSystemId);
                ltlCategoryName.Text = categoryClassInfo.ClassName;

                if (Body.IsQueryExists("CategoryID"))
                {
                    var categoryId = TranslateUtils.ToInt(Request.QueryString["CategoryID"]);
                    var categoryName = DataProvider.GovPublicCategoryDao.GetCategoryName(categoryId);
                    string scripts = $"window.parent.showCategory{_classCode}('{categoryName}', '{categoryId}');";
                    PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                }
                else
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", GovPublicCategoryTreeItem.GetScript(_classCode, PublishmentSystemId, EGovPublicCategoryLoadingType.Select, null));
                    BindGrid();
                }
			}
		}

        public void BindGrid()
        {
            try
            {
                rptCategory.DataSource = DataProvider.GovPublicCategoryDao.GetCategoryIdArrayListByParentId(_classCode, PublishmentSystemId, 0);
                rptCategory.ItemDataBound += rptCategory_ItemDataBound;
                rptCategory.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        private void rptCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var categoryId = (int)e.Item.DataItem;
            var categoryInfo = DataProvider.GovPublicCategoryDao.GetCategoryInfo(categoryId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = PageGovPublicCategory.GetCategoryRowHtml(categoryInfo, true, EGovPublicCategoryLoadingType.Select, null);
        }
	}
}

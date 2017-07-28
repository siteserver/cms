using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovInteractDepartmentSelect : BasePageCms
    {
        public Repeater rptCategory;

        private int _nodeId;
        private readonly NameValueCollection _additional = new NameValueCollection();

        protected override bool IsSinglePage => true;

	    public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowString("选择部门", PageUtils.GetWcmUrl(nameof(ModalGovInteractDepartmentSelect), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            }), 460, 360, true);
        }

        //PageUtils.GetWcmUrl(nameof(ModalGovInteractDepartmentSelect), new NameValueCollection())$"modal_govInteractDepartmentSelect.aspx?PublishmentSystemID={PublishmentSystemID}" + "&DepartmentID={0}"));

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);
            _additional.Add("UrlFormatString",
                PageUtils.GetWcmUrl(nameof(ModalGovInteractDepartmentSelect), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()}
                }));

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("DepartmentID"))
                {
                    var departmentId = TranslateUtils.ToInt(Request.QueryString["DepartmentID"]);
                    var departmentName = DepartmentManager.GetDepartmentName(departmentId);
                    string scripts = $"window.parent.showCategoryDepartment('{departmentName}', '{departmentId}');";
                    PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                }
                else
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.DepartmentSelect, _additional));
                    BindGrid();
                }
			}
		}

        public void BindGrid()
        {
            try
            {
                var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(PublishmentSystemId, _nodeId);
                rptCategory.DataSource = GovInteractManager.GetFirstDepartmentIdList(channelInfo);
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
            var departmentID = (int)e.Item.DataItem;
            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentID);

            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;

            //ltlHtml.Text = BackgroundDepartment.GetDepartmentRowHtml(departmentInfo, EDepartmentLoadingType.DepartmentSelect, additional);
        }
	}
}

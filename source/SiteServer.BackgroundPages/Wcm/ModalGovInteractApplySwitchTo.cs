using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovInteractApplySwitchTo : BasePageCms
	{
        protected TextBox tbSwitchToRemark;
        public HtmlControl divAddDepartment;
        public Literal ltlDepartmentName;
        public Literal ltlUserName;

        private int _nodeId;
        private List<int> _idArrayList;

	    public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
	    {
	        return PageUtils.GetOpenWindowStringWithCheckBoxValue("转办办件",
	            PageUtils.GetWcmUrl(nameof(ModalGovInteractApplySwitchTo), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"NodeID", nodeId.ToString()}
	            }), "IDCollection", "请选择需要转办的办件！", 500, 500);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);
            _idArrayList = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);

			if (!IsPostBack)
			{
                divAddDepartment.Attributes.Add("onclick", ModalGovInteractDepartmentSelect.GetOpenWindowString(PublishmentSystemId, _nodeId));
                ltlDepartmentName.Text = DepartmentManager.GetDepartmentName(Body.AdministratorInfo.DepartmentId);
                ltlUserName.Text = Body.AdministratorInfo.DisplayName;
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
            try
            {
                var switchToDepartmentID = TranslateUtils.ToInt(Request.Form["switchToDepartmentID"]);
                if (switchToDepartmentID == 0)
                {
                    FailMessage("转办失败，必须选择转办部门");
                    return;
                }
                var switchToDepartmentName = DepartmentManager.GetDepartmentName(switchToDepartmentID);

                foreach (int contentID in _idArrayList)
                {
                    var contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(PublishmentSystemInfo, contentID);
                    if (contentInfo.State != EGovInteractState.Denied && contentInfo.State != EGovInteractState.Checked)
                    {
                        DataProvider.GovInteractContentDao.UpdateDepartmentId(PublishmentSystemInfo, contentID, switchToDepartmentID);

                        if (!string.IsNullOrEmpty(tbSwitchToRemark.Text))
                        {
                            var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, contentInfo.NodeId, contentID, EGovInteractRemarkType.SwitchTo, tbSwitchToRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                            DataProvider.GovInteractRemarkDao.Insert(remarkInfo);
                        }

                        GovInteractApplyManager.LogSwitchTo(PublishmentSystemId, contentInfo.NodeId, contentID, switchToDepartmentName, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                    }
                }

                isChanged = true;
            }
			catch(Exception ex)
			{
                FailMessage(ex, ex.Message);
			    isChanged = false;
			}

			if (isChanged)
			{
                PageUtils.CloseModalPage(Page, "alert(\'办件转办成功!\');");
			}
		}

	}
}

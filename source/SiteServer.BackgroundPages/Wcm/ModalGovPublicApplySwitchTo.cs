using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicApplySwitchTo : BasePageCms
	{
        protected TextBox tbSwitchToRemark;
        public HtmlControl divAddDepartment;
        public Literal ltlDepartmentName;
        public Literal ltlUserName;

        private List<int> _idArrayList;

	    public static string GetOpenWindowString(int publishmentSystemId)
	    {
	        return PageUtils.GetOpenWindowStringWithCheckBoxValue("转办申请",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicApplySwitchTo), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()}
	            }), "IDCollection", "请选择需要转办的申请！", 500, 500);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _idArrayList = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);

			if (!IsPostBack)
			{
                divAddDepartment.Attributes.Add("onclick", ModalGovPublicCategoryDepartmentSelect.GetOpenWindowString(PublishmentSystemId));
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

                foreach (int applyID in _idArrayList)
                {
                    var state = DataProvider.GovPublicApplyDao.GetState(applyID);
                    if (state != EGovPublicApplyState.Denied && state != EGovPublicApplyState.Checked)
                    {
                        DataProvider.GovPublicApplyDao.UpdateDepartmentId(applyID, switchToDepartmentID);

                        var remarkInfo = new GovPublicApplyRemarkInfo(0, PublishmentSystemId, applyID, EGovPublicApplyRemarkType.SwitchTo, tbSwitchToRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                        DataProvider.GovPublicApplyRemarkDao.Insert(remarkInfo);

                        GovPublicApplyManager.LogSwitchTo(PublishmentSystemId, applyID, switchToDepartmentName, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
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
                PageUtils.CloseModalPage(Page, "alert(\'申请转办成功!\');");
			}
		}

	}
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicApplyRedo : BasePageCms
	{
        protected TextBox tbRedoRemark;
        public Literal ltlDepartmentName;
        public Literal ltlUserName;

        private List<int> _idArrayList;

	    public static string GetOpenWindowString(int publishmentSystemId)
	    {
	        return PageUtils.GetOpenWindowStringWithCheckBoxValue("要求返工",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicApplyRedo), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()}
	            }), "IDCollection", "请选择需要返工的申请！", 450, 320);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _idArrayList = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);

			if (!IsPostBack)
			{
                ltlDepartmentName.Text = DepartmentManager.GetDepartmentName(Body.AdministratorInfo.DepartmentId);
                ltlUserName.Text = Body.AdministratorInfo.DisplayName;
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
            try
            {
                if (string.IsNullOrEmpty(tbRedoRemark.Text))
                {
                    FailMessage("要求返工失败，必须填写意见");
                    return;
                }

                foreach (int applyID in _idArrayList)
                {
                    var state = DataProvider.GovPublicApplyDao.GetState(applyID);
                    if (state == EGovPublicApplyState.Replied || state == EGovPublicApplyState.Redo)
                    {
                        var remarkInfo = new GovPublicApplyRemarkInfo(0, PublishmentSystemId, applyID, EGovPublicApplyRemarkType.Redo, tbRedoRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                        DataProvider.GovPublicApplyRemarkDao.Insert(remarkInfo);

                        GovPublicApplyManager.Log(PublishmentSystemId, applyID, EGovPublicApplyLogType.Redo, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                        DataProvider.GovPublicApplyDao.UpdateState(applyID, EGovPublicApplyState.Redo);
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
                PageUtils.CloseModalPage(Page, "alert(\'要求返工成功!\');");
			}
		}

	}
}

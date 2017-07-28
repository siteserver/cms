using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovInteractApplyComment : BasePageCms
    {
        protected TextBox tbCommentRemark;
        public Literal ltlDepartmentName;
        public Literal ltlUserName;

        private List<int> _idArrayList;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("批示",
                PageUtils.GetWcmUrl(nameof(ModalGovInteractApplyComment), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                }), "IDCollection", "请选择需要批示的申请！", 450, 320);
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
                if (string.IsNullOrEmpty(tbCommentRemark.Text))
                {
                    FailMessage("批示失败，必须填写意见");
                    return;
                }

                foreach (int contentID in _idArrayList)
                {
                    var nodeID = DataProvider.GovInteractContentDao.GetNodeId(PublishmentSystemInfo, contentID);
                    var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, nodeID, contentID, EGovInteractRemarkType.Comment, tbCommentRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                    DataProvider.GovInteractRemarkDao.Insert(remarkInfo);

                    GovInteractApplyManager.Log(PublishmentSystemId, nodeID, contentID, EGovInteractLogType.Comment, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
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
                PageUtils.CloseModalPage(Page, "alert(\'批示成功!\');");
			}
		}

	}
}

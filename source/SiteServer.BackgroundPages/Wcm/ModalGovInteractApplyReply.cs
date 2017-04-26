using System;
using System.Collections.Specialized;
using System.Web;
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
	public class ModalGovInteractApplyReply : BasePageCms
	{
        protected TextBox tbReply;
        public Literal ltlDepartmentName;
        public Literal ltlUserName;
        public HtmlInputFile htmlFileUrl;

        private GovInteractContentInfo _contentInfo;

	    public static string GetOpenWindowString(int publishmentSystemId, int nodeId, int contentId)
	    {
	        return PageUtils.GetOpenWindowString("回复办件",
	            PageUtils.GetWcmUrl(nameof(ModalGovInteractApplyReply), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"ContentID", contentId.ToString()}
	            }), 600, 450);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ContentID");

            _contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(PublishmentSystemInfo, TranslateUtils.ToInt(Request.QueryString["ContentID"]));

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
                DataProvider.GovInteractReplyDao.DeleteByContentId(PublishmentSystemId, _contentInfo.Id);
                var fileUrl = UploadFile(htmlFileUrl.PostedFile);
                var replyInfo = new GovInteractReplyInfo(0, PublishmentSystemId, _contentInfo.NodeId, _contentInfo.Id, tbReply.Text, fileUrl, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovInteractReplyDao.Insert(replyInfo);

                GovInteractApplyManager.Log(PublishmentSystemId, _contentInfo.NodeId, _contentInfo.Id, EGovInteractLogType.Reply, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                if (Body.AdministratorInfo.DepartmentId > 0)
                {
                    DataProvider.GovInteractContentDao.UpdateDepartmentId(PublishmentSystemInfo, _contentInfo.Id, Body.AdministratorInfo.DepartmentId);
                }
                DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, _contentInfo.Id, EGovInteractState.Replied);

                isChanged = true;
            }
			catch(Exception ex)
			{
                FailMessage(ex, ex.Message);
			    isChanged = false;
			}

			if (isChanged)
			{
                PageUtils.CloseModalPage(Page, "alert(\'办件回复成功!\');");
			}
		}

        private string UploadFile(HttpPostedFile myFile)
        {
            var fileUrl = string.Empty;

            if (myFile != null && !string.IsNullOrEmpty(myFile.FileName))
            {
                var filePath = myFile.FileName;
                try
                {
                    var fileExtName = PathUtils.GetExtension(filePath);
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, filePath);

                    var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                    if (!PathUtility.IsFileExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                    {
                        return string.Empty;
                    }
                    if (!PathUtility.IsFileSizeAllowed(PublishmentSystemInfo, myFile.ContentLength))
                    {
                        return string.Empty;
                    }

                    myFile.SaveAs(localFilePath);
                    FileUtility.AddWaterMark(PublishmentSystemInfo, localFilePath);

                    fileUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                    fileUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, fileUrl);
                }
                catch { }
            }

            return fileUrl;
        }
	}
}

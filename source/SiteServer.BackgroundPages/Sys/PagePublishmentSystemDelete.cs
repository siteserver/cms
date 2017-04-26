using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;

namespace SiteServer.BackgroundPages.Sys
{
	public class PagePublishmentSystemDelete : BasePageCms
    {
        public PlaceHolder phIsRetainFiles;
		public RadioButtonList RetainFiles;
        public PlaceHolder phReturn;

		private int _nodeId;
        private bool _isBackgroundDelete;

	    public static string GetRedirectUrl(int publishmentSystemId)
	    {
	        return PageUtils.GetSysUrl(nameof(PagePublishmentSystemDelete), new NameValueCollection
	        {
	            {"NodeID", publishmentSystemId.ToString()}
	        });
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			_nodeId = Body.GetQueryInt("nodeID");
            _isBackgroundDelete = Body.GetQueryBool("isBackgroundDelete");

			if (!IsPostBack)
            {
                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "删除站点", AppManager.Sys.Permission.SysSite);

                phIsRetainFiles.Visible = phReturn.Visible = !_isBackgroundDelete;

                var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(_nodeId);
                InfoMessage($"此操作将会删除站点“{psInfo.PublishmentSystemName}({psInfo.PublishmentSystemDir})”，确认吗？");
			}
		}

        public string GetReturnUrl()
        {
            return PagePublishmentSystem.GetRedirectUrl();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				try
				{
                    var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(_nodeId);

                    var isRetainFiles = false;
                    if (!_isBackgroundDelete)
                    {
                        isRetainFiles = TranslateUtils.ToBool(RetainFiles.SelectedValue);
                    }

                    if (isRetainFiles == false)
                    {
                        DirectoryUtility.DeletePublishmentSystemFiles(psInfo);
                        SuccessMessage("成功删除站点以及相关文件！");                       
                    }
                    else
                    {
                        SuccessMessage("成功删除站点，相关文件未被删除！");
                    }

                    if (!_isBackgroundDelete)
                    {
                        AddWaitAndRedirectScript(PagePublishmentSystem.GetRedirectUrl());
                    }
                    else
                    {
                        AddScript(
                            $@"setTimeout(""window.top.location.href='{PageMain.GetRedirectUrl()}'"", 1500);");
                    }

                    DataProvider.PublishmentSystemDao.Delete(_nodeId);

                    Body.AddAdminLog("删除站点", $"站点:{psInfo.PublishmentSystemName}");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "删除系统失败！");
				}
			}
		}

	}
}

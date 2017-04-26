using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTagStyleMenu : BasePageCms
    {
		public DataGrid dgContents;

		public bool IsDefault(string isDefault)
		{
            return TranslateUtils.ToBool(isDefault);
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("Delete"))
            {
                var menuDisplayId = Body.GetQueryInt("MenuDisplayID");

                try
                {
                    DataProvider.MenuDisplayDao.Delete(menuDisplayId);
                    Body.AddSiteLog(PublishmentSystemId, "删除菜单显示方式");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("SetDefault"))
            {
                var menuDisplayId = Body.GetQueryInt("MenuDisplayID");

                try
                {
                    DataProvider.MenuDisplayDao.SetDefault(menuDisplayId);
                    SuccessMessage();
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "操作失败");
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, AppManager.Cms.LeftMenu.Template.IdTagStyle, "下拉菜单样式", AppManager.Cms.Permission.WebSite.Template);

                dgContents.DataSource = DataProvider.MenuDisplayDao.GetDataSource(PublishmentSystemId);
                dgContents.DataBind();
            }
		}
	}
}

using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationComment : BasePageCms
    {
        public RadioButtonList RblIsCommentable;
        public PlaceHolder PhComments;
        public RadioButtonList RblIsCheckComments;
        public RadioButtonList RblIsAnonymousComments;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;

            BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "评论管理设置", AppManager.Cms.Permission.WebSite.Configration);

            ControlUtils.SelectListItemsIgnoreCase(RblIsCommentable, PublishmentSystemInfo.Additional.IsCommentable.ToString());
            ControlUtils.SelectListItemsIgnoreCase(RblIsCheckComments, PublishmentSystemInfo.Additional.IsCheckComments.ToString());
            ControlUtils.SelectListItemsIgnoreCase(RblIsAnonymousComments, PublishmentSystemInfo.Additional.IsAnonymousComments.ToString());
            PhComments.Visible = PublishmentSystemInfo.Additional.IsCommentable;
        }

        public void RblIsCommentable_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PhComments.Visible = EBooleanUtils.Equals(RblIsCommentable.SelectedValue, EBoolean.True);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    PublishmentSystemInfo.Additional.IsCommentable = TranslateUtils.ToBool(RblIsCommentable.SelectedValue);
		    PublishmentSystemInfo.Additional.IsCheckComments = TranslateUtils.ToBool(RblIsCheckComments.SelectedValue);
            PublishmentSystemInfo.Additional.IsAnonymousComments = TranslateUtils.ToBool(RblIsAnonymousComments.SelectedValue);

            try
		    {
		        DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

		        Body.AddSiteLog(PublishmentSystemId, "修改评论管理设置");

		        SuccessMessage("评论管理设置修改成功！");
		    }
		    catch (Exception ex)
		    {
		        FailMessage(ex, "评论管理设置修改失败！");
		    }
		}
	}
}

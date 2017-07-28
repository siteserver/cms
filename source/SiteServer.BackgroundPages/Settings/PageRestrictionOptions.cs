using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageRestrictionOptions : BasePage
    {
        public RadioButtonList RestrictionType;
		
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Restriction, "访问限制选项", AppManager.Settings.Permission.SettingsRestriction);

                ERestrictionTypeUtils.AddListItems(RestrictionType);
                ControlUtils.SelectListItemsIgnoreCase(RestrictionType, ERestrictionTypeUtils.GetValue(ConfigManager.SystemConfigInfo.RestrictionType));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                ConfigManager.SystemConfigInfo.RestrictionType = ERestrictionTypeUtils.GetEnumType(RestrictionType.SelectedValue);

                try
                {
                    BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                    Body.AddAdminLog("设置访问限制选项");

                    SuccessMessage("访问限制选项修改成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "访问限制选项修改失败！");
                }
            }
        }
	}
}

using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageConfigurationCreate : BasePageCms
    {
        public RadioButtonList IsCreateContentIfContentChanged;
        public RadioButtonList IsCreateChannelIfChannelChanged;

        public RadioButtonList IsCreateShowPageInfo;

        public RadioButtonList IsCreateIE8Compatible;
        public RadioButtonList IsCreateBrowserNoCache;
        public RadioButtonList IsCreateJsIgnoreError;
        public RadioButtonList IsCreateSearchDuplicate;

        public RadioButtonList IsCreateIncludeToSSI;
        public RadioButtonList IsCreateWithJQuery;

        public RadioButtonList IsCreateDoubleClick;
        public TextBox tbCreateStaticMaxPage;

        public RadioButtonList IsCreateStaticContentByAddDate;
        public PlaceHolder phIsCreateStaticContentByAddDate;
        public DateTimeTextBox tbCreateStaticContentAddDate;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdCreate, AppManager.Cms.LeftMenu.Create.IdConfigurationCreate, "页面生成设置", AppManager.Cms.Permission.WebSite.Create);

                EBooleanUtils.AddListItems(IsCreateContentIfContentChanged, "生成", "不生成");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateContentIfContentChanged, PublishmentSystemInfo.Additional.IsCreateContentIfContentChanged.ToString());

                EBooleanUtils.AddListItems(IsCreateChannelIfChannelChanged, "生成", "不生成");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateChannelIfChannelChanged, PublishmentSystemInfo.Additional.IsCreateChannelIfChannelChanged.ToString());

                EBooleanUtils.AddListItems(IsCreateShowPageInfo, "显示", "不显示");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateShowPageInfo, PublishmentSystemInfo.Additional.IsCreateShowPageInfo.ToString());

                EBooleanUtils.AddListItems(IsCreateIE8Compatible, "强制兼容", "不设置");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateIE8Compatible, PublishmentSystemInfo.Additional.IsCreateIe8Compatible.ToString());

                EBooleanUtils.AddListItems(IsCreateBrowserNoCache, "强制清除缓存", "不设置");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateBrowserNoCache, PublishmentSystemInfo.Additional.IsCreateBrowserNoCache.ToString());

                EBooleanUtils.AddListItems(IsCreateJsIgnoreError, "包含JS容错代码", "不设置");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateJsIgnoreError, PublishmentSystemInfo.Additional.IsCreateJsIgnoreError.ToString());

                EBooleanUtils.AddListItems(IsCreateSearchDuplicate, "包含", "不包含");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateSearchDuplicate, PublishmentSystemInfo.Additional.IsCreateSearchDuplicate.ToString());

                EBooleanUtils.AddListItems(IsCreateWithJQuery, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateWithJQuery, PublishmentSystemInfo.Additional.IsCreateWithJQuery.ToString());

                EBooleanUtils.AddListItems(IsCreateIncludeToSSI, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateIncludeToSSI, PublishmentSystemInfo.Additional.IsCreateIncludeToSsi.ToString());

                EBooleanUtils.AddListItems(IsCreateDoubleClick, "启用双击生成", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateDoubleClick, PublishmentSystemInfo.Additional.IsCreateDoubleClick.ToString());

                tbCreateStaticMaxPage.Text = PublishmentSystemInfo.Additional.CreateStaticMaxPage.ToString();

                EBooleanUtils.AddListItems(IsCreateStaticContentByAddDate, "启用", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateStaticContentByAddDate, PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate.ToString());
                phIsCreateStaticContentByAddDate.Visible = PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate;
                if (PublishmentSystemInfo.Additional.CreateStaticContentAddDate != DateTime.MinValue)
                {
                    tbCreateStaticContentAddDate.DateTime = PublishmentSystemInfo.Additional.CreateStaticContentAddDate;
                }
            }
		}

        public void IsCreateStaticContentByAddDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            phIsCreateStaticContentByAddDate.Visible = TranslateUtils.ToBool(IsCreateStaticContentByAddDate.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.IsCreateContentIfContentChanged = TranslateUtils.ToBool(IsCreateContentIfContentChanged.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateChannelIfChannelChanged = TranslateUtils.ToBool(IsCreateChannelIfChannelChanged.SelectedValue);

                PublishmentSystemInfo.Additional.IsCreateShowPageInfo = TranslateUtils.ToBool(IsCreateShowPageInfo.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateIe8Compatible = TranslateUtils.ToBool(IsCreateIE8Compatible.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateBrowserNoCache = TranslateUtils.ToBool(IsCreateBrowserNoCache.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateJsIgnoreError = TranslateUtils.ToBool(IsCreateJsIgnoreError.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateSearchDuplicate = TranslateUtils.ToBool(IsCreateSearchDuplicate.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateIncludeToSsi = TranslateUtils.ToBool(IsCreateIncludeToSSI.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateWithJQuery = TranslateUtils.ToBool(IsCreateWithJQuery.SelectedValue);

                PublishmentSystemInfo.Additional.IsCreateDoubleClick = TranslateUtils.ToBool(IsCreateDoubleClick.SelectedValue);
                PublishmentSystemInfo.Additional.CreateStaticMaxPage = TranslateUtils.ToInt(tbCreateStaticMaxPage.Text);

                PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate = TranslateUtils.ToBool(IsCreateStaticContentByAddDate.SelectedValue);
                if (PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate)
                {
                    PublishmentSystemInfo.Additional.CreateStaticContentAddDate = tbCreateStaticContentAddDate.DateTime;
                }

                try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改页面生成设置");

                    SuccessMessage("页面生成设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "页面生成设置修改失败！");
				}
			}
		}
	}
}

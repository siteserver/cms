using System;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageCache : BasePage
    {
        public int CacheCount;
        public long CacheSize;
        public string CachePercentStr;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Utility, "系统缓存", AppManager.Settings.Permission.SettingsUtility);

                CacheCount = CacheUtils.GetCacheCount() + DbCacheManager.GetCount();
                CacheSize = 100 - CacheUtils.GetCacheEnabledPercent();
                CachePercentStr = $@"<div style=""width:230px;"" class=""progress progress-success progress-striped"">
            <div class=""bar"" style=""width: {100 - CacheUtils.GetCacheEnabledPercent()}%""></div><span>&nbsp;{CacheSize +"%"}</span>
          </div>";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                CacheUtils.Clear();
                DbCacheManager.Clear();
                PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageCache), null));
            }
        }

    }
}

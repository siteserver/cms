using System;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageCache : BasePage
    {
        public Literal LtlCount;
        public Literal LtlPercentage;
        public Repeater RptContents;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings("系统缓存", AppManager.Permissions.Settings.Utility);

                var count = CacheUtils.Count;
                var percentage = CacheUtils.EffectivePercentagePhysicalMemoryLimit;

                LtlCount.Text = count.ToString();
                LtlPercentage.Text = $@"
<div style=""width:230px;"" class=""progress progress-success progress-striped"">
    <div class=""bar"" style=""width: {100 - percentage}%""></div><span>&nbsp;{100 - percentage + "%"}</span>
</div>";

                RptContents.DataSource = CacheUtils.AllKeys;
                RptContents.ItemDataBound += RptContents_ItemDataBound;
                RptContents.DataBind();
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var key = (string) e.Item.DataItem;
                var value = CacheUtils.Get(key);

                if (value == null) return;

                var ltlKey = (Literal)e.Item.FindControl("ltlKey");
                var ltlValue = (Literal)e.Item.FindControl("ltlValue");

                ltlKey.Text = key;
                ltlValue.Text = value.GetType().FullName;
                if (ltlValue.Text == "System.String")
                {
                    ltlValue.Text += $"（{value.ToString().Length}）";
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                CacheUtils.ClearAll();
                CacheDbUtils.Clear();
                PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageCache), null));
            }
        }

    }
}

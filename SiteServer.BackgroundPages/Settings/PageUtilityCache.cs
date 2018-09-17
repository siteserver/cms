using System;
using System.Collections;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUtilityCache : BasePage
    {
        public Literal LtlCount;
        public Repeater RptContents;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Utility);

            LtlCount.Text = CacheUtils.Count.ToString();

            RptContents.DataSource = CacheUtils.AllKeys;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var key = (string) e.Item.DataItem;
            var value = CacheUtils.Get(key);

            var valueType = value?.GetType().FullName;
            if (valueType == null)
            {
                e.Item.Visible = false;
                return;
            }

            var ltlKey = (Literal)e.Item.FindControl("ltlKey");
            var ltlValue = (Literal)e.Item.FindControl("ltlValue");

            ltlKey.Text = key;
                
            if (valueType == "System.String")
            {
                ltlValue.Text = $"String, Length:{value.ToString().Length}";
            }
            else if (valueType == "System.Int32")
            {
                ltlValue.Text = value.ToString();
            }
            else if (valueType.StartsWith("System.Collections.Generic.List"))
            {
                ltlValue.Text = $"List, Count:{((ICollection)value).Count}";
            }
            else
            {
                ltlValue.Text = valueType;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            CacheUtils.ClearAll();
            CacheDbUtils.Clear();
            PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageUtilityCache), null));
        }

    }
}

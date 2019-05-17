using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Core
{
    public static partial class ControlUtils
    {
        public static class TableRuleUI
        {
            public static ListItem GetListItem(ETableRule type, bool selected)
            {
                var item = new ListItem(ETableRuleUtils.GetText(type), ETableRuleUtils.GetValue(type));
                if (selected)
                {
                    item.Selected = true;
                }
                return item;
            }
        }
    }
}
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
        public static class RelatedFieldStyleUI
        {

            public static ListItem GetListItem(ERelatedFieldStyle type, bool selected)
            {
                var item = new ListItem(ERelatedFieldStyleUtils.GetText(type), ERelatedFieldStyleUtils.GetValue(type));
                if (selected)
                {
                    item.Selected = true;
                }
                return item;
            }

            public static void AddListItems(ListControl listControl)
            {
                if (listControl != null)
                {
                    listControl.Items.Add(GetListItem(ERelatedFieldStyle.Horizontal, false));
                    listControl.Items.Add(GetListItem(ERelatedFieldStyle.Virtical, false));
                }
            }
        }
    }
}
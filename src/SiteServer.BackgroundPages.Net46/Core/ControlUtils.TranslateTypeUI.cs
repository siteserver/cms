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
        public static class TranslateTypeUI
        {
            public static ListItem GetListItem(ETranslateType type, bool selected)
            {
                var item = new ListItem(ETranslateTypeUtils.GetText(type), ETranslateTypeUtils.GetValue(type));
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
                    listControl.Items.Add(GetListItem(ETranslateType.Content, false));
                    listControl.Items.Add(GetListItem(ETranslateType.Channel, false));
                    listControl.Items.Add(GetListItem(ETranslateType.All, false));
                }
            }
        }
    }
}
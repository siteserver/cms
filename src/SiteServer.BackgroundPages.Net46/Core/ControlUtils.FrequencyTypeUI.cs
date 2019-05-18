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
        public static class FrequencyTypeUI
        {

            public static ListItem GetListItem(EFrequencyType type, bool selected)
            {
                var item = new ListItem(EFrequencyTypeUtils.GetText(type), EFrequencyTypeUtils.GetValue(type));
                if (selected)
                {
                    item.Selected = true;
                }
                return item;
            }

            public static void AddListItems(ListControl listControl, bool withJustInTime)
            {
                if (listControl != null)
                {
                    listControl.Items.Add(GetListItem(EFrequencyType.Month, false));
                    listControl.Items.Add(GetListItem(EFrequencyType.Week, false));
                    listControl.Items.Add(GetListItem(EFrequencyType.Day, false));
                    listControl.Items.Add(GetListItem(EFrequencyType.Hour, false));
                    listControl.Items.Add(GetListItem(EFrequencyType.Period, false));
                    //listControl.Items.Add(GetListItem(EFrequencyType.OnlyOnce, false));
                    if (withJustInTime)
                    {
                        listControl.Items.Add(GetListItem(EFrequencyType.JustInTime, false));
                    }
                }
            }
        }
    }
}
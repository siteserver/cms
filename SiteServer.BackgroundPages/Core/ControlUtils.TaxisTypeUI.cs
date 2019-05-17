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
        public static class TaxisTypeUI
        {
            public static ListItem GetListItem(ETaxisType type, bool selected)
            {
                var item = new ListItem(ETaxisTypeUtils.GetText(type), ETaxisTypeUtils.GetValue(type));
                if (selected)
                {
                    item.Selected = true;
                }
                return item;
            }

            public static void AddListItems(ListControl listControl)
            {
                if (listControl == null) return;

                listControl.Items.Add(GetListItem(ETaxisType.OrderById, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByIdDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByChannelId, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByChannelIdDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDate, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDateDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDate, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDateDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxis, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxisDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByHits, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByHitsByDay, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByHitsByWeek, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByHitsByMonth, false));
            }

            public static void AddListItemsForChannelEdit(ListControl listControl)
            {
                if (listControl == null) return;

                listControl.Items.Add(GetListItem(ETaxisType.OrderById, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByIdDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDate, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDateDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDate, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDateDesc, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxis, false));
                listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxisDesc, false));
            }
        }
    }
}
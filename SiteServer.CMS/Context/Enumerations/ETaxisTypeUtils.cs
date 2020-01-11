using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto;

namespace SiteServer.CMS.Context.Enumerations
{
    public static class ETaxisTypeUtilsExtensions
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

        public static List<Select<string>> GetAllForChannel()
        {
            return new List<Select<string>>
            {
                new Select<string>(ETaxisType.OrderByTaxisDesc),
                new Select<string>(ETaxisType.OrderByTaxis),
                new Select<string>(ETaxisType.OrderByAddDateDesc),
                new Select<string>(ETaxisType.OrderByAddDate)
            };
        }
    }
}

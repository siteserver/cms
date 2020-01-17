using System.Collections.Generic;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto;

namespace SiteServer.CMS.Context.Enumerations
{
    public static class ETaxisTypeUtilsExtensions
	{
		public static ListItem GetListItem(TaxisType type, bool selected)
		{
			var item = new ListItem(type.GetDisplayName(), type.GetValue());
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

        public static void AddListItemsForChannelEdit(ListControl listControl)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(TaxisType.OrderById, false));
            listControl.Items.Add(GetListItem(TaxisType.OrderByIdDesc, false));
            listControl.Items.Add(GetListItem(TaxisType.OrderByAddDate, false));
            listControl.Items.Add(GetListItem(TaxisType.OrderByAddDateDesc, false));
            listControl.Items.Add(GetListItem(TaxisType.OrderByLastEditDate, false));
            listControl.Items.Add(GetListItem(TaxisType.OrderByLastEditDateDesc, false));
            listControl.Items.Add(GetListItem(TaxisType.OrderByTaxis, false));
            listControl.Items.Add(GetListItem(TaxisType.OrderByTaxisDesc, false));
        }

        public static List<Select<string>> GetAllForChannel()
        {
            return new List<Select<string>>
            {
                new Select<string>(TaxisType.OrderByTaxisDesc),
                new Select<string>(TaxisType.OrderByTaxis),
                new Select<string>(TaxisType.OrderByAddDateDesc),
                new Select<string>(TaxisType.OrderByAddDate)
            };
        }
    }
}

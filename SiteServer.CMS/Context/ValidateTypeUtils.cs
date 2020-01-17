using System;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context
{
	public static class ValidateTypeUtils
	{
		public static ListItem GetListItem(ValidateType type, bool selected)
		{
			var item = new ListItem(type.GetDisplayName(), type.GetValue());
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
                listControl.Items.Add(GetListItem(ValidateType.None, false));
                listControl.Items.Add(GetListItem(ValidateType.Chinese, false));
                listControl.Items.Add(GetListItem(ValidateType.Email, false));
                listControl.Items.Add(GetListItem(ValidateType.Url, false));
                listControl.Items.Add(GetListItem(ValidateType.Mobile, false));
                listControl.Items.Add(GetListItem(ValidateType.Currency, false));
                listControl.Items.Add(GetListItem(ValidateType.Zip, false));
                listControl.Items.Add(GetListItem(ValidateType.IdCard, false));
            }
        }
	}
}

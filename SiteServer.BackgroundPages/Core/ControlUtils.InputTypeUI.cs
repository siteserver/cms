using System.Web.UI.WebControls;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Core
{
    public static partial class ControlUtils
    {
        public static class InputTypeUI
        {
            public static ListItem GetListItem(InputType type, bool selected)
            {
                var item = new ListItem(InputTypeUtils.GetText(type), type.Value);
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
                    listControl.Items.Add(GetListItem(InputType.Text, false));
                    listControl.Items.Add(GetListItem(InputType.TextArea, false));
                    listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                    listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                    listControl.Items.Add(GetListItem(InputType.Radio, false));
                    listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                    listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                    listControl.Items.Add(GetListItem(InputType.SelectCascading, false));
                    listControl.Items.Add(GetListItem(InputType.Date, false));
                    listControl.Items.Add(GetListItem(InputType.DateTime, false));
                    listControl.Items.Add(GetListItem(InputType.Image, false));
                    listControl.Items.Add(GetListItem(InputType.Video, false));
                    listControl.Items.Add(GetListItem(InputType.File, false));
                    listControl.Items.Add(GetListItem(InputType.Customize, false));
                    listControl.Items.Add(GetListItem(InputType.Hidden, false));
                }
            }

            public static void AddListItems(ListControl listControl, DataType dataType)
            {
                if (listControl != null)
                {
                    listControl.Items.Add(GetListItem(InputType.Text, false));
                    listControl.Items.Add(GetListItem(InputType.TextArea, false));
                    if (dataType == DataType.Text)
                    {
                        listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                    }
                    listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                    listControl.Items.Add(GetListItem(InputType.Radio, false));
                    listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                    listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                    listControl.Items.Add(GetListItem(InputType.SelectCascading, false));
                    listControl.Items.Add(GetListItem(InputType.Date, false));
                    listControl.Items.Add(GetListItem(InputType.DateTime, false));
                    listControl.Items.Add(GetListItem(InputType.Image, false));
                    listControl.Items.Add(GetListItem(InputType.Video, false));
                    listControl.Items.Add(GetListItem(InputType.File, false));
                    listControl.Items.Add(GetListItem(InputType.Customize, false));
                    listControl.Items.Add(GetListItem(InputType.Hidden, false));
                }
            }

            public static void AddListItemsToText(ListControl listControl)
            {
                if (listControl != null)
                {
                    listControl.Items.Add(GetListItem(InputType.Text, false));
                    listControl.Items.Add(GetListItem(InputType.TextArea, false));
                    listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                }
            }
        }
    }
}

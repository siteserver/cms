using System.Web.UI.WebControls;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Core
{
    public static partial class ControlUtils
    {
        public static class TemplateTypeUI
        {
            public static ListItem GetListItem(TemplateType type, bool selected)
            {
                var item = new ListItem(TemplateTypeUtils.GetText(type), type.Value);
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
                    listControl.Items.Add(GetListItem(TemplateType.IndexPageTemplate, false));
                    listControl.Items.Add(GetListItem(TemplateType.ChannelTemplate, false));
                    listControl.Items.Add(GetListItem(TemplateType.ContentTemplate, false));
                    listControl.Items.Add(GetListItem(TemplateType.FileTemplate, false));
                }
            }
        }
    }
}
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
        public static class LinkTypeUI
        {
            public static ListItem GetListItem(ELinkType type, bool selected)
            {
                var item = new ListItem(ELinkTypeUtils.GetText(type), ELinkTypeUtils.GetValue(type));
                if (selected)
                {
                    item.Selected = true;
                }
                return item;
            }

            public static void AddListItems(ListControl listControl)
            {
                if (listControl == null) return;

                listControl.Items.Add(GetListItem(ELinkType.None, false));
                listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExists, false));
                listControl.Items.Add(GetListItem(ELinkType.LinkToOnlyOneContent, false));
                listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent, false));
                listControl.Items.Add(GetListItem(ELinkType.LinkToFirstContent, false));
                listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent, false));
                listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExists, false));
                listControl.Items.Add(GetListItem(ELinkType.LinkToLastAddChannel, false));
                listControl.Items.Add(GetListItem(ELinkType.LinkToFirstChannel, false));
                listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel, false));
                listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel, false));
                listControl.Items.Add(GetListItem(ELinkType.NoLink, false));
            }
        }
    }

}
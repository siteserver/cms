using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Tabs;
using SiteServer.CMS.Plugins;

namespace SiteServer.BackgroundPages.Core
{

	/// <summary>
	/// Renders a Tab + Submenus based on the tab configuration file.
	/// </summary>
	public abstract class TabDrivenTemplatedWebControl : Control
	{
        /// <summary>
        /// Returns the currently selected tab
        /// </summary>
        public virtual string Selected
        {
            get
            {
                return (string)ViewState["Selected"];
            }
            set
            {
                ViewState["Selected"] = value;
            }
        }

        /// <summary>
        /// returns the file name containing the tab configuration
        /// </summary>
        public virtual string FileName
        {
            get
            {
                var state = ViewState["FileName"];
                return (string) state;
                //return "tabs.config";
            }
            set
            {
                ViewState["FileName"] = value;
            }
        }

	    public virtual List<string> PermissionList { get; set; }

        /// <summary>
        /// Returns the current instance of the TabCollection
        /// </summary>
        /// <returns></returns>
        protected TabCollection GetTabs()
        {
            var tabCollection = new TabCollection();
            if (!string.IsNullOrEmpty(FileName))
            {
                var path = Context.Server.MapPath(ResolveUrl(FileName));
                tabCollection = TabCollection.GetTabs(path);

                var pluginInfoList = PluginManager.GetPluginInfoList();
                foreach (var pluginInfo in pluginInfoList)
                {
                    if (string.IsNullOrEmpty(pluginInfo.MenusPath)) continue;

                    var pluginTabCollection = TabCollection.GetTabsByPluginDirectory(pluginInfo.MenusPath, pluginInfo.DirectoryName);
                    var i = 0;
                    foreach (var pluginTab in pluginTabCollection.Tabs)
                    {
                        foreach (var tab in tabCollection.Tabs)
                        {
                            if (tab.Id == pluginTab.ParentId)
                            {
                                var isExists = false;
                                foreach (var childTab in tab.Children)
                                {
                                    if (childTab.Id == pluginTab.Id)
                                    {
                                        isExists = true;
                                    }
                                }

                                if (!isExists)
                                {
                                    var list = new List<Tab>();
                                    if (tab.Children != null)
                                    {
                                        list = tab.Children.ToList();
                                    }
                                    list.Add(pluginTab);
                                    tab.Children = list.ToArray();
                                }
                            }
                        }
                    }
                }
            }
            return tabCollection;
        }

        /// <summary>
        /// Resolves the current url and attempts to append the specified querystring
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string FormatLink(Tab t)
        {
            if (!t.HasHref)
                return null;

            var url = t.KeepQueryString ? PageUtils.AddQueryString(t.Href, Context.Request.QueryString) : t.Href;

            return url;

            //return ResolveUrl(url);
        }

        protected virtual string GetText(Tab t)
        {
            return t.Text;
        }

        /// <summary>
        /// Walks the tab and it's children to see if any of them are currently selected
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected SelectedState GetState(Tab t)
        {
            //Check the parent
            if (string.Compare(t.Name, Selected, true, CultureInfo.InvariantCulture) == 0)
                return SelectedState.Selected;

            //Walk each of the child tabs
            if (t.HasChildren)
            {
                foreach (var child in t.Children)
                {
                    if (string.Compare(child.Name, Selected, true, CultureInfo.InvariantCulture) == 0)
                        return SelectedState.ChildSelected;

                    else if (child.HasChildren)
                    {
                        foreach (var cc in child.Children)
                            if (string.Compare(cc.Name, Selected, true, CultureInfo.InvariantCulture) == 0)
                                return SelectedState.ChildSelected;
                    }
                }
            }

            //Nothing here is selected
            return SelectedState.Not;
        }

        /// <summary>
        /// Internal enum used to track if a tab is selected
        /// </summary>
        protected enum SelectedState
        {
            Not,
            Selected,
            ChildSelected
        }
    }
}

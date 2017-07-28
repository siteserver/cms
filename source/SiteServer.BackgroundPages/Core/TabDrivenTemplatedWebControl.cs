using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Tabs;

namespace SiteServer.BackgroundPages.Core
{

	/// <summary>
	/// Renders a Tab + Submenus based on the tab configuration file.
	/// </summary>
	public abstract class TabDrivenTemplatedWebControl : Control
	{
		#region Public Properties

		/// <summary>
		/// Returns the currently selected tab
		/// </summary>
		public virtual string Selected
		{
			get 
			{
				return (string) ViewState["Selected"]; 
			}
			set 
			{  
				ViewState["Selected"] = value; 
			}
		}
        
		/// <summary>
		/// returns the location of the current tab configuration file
		/// </summary>
		public virtual string FileLocation
		{
			get
			{
				var path = Context.Server.MapPath(ResolveUrl(FileName));
				return path;
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
				if ( state != null ) 
				{
					return (String)state;
				}
                return null;
				//return "tabs.config";
			}
			set 
			{
				ViewState["FileName"] = value;
			}
		}

        TabCollection tabs;
        public virtual TabCollection Tabs
        {
            get
            {
                return tabs;
            }
            set
            {
                tabs = value;
            }
        }

        List<string> permissionList;
		public virtual List<string> PermissionList
		{
			get 
			{
				return permissionList;
			}
			set 
			{
                permissionList = value;
			}
		}

		/// <summary>
		/// if true, the url is written into the control label
		/// </summary>
		public virtual bool UseDirectNavigation
		{
			get 
			{
				var state = ViewState["UseDirectNavigation"];
				if ( state != null ) 
				{
					return (bool)state;
				}
				return false;
			}
			set 
			{
				ViewState["UseDirectNavigation"] = value;
			}
		}
		#endregion

		#region GetTabs()
		/// <summary>
		/// Returns the current instance of the TabCollection
		/// </summary>
		/// <returns></returns>
		protected TabCollection GetTabs()
		{
            if (tabs != null)
            {
                return tabs;
            }
            else
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    var path = FileLocation;
                    return TabCollection.GetTabs(path);
                }
                return new TabCollection();
            }
		}
		#endregion

		#region Tab Helpers

		/// <summary>
		/// Resolves the current url and attempts to append the specified querystring
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		protected string FormatLink(Tab t)
		{
			string url = null;

			if(!t.HasHref)
				return null;

			if(t.KeepQueryString)
			{
				url = PageUtils.AddQueryString(t.Href, Context.Request.QueryString);
			}
			else
			{
				url = t.Href;
			}

		    return url;

			//return ResolveUrl(url);
		}

		protected virtual string GetText(Tab t)
		{
			return t.Text;
		}
		#endregion

		#region GetState
		/// <summary>
		/// Walks the tab and it's children to see if any of them are currently selected
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		protected SelectedState GetState(Tab t)
		{
			//Check the parent
			if(string.Compare(t.Name,Selected,true,CultureInfo.InvariantCulture) == 0)
				return SelectedState.Selected;

			//Walk each of the child tabs
			if(t.HasChildren)
			{
				foreach(var child in t.Children)
				{
					if(string.Compare(child.Name,Selected,true,CultureInfo.InvariantCulture) == 0)
						return SelectedState.ChildSelected;

					else if(child.HasChildren)
					{
						foreach(var cc in child.Children)
							if(string.Compare(cc.Name,Selected,true,CultureInfo.InvariantCulture) == 0)
								return SelectedState.ChildSelected;
					}
				}
			}

			//Nothing here is selected
			return SelectedState.Not;
		}

		#endregion

		#region SelectedState
		/// <summary>
		/// Internal enum used to track if a tab is selected
		/// </summary>
		protected enum SelectedState
		{
			Not,
			Selected,
			ChildSelected
		};
		#endregion

	}
}

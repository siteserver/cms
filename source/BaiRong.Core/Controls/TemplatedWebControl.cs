using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Web.Controls
{
	/// <summary>
	/// The base class for ALL community server controls that must load their UI either through an external skin
	/// or via an in-page template.
	/// </summary>
	[
	ParseChildren( true ),
	PersistChildren( false ),
	]
	public abstract class TemplatedWebControl : WebControl, INamingContainer 
	{

		#region Composite Controls

		/// <exclude/>
		public override ControlCollection Controls 
		{
			get 
			{
				EnsureChildControls();
				return base.Controls;
			}
		}

		/// <exclude/>
		public override void DataBind() 
		{
			EnsureChildControls();
		}

		#endregion

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
		}
		   
		/// <summary>
		/// No End Span
		/// </summary>
		/// <param name="writer"></param>
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			//we don't need a span tag
		}

		protected virtual string ControlText()
		{
			return null;
		}

		public override Page Page
		{
			get
			{
				//There is at least one control which causes Page to be null
				//this check should allow us to safely populate the page value
                
				//ReWritten for aspnet 2 support
				//if(base.Page == null && this.Context != null)
				//{
				//    base.Page = this.Context.Handler as System.Web.UI.Page;
				//}
				if(base.Page == null)
				{
					base.Page = HttpContext.Current.Handler as Page;
				}

				return base.Page;
			}
			set
			{
				base.Page = value;
			}
		}


		/// <exclude/>
		public override Control FindControl( string id ) 
		{
			var ctrl = base.FindControl( id );
			if ( ctrl == null && Controls.Count == 1 ) 
			{
				ctrl = Controls[ 0 ].FindControl( id );
			}
			return ctrl;
		}


		/// <summary>
		/// First choice for skins. The value of Control() text will be interpreted as a skin. The 
		/// primary usage of this feature will be to serve skins from the database
		/// </summary>
		/// <returns></returns>
		protected virtual bool LoadTextBasedControl()
		{
			var text = ControlText();

			if(!string.IsNullOrEmpty(text))
			{
				var skin = Page.ParseControl(text);
				skin.ID = "_";
				Controls.Add(skin);
				return true;
			}

			return false;
		}
		
		/// <summary>
		/// Override this method to attach templated or external skin controls to local references.
		/// </summary>
		/// <remarks>
		/// This will only be called if the non-default skin is used.
		/// </remarks>
		protected abstract void AttachChildControls();

		/// <summary>
		/// Identifies the control that fired posback.
		/// </summary>
		public Control GetPostBackControl ()
		{
			return GetPostBackControl( this );
		}

		public Control GetPostBackControl (Control container)
		{
			// Note: this is an adapted eggheadcafe.com code.
			//
			Control control = null;

			var ctrlname = Page.Request.Params["__EVENTTARGET"];
			if (ctrlname != null && ctrlname != string.Empty)
			{
				var tokens = ctrlname.Split( new char[1] { ':' } );
				if (tokens != null && tokens.GetLength( 0 ) > 0)
					ctrlname = tokens[(tokens.GetLength( 0 ) - 1)];

				control = FindControl( ctrlname );
			}
			else
			{
				// If __EVENTTARGET is null, control is a button type and need to 
				// iterate over the form collection to find it
				//
				var ctrlStr = string.Empty;
				Control c = null;
				foreach (string ctl in Page.Request.Form)
				{

					// Handle ImageButton controls
					if (ctl.EndsWith( ".x" ) || ctl.EndsWith( ".y" ))
					{
						ctrlStr = ctl.Substring( 0, (ctl.Length - 2) );
						c = FindControl( ctrlStr );
					}
					else
					{
						c = FindControl( ctl );
					}
					if (c is Button ||
						c is ImageButton)
					{
						control = c;
						break;
					}
				}
			}

			return control;
		}

	}
}

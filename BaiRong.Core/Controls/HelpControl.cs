using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace BaiRong.Core.Web.Controls
{

	public class HelpControl : System.Web.UI.UserControl
	{

		public LinkButton cmdHelp;
		public Image imgHelp;
		public Label lblLabel;
		public Panel pnlHelp;
		public Label lblHelp;
		public HtmlGenericControl label;

		private string _ControlName = string.Empty; //Associated Edit Control for this Label


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ControlName is the Id of the control that is associated with the label
		/// </summary>
		/// <value>A string representing the id of the associated control</value>
		/// <remarks>
		/// </remarks>
		/// -----------------------------------------------------------------------------
		public string ControlName
		{
			get
			{
				if (_ControlName != null)
				{
					return _ControlName;
				}
				else
				{
					return "";
				}
			}
			set
			{
				_ControlName = value;
			}
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// HelpText is value of the Help Text if no ResourceKey is provided
		/// </summary>
		/// <value>A string representing the Text</value>
		/// <remarks>
		/// </remarks>
		/// -----------------------------------------------------------------------------
		public string HelpText
		{
			get
			{
				return lblHelp.Text;
			}
			set
			{
				lblHelp.Text = value;
				imgHelp.AlternateText = value;
			}
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Text is value of the Label Text if no ResourceKey is provided
		/// </summary>
		/// <value>A string representing the Text</value>
		/// <remarks>
		/// </remarks>
		/// -----------------------------------------------------------------------------
		public string Text
		{
			get
			{
				return lblLabel.Text;
			}
			set
			{
				lblLabel.Text = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				cmdHelp.Attributes.Add("onclick", "if (__Help_OnClick(\'" + pnlHelp.ClientID + "\')) return false;");
				pnlHelp.Style.Add("display", "none");

				//find the reference control in the parents Controls collection
				var c = Parent.FindControl(ControlName);
				if (c != null)
				{
					label.Attributes["for"] = c.ClientID;
				}

				if (Text == null || Text.Length == 0)
				{
					lblLabel.Visible = false;
				}

			}
			catch
			{
			}
		}
	}
}

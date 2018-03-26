using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.StlParser.Utility
{
	public class SeparatorTemplate : ITemplate
	{
	    readonly string separator = string.Empty;

		public SeparatorTemplate(string separator)
		{
			this.separator = separator;
		}

		public void InstantiateIn(Control container)
		{
			var noTagText = new Literal();
			noTagText.DataBinding += TemplateControl_DataBinding;
			container.Controls.Add(noTagText);
		}

		private void TemplateControl_DataBinding(object sender, EventArgs e)
		{
			var noTagText = (Literal) sender;
			noTagText.Text = separator;
		}
	}
}

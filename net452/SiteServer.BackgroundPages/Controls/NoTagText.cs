using System.Web.UI;

namespace SiteServer.BackgroundPages.Controls
{
	public class NoTagText : Control
	{
		public virtual string Text 
		{
			get 
			{
				var state = ViewState["Text"];
				if ( state != null ) 
				{
					return (string)state;
				}
				return string.Empty;
			}
			set 
			{
				ViewState["Text"] = value;
			}
		}

        public virtual string NoWrap
        {
            get
            {
                var state = ViewState["NoWrap"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["NoWrap"] = value;
            }
        }

		protected override void Render(HtmlTextWriter writer)
		{
            if (!string.IsNullOrEmpty(NoWrap))
            {
                writer.Write("<nobr>");
            }
			writer.Write(Text);
            if (!string.IsNullOrEmpty(NoWrap))
            {
                writer.Write("</nobr>");
            }
		}
	}

}

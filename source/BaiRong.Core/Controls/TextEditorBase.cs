using System.Web.UI;

namespace BaiRong.Core.Web.Controls
{
	public abstract class TextEditorBase : Control 
	{
		public virtual string Text
		{
			get
			{
				var state = ViewState["Text"];
				if (state != null)
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

		public virtual string Width
		{
			get
			{
				var state = ViewState["Width"];
				if (state != null)
				{
					return (string)state;
				}
				return "0";
			}
			set
			{
				ViewState["Width"] = value;
			}
		}

		public virtual string Height
		{
			get
			{
				var state = ViewState["Height"];
				if (state != null)
				{
					return (string)state;
				}
				return "0";
			}
			set
			{
				ViewState["Height"] = value;
			}
		}
	}
}

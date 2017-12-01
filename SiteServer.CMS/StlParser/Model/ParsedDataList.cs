using System.Collections.Specialized;
using System.Drawing;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Model
{
	/// <summary>
	/// 模板支持的属性：
	/// AccessKey
	/// BackColor
	/// BorderColor
	/// BorderWidth
	/// CellPadding
	/// CellSpacing
	/// CssClass
	/// Enabled
	/// ForeColor
	/// GridLines
	/// HorizontalAlign
	/// RepeatColumns
	/// RepeatDirection
	/// RepeatLayout
	/// ToolTip
	/// Visible
	/// </summary>
	public class ParsedDataList : DataList
	{
		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add("cellpadding", "填充");
				attributes.Add("cellspacing", "间距");
				attributes.Add("class", "类");
				attributes.Add("columns", "列数");
				attributes.Add("direction", "方向");
				attributes.Add("layout", "指定流或表格式");
				return attributes;
			}
		}

		public StringDictionary stringDictionary = new StringDictionary();
		
		private bool IsExists(string attributeName)
		{
			if (stringDictionary[attributeName] != null)
			{
				return true;
			}
			return false;
		}

		public override string AccessKey
		{
			get{ return base.AccessKey; }
			set
			{
				if (!IsExists("AccessKey"))
				{
					stringDictionary["AccessKey"] = string.Empty;
					base.AccessKey = value;
				}
			}
		}

		public override Color BackColor
		{
			get{ return base.BackColor; }
			set
			{
				if (!IsExists("BackColor"))
				{
					stringDictionary["BackColor"] = string.Empty;
					base.BackColor = value;
				}
			}
		}

		public override Color BorderColor
		{
			get{ return base.BorderColor; }
			set
			{
				if (!IsExists("BorderColor"))
				{
					stringDictionary["BorderColor"] = string.Empty;
					base.BorderColor = value;
				}
			}
		}

		public override Unit BorderWidth
		{
			get{ return base.BorderWidth; }
			set
			{
				if (!IsExists("BorderWidth"))
				{
					stringDictionary["BorderWidth"] = string.Empty;
					base.BorderWidth = value;
				}
			}
		}

		public override int CellPadding
		{
			get{ return base.CellPadding; }
			set
			{
				if (!IsExists("CellPadding"))
				{
					stringDictionary["CellPadding"] = string.Empty;
					base.CellPadding = value;
				}
			}
		}

		public override int CellSpacing
		{
			get{ return base.CellSpacing; }
			set
			{
				if (!IsExists("CellSpacing"))
				{
					stringDictionary["CellSpacing"] = string.Empty;
					base.CellSpacing = value;
				}
			}
		}

		public override string CssClass
		{
			get{ return base.CssClass; }
			set
			{
				if (!IsExists("CssClass"))
				{
					stringDictionary["CssClass"] = string.Empty;
					base.CssClass = value;
				}
			}
		}

		public override bool Enabled
		{
			get{ return base.Enabled; }
			set
			{
				if (!IsExists("Enabled"))
				{
					stringDictionary["Enabled"] = string.Empty;
					base.Enabled = value;
				}
			}
		}

		public override Color ForeColor
		{
			get{ return base.ForeColor; }
			set
			{
				if (!IsExists("ForeColor"))
				{
					stringDictionary["ForeColor"] = string.Empty;
					base.ForeColor = value;
				}
			}
		}

		public override GridLines GridLines
		{
			get{ return base.GridLines; }
			set
			{
				if (!IsExists("GridLines"))
				{
					stringDictionary["GridLines"] = string.Empty;
					base.GridLines = value;
				}
			}
		}

		public override HorizontalAlign HorizontalAlign
		{
			get{ return base.HorizontalAlign; }
			set
			{
				if (!IsExists("HorizontalAlign"))
				{
					stringDictionary["HorizontalAlign"] = string.Empty;
					base.HorizontalAlign = value;
				}
			}
		}

		public override int RepeatColumns
		{
			get{ return base.RepeatColumns; }
			set
			{
				if (!IsExists("RepeatColumns"))
				{
					stringDictionary["RepeatColumns"] = string.Empty;
					base.RepeatColumns = value;
				}
			}
		}

		public override RepeatDirection RepeatDirection
		{
			get{ return base.RepeatDirection; }
			set
			{
				if (!IsExists("RepeatDirection"))
				{
					stringDictionary["RepeatDirection"] = string.Empty;
					base.RepeatDirection = value;
				}
			}
		}

		public override RepeatLayout RepeatLayout
		{
			get{ return base.RepeatLayout; }
			set
			{
				if (!IsExists("RepeatLayout"))
				{
					stringDictionary["RepeatLayout"] = string.Empty;
					base.RepeatLayout = value;
				}
			}
		}

		public override string ToolTip
		{
			get{ return base.ToolTip; }
			set
			{
				if (!IsExists("ToolTip"))
				{
					stringDictionary["ToolTip"] = string.Empty;
					base.ToolTip = value;
				}
			}
		}

		public override bool Visible
		{
			get{ return base.Visible; }
			set
			{
				if (!IsExists("Visible"))
				{
					stringDictionary["Visible"] = string.Empty;
					base.Visible = value;
				}
			}
		}

		public void AddAttribute(string attributeName, string attributeValue)
		{
			if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue))
			{
				return;
			}
			attributeName = attributeName.Trim().ToLower();
			attributeValue = attributeValue.Trim().ToLower();
			if (attributeName.Equals("accesskey"))
			{
				AccessKey = attributeValue;
			}
			else if (attributeName.Equals("backcolor") || attributeName.Equals("bgcolor"))
			{
				BackColor = TranslateUtils.ToColor(attributeValue);
			}
			else if (attributeName.Equals("bordercolor"))
			{
				BorderColor = TranslateUtils.ToColor(attributeValue);
			}
			else if (attributeName.Equals("borderwidth") || attributeName.Equals("border"))
			{
				BorderWidth = TranslateUtils.ToUnit(attributeValue);
			}
			else if (attributeName.Equals("cellpadding"))
			{
				CellPadding = TranslateUtils.ToInt(attributeValue);
			}
			else if (attributeName.Equals("cellspacing"))
			{
				CellSpacing = TranslateUtils.ToInt(attributeValue);
			}
			else if (attributeName.Equals("cssclass") || attributeName.Equals("class"))
			{
				CssClass = attributeValue;
			}
			else if (attributeName.Equals("enabled"))
			{
				Enabled = TranslateUtils.ToBool(attributeValue, true);
			}
			else if (attributeName.Equals("forecolor"))
			{
				ForeColor = TranslateUtils.ToColor(attributeValue);
			}
			else if (attributeName.Equals("gridlines"))
			{
				GridLines = Converter.ToGridLines(attributeValue);
			}
			else if (attributeName.Equals("horizontalalign"))
			{
				HorizontalAlign = Converter.ToHorizontalAlign(attributeValue);
			}
			else if (attributeName.Equals("repeatcolumns") || attributeName.Equals("columns"))
			{
				RepeatColumns = TranslateUtils.ToInt(attributeValue);
			}
			else if (attributeName.Equals("repeatdirection") || attributeName.Equals("direction"))
			{
				RepeatDirection = Converter.ToRepeatDirection(attributeValue);
			}
			else if (attributeName.Equals("repeatlayout"))
			{
				RepeatLayout = Converter.ToRepeatLayout(attributeValue);
			}
			else if (attributeName.Equals("tooltip"))
			{
				ToolTip = attributeValue;
			}
			else if (attributeName.Equals("visible"))
			{
				Visible = TranslateUtils.ToBool(attributeValue, true);
			}
			else
			{
				ControlUtils.AddAttribute(this, attributeName, attributeValue);
			}
		}
	}
}

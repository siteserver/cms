using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Controls
{
    public class DateTimeTextBox : TextBox
    {
        public bool Now
        {
            get
            {
                var o = ViewState["Now"];
                if (o == null)
                {
                    return false;
                }
                return (bool)o;
            }
            set { ViewState["Now"] = value; }
        }

        public bool ShowTime
        {
            get
            {
                var o = ViewState["ShowTime"];
                if (o == null)
                {
                    return false;
                }
                return (bool)o;
            }
            set { ViewState["ShowTime"] = value; }
        }

        public override string Text
        {
            get
            {
                var formatString = (ShowTime) ? DateUtils.FormatStringDateTime : DateUtils.FormatStringDateOnly;
                if (Now && string.IsNullOrEmpty(base.Text))
                {
                    base.Text = DateTime.Now.ToString(formatString);
                }
                if (!string.IsNullOrEmpty(base.Text))
                {
                    base.Text = TranslateUtils.ToDateTime(base.Text).ToString(formatString);
                }
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        public virtual DateTime DateTime
        {
            get
            {
                return TranslateUtils.ToDateTime(Text);
            }
            set
            {
                var formatString = (ShowTime) ? DateUtils.FormatStringDateTime : DateUtils.FormatStringDateOnly;
                Text = value.ToString(formatString);
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            var onfocus = (ShowTime) ? SiteServerAssets.DatePicker.OnFocus : SiteServerAssets.DatePicker.OnFocusDateOnly;
            Attributes.Add("onfocus", onfocus);
            base.AddAttributesToRender(writer);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Page != null)
            {
                if (!Page.ClientScript.IsStartupScriptRegistered("DateTimeTextBox_Calendar"))
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "DateTimeTextBox_Calendar",
                        $@"<script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.DatePicker.Js)}""></script>");
                }
            }
            base.OnLoad(e);
        }
    }
}

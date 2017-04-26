using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Controls
{
	public class AuxiliarySingleControl : Control
	{
        private NameValueCollection formCollection;
        private bool isEdit;
        private bool isPostBack;
        private PublishmentSystemInfo publishmentSystemInfo;
        private int nodeID;
        private ETableStyle tableStyle;
        private string tableName;
        private string attributeName;

        private List<int> relatedIdentities;

        public void SetParameters(PublishmentSystemInfo publishmentSystemInfo, int nodeID, ETableStyle tableStyle, string tableName, string attributeName, NameValueCollection formCollection, bool isEdit, bool isPostBack)
        {
            this.publishmentSystemInfo = publishmentSystemInfo;
            this.nodeID = nodeID;
            this.tableStyle = tableStyle;
            this.tableName = tableName;
            this.attributeName = attributeName;
            this.formCollection = formCollection;
            this.isEdit = isEdit;
            this.isPostBack = isPostBack;

            relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeID);
        }

		protected override void Render(HtmlTextWriter output)
		{
            if (formCollection == null)
            {
                if (HttpContext.Current.Request.Form != null && HttpContext.Current.Request.Form.Count > 0)
                {
                    formCollection = HttpContext.Current.Request.Form;
                }
                else
                {
                    formCollection = new NameValueCollection();
                }
            }

            var styleInfo = TableStyleManager.GetTableStyleInfo(tableStyle, tableName, attributeName, relatedIdentities);
            if (!string.IsNullOrEmpty(DefaultValue))
            {
                styleInfo.DefaultValue = DefaultValue;
            }

            if (styleInfo.IsVisible == false) return;

            var helpHtml = $"{styleInfo.DisplayName}：";
            var pageScripts = new NameValueCollection();

            if (string.IsNullOrEmpty(AdditionalAttributes))
            {
                AdditionalAttributes = InputParserUtils.GetAdditionalAttributes(string.Empty, EInputTypeUtils.GetEnumType(styleInfo.InputType));
            }

            styleInfo.Additional.IsValidate = TranslateUtils.ToBool(IsValidate);
            styleInfo.Additional.IsRequired = TranslateUtils.ToBool(IsRequire);

            var inputHtml = BackgroundInputTypeParser.Parse(publishmentSystemInfo, nodeID, styleInfo, tableStyle, attributeName, formCollection, isEdit, isPostBack, AdditionalAttributes, pageScripts, true);

            if (string.IsNullOrEmpty(FormatString))
            {
                if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.TextEditor))
                {
                    output.Write(@"
<tr><td colspan=""4"" align=""left"">{0}</td></tr>
<tr><td colspan=""4"" align=""left"">{1}</td></tr>
", helpHtml, inputHtml);
                }
                else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Image))
                {
                    output.Write(@"
<tr height=""80"" valign=""middle""><td>{0}</td><td colspan=""3"">{1}</td></tr>
", helpHtml, inputHtml);
                }
                else
                {
                    output.Write(@"
<tr><td>{0}</td><td colspan=""3"">{1}</td></tr>
", helpHtml, inputHtml);
                }
            }
            else
            {
                output.Write(FormatString, helpHtml, inputHtml);
            }

            foreach (string key in pageScripts.Keys)
            {
                output.Write(pageScripts[key]);
            }
		}

        public virtual string IsValidate
        {
            get
            {
                var state = ViewState["IsValidate"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["IsValidate"] = value;
            }
        }

        public virtual string IsRequire
        {
            get
            {
                var state = ViewState["IsRequire"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["IsRequire"] = value;
            }
        }

        public virtual string ErrorMessage
        {
            get
            {
                var state = ViewState["ErrorMessage"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["ErrorMessage"] = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                var state = ViewState["DefaultValue"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["DefaultValue"] = value;
            }
        }

        public string FormatString
        {
            get
            {
                var state = ViewState["FormatString"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["FormatString"] = value;
            }
        }

        public string AdditionalAttributes
        {
            get
            {
                var state = ViewState["AdditionalAttributes"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["AdditionalAttributes"] = value;
            }
        }
    }
}

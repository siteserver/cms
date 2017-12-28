using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Table;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Controls
{
	public class ChannelAuxiliaryControl : Control
	{
        private IAttributes _attributes;
	    private readonly Hashtable _inputTypeWithFormatStringHashtable = new Hashtable();

        public void SetParameters(IAttributes attributes)
        {
            _attributes = attributes;
        }

		protected override void Render(HtmlTextWriter output)
		{
            if (_attributes == null) return;

            var nodeId = int.Parse(HttpContext.Current.Request.QueryString["NodeID"]);
            var publishmentSystemId = int.Parse(HttpContext.Current.Request.QueryString["PublishmentSystemID"]);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, nodeId);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.NodeDao.TableName, relatedIdentities);

		    if (styleInfoList == null) return;

            var builder = new StringBuilder();
		    var pageScripts = new NameValueCollection();
		    foreach (var styleInfo in styleInfoList)
		    {
		        //string inputHtml = TableInputParser.Parse(styleInfo, styleInfo.AttributeName, this.formCollection, this.isEdit, isPostBack, attributes, pageScripts);
		        string extra;
		        var value = BackgroundInputTypeParser.Parse(publishmentSystemInfo, nodeId, styleInfo, _attributes, pageScripts, out extra);

		        if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

		        builder.AppendFormat(GetFormatString(InputTypeUtils.GetEnumType(styleInfo.InputType)), styleInfo.DisplayName, value, extra);
		    }

		    output.Write(builder.ToString());

		    foreach (string key in pageScripts.Keys)
		    {
		        output.Write(pageScripts[key]);
		    }
		}

        public string FormatTextEditor
        {
            get
            {
                var formatString = _inputTypeWithFormatStringHashtable[InputTypeUtils.GetValue(InputType.TextEditor)] as string;
                if (string.IsNullOrEmpty(formatString))
                {
                    formatString = @"
<tr><td height=""25"" colspan=""2"">{0}：</td></tr>
<tr><td height=""25"" colspan=""2"">{1}</td></tr>
";
                }
                return formatString;
            }
            set
            {
                _inputTypeWithFormatStringHashtable[InputTypeUtils.GetValue(InputType.TextEditor)] = value;
            }
        }

        public string FormatImage
        {
            get
            {
                var formatString = _inputTypeWithFormatStringHashtable[InputTypeUtils.GetValue(InputType.Image)] as string;
                if (string.IsNullOrEmpty(formatString))
                {
                    formatString = @"
<tr height=""80"" valign=""middle""><td>{0}：</td><td>{1}</td></tr>
";
                }
                return formatString;
            }
            set
            {
                _inputTypeWithFormatStringHashtable[InputTypeUtils.GetValue(InputType.Image)] = value;
            }
        }

        public string FormatDefault
        {
            get
            {
                var formatString = _inputTypeWithFormatStringHashtable[InputTypeUtils.GetValue(InputType.Text)] as string;
                if (string.IsNullOrEmpty(formatString))
                {
                    formatString = @"
<tr><td height=""25"">{0}：</td><td height=""25"">{1}</td></tr>
";
                }
                return formatString;
            }
            set
            {
                _inputTypeWithFormatStringHashtable[InputTypeUtils.GetValue(InputType.Text)] = value;
            }
        }

        private string _additionalAttributes;
        public string AdditionalAttributes
        {
            get
            {
                return _additionalAttributes;
            }
            set
            {
                _additionalAttributes = value;
            }
        }

        protected virtual string GetFormatString(InputType inputType)
        {
            string formatString;
            if (inputType == InputType.TextEditor)
            {
                formatString = FormatTextEditor;
            }
            else if (inputType == InputType.Image)
            {
                formatString = FormatImage;
            }
            else
            {
                formatString = FormatDefault;
            }
            return formatString;
        }

	}
}

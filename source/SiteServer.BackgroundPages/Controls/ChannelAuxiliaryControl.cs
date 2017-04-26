using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Controls
{
	// 通过栏目辅助表生成的栏目辅助属性控件
	public class ChannelAuxiliaryControl : Control
	{
        private NameValueCollection _formCollection;
        private bool _isEdit;
        private bool _isPostBack;
	    private readonly Hashtable _inputTypeWithFormatStringHashtable = new Hashtable();

        public void SetParameters(NameValueCollection formCollection, bool isEdit, bool isPostBack)
        {
            _formCollection = formCollection;
            _isEdit = isEdit;
            _isPostBack = isPostBack;
        }

		protected override void Render(HtmlTextWriter output)
		{
            var nodeId = int.Parse(HttpContext.Current.Request.QueryString["NodeID"]);
            var publishmentSystemId = int.Parse(HttpContext.Current.Request.QueryString["PublishmentSystemID"]);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            if (_formCollection == null)
            {
                if (HttpContext.Current.Request.Form.Count > 0)
                {
                    _formCollection = HttpContext.Current.Request.Form;
                }
                else
                {
                    _formCollection = new NameValueCollection();
                }
            }

            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, nodeId);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.Channel, DataProvider.NodeDao.TableName, relatedIdentities);

            if (styleInfoList != null)
            {
                var builder = new StringBuilder();
                var pageScripts = new NameValueCollection();
                foreach (var styleInfo in styleInfoList)
                {
                    if (styleInfo.IsVisible)
                    {
                        var attributes = InputParserUtils.GetAdditionalAttributes(string.Empty, EInputTypeUtils.GetEnumType(styleInfo.InputType));
                        //string inputHtml = TableInputParser.Parse(styleInfo, styleInfo.AttributeName, this.formCollection, this.isEdit, isPostBack, attributes, pageScripts);
                        var inputHtml = BackgroundInputTypeParser.Parse(publishmentSystemInfo, nodeId, styleInfo, ETableStyle.Channel, styleInfo.AttributeName, _formCollection, _isEdit, _isPostBack, attributes, pageScripts, true);

                        builder.AppendFormat(GetFormatString(EInputTypeUtils.GetEnumType(styleInfo.InputType)), styleInfo.DisplayName, inputHtml, styleInfo.HelpText);
                    }
                }

                output.Write(builder.ToString());

                foreach (string key in pageScripts.Keys)
                {
                    output.Write(pageScripts[key]);
                }
            }
		}

        public string FormatTextEditor
        {
            get
            {
                var formatString = _inputTypeWithFormatStringHashtable[EInputTypeUtils.GetValue(EInputType.TextEditor)] as string;
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
                _inputTypeWithFormatStringHashtable[EInputTypeUtils.GetValue(EInputType.TextEditor)] = value;
            }
        }

        public string FormatImage
        {
            get
            {
                var formatString = _inputTypeWithFormatStringHashtable[EInputTypeUtils.GetValue(EInputType.Image)] as string;
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
                _inputTypeWithFormatStringHashtable[EInputTypeUtils.GetValue(EInputType.Image)] = value;
            }
        }

        public string FormatDefault
        {
            get
            {
                var formatString = _inputTypeWithFormatStringHashtable[EInputTypeUtils.GetValue(EInputType.Text)] as string;
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
                _inputTypeWithFormatStringHashtable[EInputTypeUtils.GetValue(EInputType.Text)] = value;
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

        protected virtual string GetFormatString(EInputType inputType)
        {
            string formatString;
            if (inputType == EInputType.TextEditor)
            {
                formatString = FormatTextEditor;
            }
            else if (inputType == EInputType.Image)
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

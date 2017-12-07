using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Controls
{
    public class AuxiliaryControl : Control
    {
        private NameValueCollection _formCollection;
        private PublishmentSystemInfo _publishmentSystemInfo;
        private int _nodeId;
        private List<int> _relatedIdentities;
        private ETableStyle _tableStyle;
        private string _tableName;
        private bool _isEdit;
        private bool _isPostBack;

        public void SetParameters(NameValueCollection formCollection, PublishmentSystemInfo publishmentSystemInfo, int nodeId, List<int> relatedIdentities, ETableStyle tableStyle, string tableName, bool isEdit, bool isPostBack)
        {
            _formCollection = formCollection;
            _publishmentSystemInfo = publishmentSystemInfo;
            _nodeId = nodeId;
            _relatedIdentities = relatedIdentities;
            _tableStyle = tableStyle;
            _tableName = tableName;
            _isEdit = isEdit;
            _isPostBack = isPostBack;
        }

        protected override void Render(HtmlTextWriter output)
        {
            if (string.IsNullOrEmpty(_tableName)) return;

            if (_formCollection == null)
            {
                _formCollection = HttpContext.Current.Request.Form.Count > 0 ? HttpContext.Current.Request.Form : new NameValueCollection();
            }

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);
            var pageScripts = new NameValueCollection();

            if (styleInfoList == null) return;

            var builder = new StringBuilder();
            foreach (var styleInfo in styleInfoList)
            {
                if (!styleInfo.IsVisible) continue;

                string extra;
                var value = BackgroundInputTypeParser.Parse(_publishmentSystemInfo, _nodeId, styleInfo, _tableStyle, styleInfo.AttributeName, _formCollection, _isEdit, _isPostBack, null, pageScripts, out extra);

                if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor))
                {
                    var commands = WebUtils.GetTextEditorCommands(_publishmentSystemInfo, styleInfo.AttributeName);
                    builder.Append($@"
<div class=""form-group"">
    <label class=""col-sm-1 control-label"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-10"">
        {commands}
        <div class=""m-t-10"">
            {value}
        </div>
    </div>
    <div class=""col-sm-1"">
        {extra}
    </div>
</div>");
                }
                else
                {
                    builder.Append($@"
<div class=""form-group"">
    <label class=""col-sm-1 control-label"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-6"">
        {value}
    </div>
    <div class=""col-sm-5"">
        {extra}
    </div>
</div>");
                }
            }

            output.Write(builder.ToString());

            foreach (string key in pageScripts.Keys)
            {
                output.Write(pageScripts[key]);
            }
        }
    }
}

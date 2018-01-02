using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Table;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Controls
{
	public class ChannelAuxiliaryControl : Control
	{
        public IAttributes Attributes { get; set; }

        public PublishmentSystemInfo PublishmentSystemInfo { get; set; }

        public int NodeId { get; set; }

        public string AdditionalAttributes { get; set; }

		protected override void Render(HtmlTextWriter output)
		{
            if (Attributes == null) return;

            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemInfo.PublishmentSystemId, NodeId);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.NodeDao.TableName, relatedIdentities);

		    if (styleInfoList == null) return;

            var builder = new StringBuilder();
		    var pageScripts = new NameValueCollection();
		    foreach (var styleInfo in styleInfoList)
		    {
		        string extra;
		        var value = BackgroundInputTypeParser.Parse(PublishmentSystemInfo, NodeId, styleInfo, Attributes, pageScripts, out extra);

		        if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor))
                {
                    builder.Append($@"
<div class=""form-group"">
    <label class=""col-sm-1 control-label"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-10"">
        {value}
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
    <label class=""col-sm-2 control-label"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-4"">
        {value}
    </div>
    <div class=""col-sm-6"">
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

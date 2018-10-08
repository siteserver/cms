using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Controls
{
	public class ChannelAuxiliaryControl : Control
	{
        public IAttributes Attributes { get; set; }

        public SiteInfo SiteInfo { get; set; }

        public int ChannelId { get; set; }

        public string AdditionalAttributes { get; set; }

		protected override void Render(HtmlTextWriter output)
		{
            if (Attributes == null) return;

		    var channelInfo = ChannelManager.GetChannelInfo(SiteInfo.Id, ChannelId);
            var styleInfoList = TableStyleManager.GetChannelStyleInfoList(channelInfo);

		    if (styleInfoList == null) return;

            var builder = new StringBuilder();
		    var pageScripts = new NameValueCollection();
		    foreach (var styleInfo in styleInfoList)
		    {
		        var value = BackgroundInputTypeParser.Parse(SiteInfo, ChannelId, styleInfo, Attributes, pageScripts, out var extra);

		        if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (styleInfo.InputType == InputType.TextEditor)
                {
                    builder.Append($@"
<div class=""form-group form-row"">
    <label class=""col-sm-2 col-form-label text-right"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-9"">
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
<div class=""form-group form-row"">
    <label class=""col-sm-2 col-form-label text-right"">{styleInfo.DisplayName}</label>
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

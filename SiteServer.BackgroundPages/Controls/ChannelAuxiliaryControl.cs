using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;


namespace SiteServer.BackgroundPages.Controls
{
	public class ChannelAuxiliaryControl : Control
	{
        public IDictionary<string, object> Attributes { get; set; }

        public Site Site { get; set; }

        public int ChannelId { get; set; }

        public string AdditionalAttributes { get; set; }

		protected override void Render(HtmlTextWriter output)
		{
            if (Attributes == null) return;

		    var channelInfo = DataProvider.ChannelRepository.GetAsync(ChannelId).GetAwaiter().GetResult();
            var styleList = DataProvider.TableStyleRepository.GetChannelStyleListAsync(channelInfo).GetAwaiter().GetResult();

		    if (styleList == null) return;

            var builder = new StringBuilder();
		    var pageScripts = new NameValueCollection();
		    foreach (var style in styleList)
		    {
		        var (value, extra) = BackgroundInputTypeParser.ParseAsync(Site, ChannelId, style, Attributes, pageScripts).GetAwaiter().GetResult();

		        if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (style.InputType == InputType.TextEditor)
                {
                    builder.Append($@"
<div class=""form-group form-row"">
    <label class=""col-sm-2 col-form-label text-right"">{style.DisplayName}</label>
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
    <label class=""col-sm-2 col-form-label text-right"">{style.DisplayName}</label>
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

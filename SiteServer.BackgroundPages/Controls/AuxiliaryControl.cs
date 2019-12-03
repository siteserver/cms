using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Controls
{
    public class AuxiliaryControl : Control
    {
        public IDictionary<string, object> Attributes { get; set; }

        public Site Site { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public List<TableStyle> StyleList { get; set; }

        protected override void Render(HtmlTextWriter output)
        {
            if (StyleList == null || StyleList.Count == 0 || Attributes == null) return;

            var pageScripts = new NameValueCollection();

            var builder = new StringBuilder();
            foreach (var style in StyleList)
            {
                if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.Title)) continue;

                string extra;
                var value = BackgroundInputTypeParser.Parse(Site, ChannelId, style, Attributes, pageScripts, out extra);

                if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (style.Type == InputType.TextEditor)
                {
                    var commands = WebUtils.GetTextEditorCommands(Site, style.AttributeName);
                    builder.Append($@"
<div class=""form-group form-row"">
    <label class=""col-sm-1 col-form-label text-right text-truncate text-nowrap"">{style.DisplayName}</label>
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
                    var html = $@"
<div class=""form-group form-row"">
    <label class=""col-sm-1 col-form-label text-right text-truncate text-nowrap"">{style.DisplayName}</label>
    <div class=""col-sm-6"">
        {value}
    </div>
    <div class=""col-sm-5"">
        {extra}
    </div>
</div>";

                    if (style.Type == InputType.Customize)
                    {
                        var eventArgs = new ContentFormLoadEventArgs(Site.Id, ChannelId, ContentId, Attributes, style.AttributeName, html);
                        foreach (var service in PluginManager.GetServicesAsync().GetAwaiter().GetResult())
                        {
                            try
                            {
                                html = service.OnContentFormLoad(eventArgs);
                            }
                            catch (Exception ex)
                            {
                                LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(IService.ContentFormLoad)).GetAwaiter().GetResult();
                            }
                        }
                    }

                    builder.Append(html);
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

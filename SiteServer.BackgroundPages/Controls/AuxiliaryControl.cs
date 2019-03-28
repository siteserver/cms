using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Controls
{
    public class AuxiliaryControl : Control
    {
        public IDictionary<string, object> Attributes { get; set; }

        public SiteInfo SiteInfo { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public List<TableStyleInfo> StyleInfoList { get; set; }

        protected override void Render(HtmlTextWriter output)
        {
            if (StyleInfoList == null || StyleInfoList.Count == 0 || Attributes == null) return;

            var pageScripts = new NameValueCollection();

            var builder = new StringBuilder();
            foreach (var styleInfo in StyleInfoList)
            {
                if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.Title)) continue;

                var value = BackgroundInputTypeParser.Parse(SiteInfo, ChannelId, styleInfo, Attributes, pageScripts, out var extra);

                if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (styleInfo.Type == InputType.TextEditor)
                {
                    var commands = WebUtils.GetTextEditorCommands(SiteInfo, styleInfo.AttributeName);
                    builder.Append($@"
<div class=""form-group form-row"">
    <label class=""col-sm-1 col-form-label text-right text-truncate"" title=""{styleInfo.DisplayName}"">{styleInfo.DisplayName}</label>
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
    <label class=""col-sm-1 col-form-label text-right text-truncate"" title=""{styleInfo.DisplayName}"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-6"">
        {value}
    </div>
    <div class=""col-sm-5"">
        {extra}
    </div>
</div>";

                    if (styleInfo.Type == InputType.Customize)
                    {
                        var eventArgs = new ContentFormLoadEventArgs(SiteInfo.Id, ChannelId, ContentId, Attributes, styleInfo.AttributeName, html);
                        foreach (var service in PluginManager.Services)
                        {
                            try
                            {
                                html = service.OnContentFormLoad(eventArgs);
                            }
                            catch (Exception ex)
                            {
                                LogUtils.AddErrorLog(service.PluginId, ex, nameof(IService.ContentFormLoad));
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

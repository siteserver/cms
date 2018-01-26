using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Controls
{
    public class AuxiliaryControl : Control
    {
        public IAttributes Attributes { get; set; }

        public SiteInfo SiteInfo { get; set; }

        public int NodeId { get; set; }

        public List<TableStyleInfo> StyleInfoList { get; set; }

        protected override void Render(HtmlTextWriter output)
        {
            if (StyleInfoList == null || StyleInfoList.Count == 0 || Attributes == null) return;

            var pageScripts = new NameValueCollection();

            var builder = new StringBuilder();
            foreach (var styleInfo in StyleInfoList)
            {
                string extra;
                var value = BackgroundInputTypeParser.Parse(SiteInfo, NodeId, styleInfo, Attributes, pageScripts, out extra);

                if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (styleInfo.InputType == InputType.TextEditor)
                {
                    var commands = WebUtils.GetTextEditorCommands(SiteInfo, styleInfo.AttributeName);
                    builder.Append($@"
<div class=""form-group form-row"">
    <label class=""col-sm-1 col-form-label text-right"">{styleInfo.DisplayName}</label>
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
                    var htmlBuilder = new StringBuilder($@"
<div class=""form-group form-row"">
    <label class=""col-sm-1 col-form-label text-right"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-6"">
        {value}
    </div>
    <div class=""col-sm-5"">
        {extra}
    </div>
</div>");

                    if (styleInfo.InputType == InputType.Customize)
                    {
                        var eventArgs = new ContentFormLoadEventArgs(SiteInfo.Id, NodeId, styleInfo.AttributeName, Attributes, htmlBuilder);
                        foreach (var service in PluginManager.Services)
                        {
                            try
                            {
                                service.OnContentFormLoad(eventArgs);
                            }
                            catch (Exception ex)
                            {
                                LogUtils.AddPluginErrorLog(service.PluginId, ex, nameof(IService.ContentFormLoad));
                            }
                        }
                    }

                    builder.Append(htmlBuilder);
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

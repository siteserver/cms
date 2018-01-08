using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Features;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Controls
{
    public class AuxiliaryControl : Control
    {
        public IAttributes Attributes { get; set; }

        public PublishmentSystemInfo PublishmentSystemInfo { get; set; }

        public int NodeId { get; set; }

        public List<TableStyleInfo> StyleInfoList { get; set; }

        public Dictionary<string, IContentRelated> Plugins { get; set; }

        protected override void Render(HtmlTextWriter output)
        {
            if (StyleInfoList == null || StyleInfoList.Count == 0 || Attributes == null) return;

            var pageScripts = new NameValueCollection();

            var pluginFuncDictLowercase = new Dictionary<string, Func<int, int, IAttributes, string>>();
            foreach (var pluginId in Plugins.Keys)
            {
                var plugin = Plugins[pluginId];

                if (plugin.ContentFormCustomized == null || plugin.ContentFormCustomized.Count == 0) continue;

                foreach (var attributeName in plugin.ContentFormCustomized.Keys)
                {
                    if (!pluginFuncDictLowercase.ContainsKey(attributeName.ToLower()) && plugin.ContentFormCustomized[attributeName] != null)
                    {
                        pluginFuncDictLowercase[attributeName.ToLower()] = plugin.ContentFormCustomized[attributeName];
                    }
                }
            }

            var builder = new StringBuilder();
            foreach (var styleInfo in StyleInfoList)
            {
                if (pluginFuncDictLowercase.ContainsKey(styleInfo.AttributeName.ToLower()))
                {
                    var formFunc = pluginFuncDictLowercase[styleInfo.AttributeName.ToLower()];

                    var html = string.Empty;
                    try
                    {
                        html = formFunc(PublishmentSystemInfo.PublishmentSystemId, NodeId, Attributes);
                    }
                    catch (Exception ex)
                    {
                        var formPluginId = string.Empty;
                        foreach (var pluginId in Plugins.Keys)
                        {
                            var plugin = Plugins[pluginId];

                            if (plugin.ContentFormCustomized == null || plugin.ContentFormCustomized.Count == 0) continue;

                            foreach (var attributeName in plugin.ContentFormCustomized.Keys)
                            {
                                if (plugin.ContentFormCustomized[attributeName] == null) continue;
                                formPluginId = pluginId;
                                break;
                            }
                        }
                        LogUtils.AddPluginErrorLog(formPluginId, ex, "ContentFormCustomized");
                    }

                    builder.Append(html);
                    continue;
                }

                string extra;
                var value = BackgroundInputTypeParser.Parse(PublishmentSystemInfo, NodeId, styleInfo, Attributes, pageScripts, out extra);

                if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor))
                {
                    var commands = WebUtils.GetTextEditorCommands(PublishmentSystemInfo, styleInfo.AttributeName);
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
                    builder.Append($@"
<div class=""form-group form-row"">
    <label class=""col-sm-1 col-form-label text-right"">{styleInfo.DisplayName}</label>
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

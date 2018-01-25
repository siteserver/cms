using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Controls
{
    public class AuxiliaryControl : Control
    {
        public IAttributes Attributes { get; set; }

        public SiteInfo SiteInfo { get; set; }

        public int NodeId { get; set; }

        public List<TableStyleInfo> StyleInfoList { get; set; }

        public Dictionary<string, Dictionary<string, Func<int, int, IAttributes, string>>> Plugins { get; set; }

        protected override void Render(HtmlTextWriter output)
        {
            if (StyleInfoList == null || StyleInfoList.Count == 0 || Attributes == null) return;

            var pageScripts = new NameValueCollection();

            var pluginFuncDictLowercase = new Dictionary<string, Func<int, int, IAttributes, string>>();
            if (Plugins != null)
            {
                foreach (var pluginId in Plugins.Keys)
                {
                    var dict = Plugins[pluginId];

                    if (dict == null || dict.Count == 0) continue;

                    foreach (var attributeName in dict.Keys)
                    {
                        if (!pluginFuncDictLowercase.ContainsKey(attributeName.ToLower()) && dict[attributeName] != null)
                        {
                            pluginFuncDictLowercase[attributeName.ToLower()] = dict[attributeName];
                        }
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
                        html = formFunc(SiteInfo.Id, NodeId, Attributes);
                    }
                    catch (Exception ex)
                    {
                        var formPluginId = string.Empty;
                        if (Plugins != null)
                        {
                            foreach (var pluginId in Plugins.Keys)
                            {
                                var dict = Plugins[pluginId];

                                if (dict == null || dict.Count == 0) continue;

                                foreach (var attributeName in dict.Keys)
                                {
                                    if (dict[attributeName] == null) continue;
                                    formPluginId = pluginId;
                                    break;
                                }
                            }
                        }
                        LogUtils.AddPluginErrorLog(formPluginId, ex, nameof(IService.AddCustomizedContentForm));
                    }

                    builder.Append(html);
                    continue;
                }

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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace SiteServer.Plugin
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PluginMenu
    {
        public string Id { get; set; }
        public string TopId { get; set; }
        public string ParentId { get; set; }
        public string Text { get; set; }
        public string IconUrl { get; set; }
        public bool Selected { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }
        public List<string> Permissions { get; set; }
        public List<PluginMenu> Menus { get; set; }
    }
}

using System;

namespace SS.CMS.Models
{
    [Serializable]
    public class PackageInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string Homepage { get; set; }
        public string Icon { get; set; }
    }
}

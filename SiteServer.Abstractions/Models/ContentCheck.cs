using System;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_ContentCheck")]
	public class ContentCheck : Entity
	{
        [DataColumn]
        public string TableName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public string UserName { get; set; }

        [DataColumn]
        public string IsChecked { get; set; }

        public bool Checked
        {
            get => TranslateUtils.ToBool(IsChecked);
            set => IsChecked = value.ToString();
        }

        [DataColumn]
        public int CheckedLevel { get; set; }

        [DataColumn]
        public DateTime CheckDate { get; set; }

        [DataColumn]
        public string Reasons { get; set; }
    }
}

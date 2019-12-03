using System;
using System.Collections.Generic;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_SitePermissions")]
    public class SitePermissions : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        public string ChannelIdCollection { get; set; }

        [DataColumn(Text = true)]
        public string ChannelPermissions { get; set; }

        [DataColumn(Text = true)]
        public string WebsitePermissions { get; set; }

        public List<int> ChannelIdList
        {
            get => StringUtils.GetIntList(ChannelIdCollection);
            set => ChannelIdCollection = StringUtils.Join(value);
        }

        public List<string> ChannelPermissionList
        {
            get => StringUtils.GetStringList(ChannelPermissions);
            set => ChannelPermissions = StringUtils.Join(value);
        }

        public List<string> WebsitePermissionList
        {
            get => StringUtils.GetStringList(WebsitePermissions);
            set => WebsitePermissions = StringUtils.Join(value);
        }
    }
}
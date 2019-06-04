using SS.CMS.Core.Common;

namespace SS.CMS.Api.Common
{
    public partial class Request
    {
        public void AddSiteLog(int siteId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, 0, 0, IpAddress, AdminName, action, summary);
        }

        public void AddChannelLog(int siteId, int channelId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, channelId, 0, IpAddress, AdminName, action, summary);
        }

        public void AddContentLog(int siteId, int channelId, int contentId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, channelId, contentId, IpAddress, AdminName, action, summary);
        }

        public void AddAdminLog(string action, string summary = "")
        {
            LogUtils.AddAdminLog(IpAddress, AdminName, action, summary);
        }

        public void AddUserLog(string action, string summary = "")
        {
            LogUtils.AddUserLog(IpAddress, UserName, action, summary);
        }
    }
}

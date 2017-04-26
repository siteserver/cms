using Top.Api.Cluster;

namespace Top.Api
{
    public class AutoRetryClusterTopClient : AutoRetryTopClient
    {
        public AutoRetryClusterTopClient(string serverUrl, string appKey, string appSecret)
            : base(serverUrl, appKey, appSecret)
        {
            ClusterManager.InitRefreshThread(this);
        }

        public AutoRetryClusterTopClient(string serverUrl, string appKey, string appSecret, string format)
            : base(serverUrl, appKey, appSecret, format)
        {
            ClusterManager.InitRefreshThread(this);
        }

        internal override string GetServerUrl(string serverUrl, string apiName, string session)
        {
            DnsConfig dnsConfig = ClusterManager.GetDnsConfigFromCache();
            if (dnsConfig == null)
            {
                return serverUrl;
            }
            else
            {
                return dnsConfig.GetBestVipUrl(serverUrl, apiName, session);
            }
        }

        internal override string GetSdkVersion()
        {
            return Constants.SDK_VERSION_CLUSTER;
        }
    }
}

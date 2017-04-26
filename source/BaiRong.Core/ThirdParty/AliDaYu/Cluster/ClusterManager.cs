using System;
using System.Collections.Generic;
using System.Threading;

namespace Top.Api.Cluster
{
    public sealed class ClusterManager
    {
        private static readonly Random random = new Random();
        private static readonly Object initLock = new Object();
        private static volatile DnsConfig dnsConfig = null;
        private static volatile Thread refreshThread = null;

        public static T GetElementByWeight<T>(List<T> list) where T : Weightable
        {
            T selected = null;
            double totalWeight = 0d;
            foreach (T element in list)
            {
                double r = random.NextDouble() * (element.Weight + totalWeight);
                if (r >= totalWeight)
                {
                    selected = element;
                }
                totalWeight += element.Weight;
            }
            return selected;
        }

        public static DnsConfig GetDnsConfigFromCache()
        {
            return dnsConfig;
        }

        public static void InitRefreshThread(ITopClient client)
        {
            if (refreshThread == null)
            {
                lock (initLock)
                {
                    if (refreshThread == null)
                    {
                        try
                        {
                            dnsConfig = GetDnsConfigFromTop(client);
                        }
                        catch (TopException e)
                        {
                            if ("22".Equals(e.ErrorCode))
                            {
                                return; // 如果HTTP DNS服务不存在，则退出守护线程
                            }
                            else
                            {
                                throw e;
                            }
                        }

                        refreshThread = new Thread(o =>
                        {
                            while (true)
                            {
                                try
                                {
                                    Thread.Sleep(dnsConfig.GetRefreshInterval() * 60 * 1000);
                                    dnsConfig = GetDnsConfigFromTop(client);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.StackTrace);
                                    Thread.Sleep(3 * 1000); // 出错则过3秒重试
                                }
                            }
                        });
                        refreshThread.IsBackground = true;
                        refreshThread.Name = "HTTP_DNS_REFRESH_THREAD";
                        refreshThread.Start();
                    }
                }
            }
        }

        private static DnsConfig GetDnsConfigFromTop(ITopClient client)
        {
            HttpdnsGetRequest req = new HttpdnsGetRequest();
            HttpdnsGetResponse rsp = client.Execute(req);
            if (!rsp.IsError)
            {
                return DnsConfig.parse(rsp.Result);
            }
            else
            {
                throw new TopException(rsp.ErrCode, rsp.ErrMsg);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Cluster
{
    public class Weightable
    {
        public double Weight { get; set; }
    }
    public class ApiRule : Weightable
    {
        public string Name { get; set; }
    }
    public class ApiConfig
    {
        public string User { get; set; }

        public List<ApiRule> Rules { get; set; }
    }

    public class VipRule : Weightable
    {
        public string Vip { get; set; }
    }

    public class EnvConfig
    {
        public string Domain { get; set; }

        public string Protocol { get; set; }

        public List<VipRule> VipRules { get; set; }
    }
    public class DnsConfig
    {
        public IDictionary<string, string> GlobalDict { get; set; }

        public IDictionary<string, ApiConfig> ApiDict { get; set; }

        public IDictionary<string, List<EnvConfig>> EnvDict { get; set; }

        public IDictionary<string, IDictionary<string, string>> UserDict { get; set; }

        public static DnsConfig parse(string json)
        {
            DnsConfig dnsConfig = new DnsConfig();
            IDictionary root = TopUtils.JsonToObject(json) as IDictionary;
            foreach (string configType in root.Keys)
            {
                if ("config".Equals(configType))
                {
                    dnsConfig.GlobalDict = new Dictionary<string, string>();
                    IDictionary globalInfo = root[configType] as IDictionary;
                    foreach (string key in globalInfo.Keys)
                    {
                        dnsConfig.GlobalDict[key] = globalInfo[key] as string;
                    }
                }
                else if ("env".Equals(configType))
                {
                    IDictionary envInfos = root[configType] as IDictionary;
                    dnsConfig.EnvDict = new Dictionary<string, List<EnvConfig>>();
                    foreach (string envName in envInfos.Keys)
                    {
                        IDictionary envInfo = envInfos[envName] as IDictionary;
                        List<EnvConfig> envConfigs = new List<EnvConfig>();

                        foreach (string domainName in envInfo.Keys)
                        {
                            IDictionary domainInfo = envInfo[domainName] as IDictionary;
                            EnvConfig envConfig = new EnvConfig();
                            envConfig.Domain = domainName as string;
                            envConfig.Protocol = domainInfo["proto"] as string;
                            IList vipInfos = domainInfo["vip"] as IList;
                            List<VipRule> vipRules = new List<VipRule>();
                            foreach (string vipInfo in vipInfos)
                            {
                                string[] vipInfoTmp = vipInfo.ToString().Split('|');
                                VipRule vipRule = new VipRule();
                                vipRule.Vip = vipInfoTmp[0];
                                vipRule.Weight = double.Parse(vipInfoTmp[1]);
                                vipRules.Add(vipRule);
                            }
                            envConfig.VipRules = vipRules;
                            envConfigs.Add(envConfig);
                        }

                        dnsConfig.EnvDict[envName] = envConfigs;
                    }
                }
                else if ("api".Equals(configType))
                {
                    dnsConfig.ApiDict = new Dictionary<string, ApiConfig>();
                    IDictionary apiInfos = root[configType] as IDictionary;
                    foreach (string apiName in apiInfos.Keys)
                    {
                        IDictionary apiInfo = apiInfos[apiName] as IDictionary;
                        ApiConfig apiConfig = new ApiConfig();
                        apiConfig.User = apiInfo["user"] as string;
                        List<ApiRule> apiRules = new List<ApiRule>();
                        IList apiRuleInfos = apiInfo["rule"] as IList;
                        foreach (string apiRuleInfo in apiRuleInfos)
                        {
                            string[] apiRuleInfoTmp = apiRuleInfo.ToString().Split('|');
                            ApiRule apiRule = new ApiRule();
                            apiRule.Name = apiRuleInfoTmp[0];
                            apiRule.Weight = double.Parse(apiRuleInfoTmp[1]);
                            apiRules.Add(apiRule);
                        }
                        apiConfig.Rules = apiRules;
                        dnsConfig.ApiDict[apiName] = apiConfig;
                    }
                }
                else if ("user".Equals(configType))
                {
                    dnsConfig.UserDict = new Dictionary<string, IDictionary<string, string>>();
                    IDictionary userInfos = root[configType] as IDictionary;
                    foreach (string routeName in userInfos.Keys)
                    {
                        IDictionary envInfos = userInfos[routeName] as IDictionary;
                        IDictionary<string, string> tags = new Dictionary<string, string>();
                        foreach (string envName in envInfos.Keys)
                        {
                            IList tagInfos = envInfos[envName] as IList;
                            foreach (string tagName in tagInfos)
                            {
                                tags.Add(tagName, envName);
                            }
                        }
                        dnsConfig.UserDict[routeName] = tags;
                    }
                }
            }
            return dnsConfig;
        }

        public string GetBestVipUrl(string serverUrl, string apiName, string session)
        {
            if (ApiDict.ContainsKey(apiName))
            {
                ApiConfig apiConfig = ApiDict[apiName];
                if (!string.IsNullOrEmpty(session) && apiConfig.User != null)
                {
                    string flag = GetUserFlag(session);
                    if (flag != null && UserDict.ContainsKey(apiConfig.User))
                    {
                        IDictionary<string, string> userEnvs = UserDict[apiConfig.User];
                        if (userEnvs.ContainsKey(flag))
                        {
                            string userEnv = userEnvs[flag];
                            if (EnvDict.ContainsKey(userEnv))
                            {
                                List<EnvConfig> envConfigs = EnvDict[userEnv];
                                return GetEnvVipUrl(serverUrl, envConfigs);
                            }
                        }
                    }
                }
                return GetApiVipUrl(serverUrl, apiConfig);
            }
            else
            {
                List<EnvConfig> envConfigs = EnvDict[GlobalDict["def_env"]];
                return GetEnvVipUrl(serverUrl, envConfigs);
            }
        }

        private string GetApiVipUrl(string serverUrl, ApiConfig apiConfig)
        {
            ApiRule apiRule = ClusterManager.GetElementByWeight(apiConfig.Rules);
            if (EnvDict.ContainsKey(apiRule.Name))
            {
                List<EnvConfig> envConfigs = EnvDict[apiRule.Name];
                return GetEnvVipUrl(serverUrl, envConfigs);
            }
            else
            {
                List<EnvConfig> envConfigs = EnvDict[GlobalDict["def_env"]];
                return GetEnvVipUrl(serverUrl, envConfigs);
            }
        }

        private string GetEnvVipUrl(string serverUrl, List<EnvConfig> envConfigs)
        {
            foreach (EnvConfig envConfig in envConfigs)
            {
                Uri uri = new Uri(serverUrl);
                if (uri.Host.Equals(envConfig.Domain, StringComparison.OrdinalIgnoreCase) && uri.Scheme.Equals(envConfig.Protocol, StringComparison.OrdinalIgnoreCase))
                {
                    string vip = ClusterManager.GetElementByWeight<VipRule>(envConfig.VipRules).Vip;
                    return serverUrl.Replace(envConfig.Domain, vip);
                }
            }
            return serverUrl;
        }

        private string GetUserFlag(string session)
        {
            if (!string.IsNullOrEmpty(session) && session.Length > 5)
            {
                if (session.StartsWith("6") || session.StartsWith("7"))
                {
                    return session.Substring(session.Length - 1, 1);
                }
                else if (session.StartsWith("5") || session.StartsWith("8"))
                {
                    return session.Substring(5, 1);
                }
            }
            return null;
        }

        public int GetRefreshInterval()
        {
            if (GlobalDict.ContainsKey("interval"))
            {
                return int.Parse(GlobalDict["interval"]);
            }
            else
            {
                return 30; // 默认半小时刷新一次
            }
        }

    }

    public class HttpdnsGetResponse : TopResponse
    {
        [XmlElement("result")]
        public string Result { get; set; }
    }

    public class HttpdnsGetRequest : BaseTopRequest<HttpdnsGetResponse>
    {
        public override string GetApiName()
        {
            return "taobao.httpdns.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            return parameters;
        }

        public override void Validate()
        {
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System.Web.Caching;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Core
{
    public class StlCacheManager
    {
        private StlCacheManager()
        {
        }

        public class FirstContentId
        {
            private FirstContentId()
            {
            }

            private const string CacheKey = "SiteServer.CreateCache.FirstContentID";

            public static Hashtable GetHashtable()
            {
                var ht = CacheUtils.Get(CacheKey) as Hashtable;
                if (ht == null)
                {
                    ht = new Hashtable();
                    CacheUtils.Insert(CacheKey, ht, null, CacheUtils.SecondFactorCalculate(15));
                }
                return ht;
            }

            public static int GetValue(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
            {
                int firstContentId;

                var hashtable = GetHashtable();
                if (hashtable[nodeInfo.NodeId] != null)
                {
                    firstContentId = (int)hashtable[nodeInfo.NodeId];
                }
                else
                {
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                    firstContentId = BaiRongDataProvider.ContentDao.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc));
                    hashtable[nodeInfo.NodeId] = firstContentId;
                }

                return firstContentId;
            }
        }

        public class NodeIdList
        {
            private NodeIdList()
            {
            }

            private const string CacheKey = "SiteServer.CreateCache.NodeIdList";

            public static Dictionary<string, List<int>> GetDictionary()
            {
                var dic = CacheUtils.Get(CacheKey) as Dictionary<string, List<int>>;
                if (dic != null) return dic;

                dic = new Dictionary<string, List<int>>();
                CacheUtils.Insert(CacheKey, dic, null, CacheUtils.SecondFactorCalculate(15));
                return dic;
            }

            public static List<int> GetNodeIdListByScopeType(NodeInfo nodeInfo, EScopeType scopeType)
            {
                List<int> list;

                var dic = GetDictionary();
                var key = EScopeTypeUtils.GetValue(scopeType) + nodeInfo.NodeId;
                if (dic.ContainsKey(key))
                {
                    list = dic[key];
                }
                else
                {
                    list = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo, scopeType, string.Empty, string.Empty);
                    dic[key] = list;
                }

                return list;
            }
        }

        public class NodeId
        {
            private NodeId()
            {
            }

            private const string CacheKey = "SiteServer.CreateCache.NodeID";

            public static Hashtable GetHashtable()
            {
                var ht = CacheUtils.Get(CacheKey) as Hashtable;
                if (ht != null) return ht;

                ht = new Hashtable();
                CacheUtils.Insert(CacheKey, ht, null, CacheUtils.SecondFactorCalculate(15));
                return ht;
            }

            public static int GetNodeIdByChannelIdOrChannelIndexOrChannelName(int publishmentSystemId, int channelId, string channelIndex, string channelName)
            {
                var retval = channelId;

                var hashtable = GetHashtable();
                string key = $"{publishmentSystemId}_{channelId}_{channelIndex}_{channelName}";
                if (hashtable[key] != null)
                {
                    retval = (int)hashtable[key];
                }
                else
                {
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        var theNodeId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(publishmentSystemId, channelIndex);
                        if (theNodeId != 0)
                        {
                            retval = theNodeId;
                        }
                    }
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        var theNodeId = DataProvider.NodeDao.GetNodeIdByParentIdAndNodeName(publishmentSystemId, retval, channelName, true);
                        if (theNodeId == 0)
                        {
                            theNodeId = DataProvider.NodeDao.GetNodeIdByParentIdAndNodeName(publishmentSystemId, publishmentSystemId, channelName, true);
                        }
                        if (theNodeId != 0)
                        {
                            retval = theNodeId;
                        }
                    }

                    hashtable[key] = retval;
                }

                return retval;
            }
        }

        public class FileContent
        {
            private FileContent()
            {
            }

            public static string GetTemplateContent(PublishmentSystemInfo publishmentSystemInfo, TemplateInfo templateInfo)
            {
                var filePath = TemplateManager.GetTemplateFilePath(publishmentSystemInfo, templateInfo);
                return GetContentByFilePath(filePath, templateInfo.Charset);
            }

            public static string GetIncludeContent(PublishmentSystemInfo publishmentSystemInfo, string file, ECharset charset)
            {
                var filePath = PathUtility.MapPath(publishmentSystemInfo, PathUtility.AddVirtualToPath(file));
                return GetContentByFilePath(filePath, charset);
            }

            public static string GetContentByFilePath(string filePath, ECharset charset)
            {
                try
                {
                    if (CacheUtils.Get(filePath) != null) return CacheUtils.Get(filePath) as string;

                    var content = string.Empty;
                    if (FileUtils.IsFileExists(filePath))
                        content = FileUtils.ReadText(filePath, charset);

                    CacheUtils.Insert(filePath, content, new CacheDependency(filePath), CacheUtils.HourFactor);
                    return content;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}

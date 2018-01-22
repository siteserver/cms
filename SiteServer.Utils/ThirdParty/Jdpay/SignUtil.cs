using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteServer.Utils.ThirdParty.Jdpay
{
    public class SignUtil
    {
        public static String signRemoveSelectedKeys(SortedDictionary<String, String> dic, String rsaPriKey, List<String> signKeyList)
        {
            //获取签名需要字符串和类型
            String sourceSignString = SignUtil.signString(dic, signKeyList);
            //摘要
            String sha256SourceSignString = SHAUtil.encryptSHA256(sourceSignString);
            byte[] newsks = RSACoder.encryptByPrivateKey(sha256SourceSignString, rsaPriKey);
            return Convert.ToBase64String(newsks, Base64FormattingOptions.InsertLineBreaks);
        }


        /// <summary>
        /// 拼装需要签名原串
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="unSignKeyList"></param>
        /// <returns></returns>
        public static String signString(SortedDictionary<String, String> dic, List<String> unSignKeyList)
        {
            bool isFirst = true;
            string link = "";
            foreach (KeyValuePair<string, string> kv in dic)
            {
                if (kv.Value == null || kv.Value.Trim().Length == 0)
                {
                    continue;
                }
                bool falg = false;
                for (int i = 0; i < unSignKeyList.Count; i++)
                {
                    if (unSignKeyList[i].Equals(kv.Key))
                    {
                        falg = true;
                        break;
                    }
                }
                if (falg)
                {
                    continue;
                }
                if (!isFirst)
                {
                    link += "&";
                }
                link += kv.Key + "=" + kv.Value;
                if (isFirst)
                {
                    isFirst = false;
                }
            }
            return link;
        }
    }
}

using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.WeiXinMP.CommonAPIs;

namespace SiteServer.CMS.WeiXin.MP
{
    public class MPUtils
    {
        public static string GetAccessToken(AccountInfo accountInfo)
        {
            if (AccessTokenContainer.CheckRegistered(accountInfo.AppID) == false)
            {
                AccessTokenContainer.Register(accountInfo.AppID, accountInfo.AppSecret);
            }
            return AccessTokenContainer.GetToken(accountInfo.AppID);
        }

        public static string GetSummary(string summary, string content)
        {
            if (!string.IsNullOrEmpty(summary))
            {
                return summary;
            }
            else
            {
                return StringUtils.MaxLengthText(StringUtils.StripTags(content), 200);
            }
        }

        public static string GetSitePreivewHtml(PublishmentSystemInfo publishmentSystemInfo, int nodeID, int contentID)
        {
            if (contentID > 0)
            {
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeID);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeID);

                var contentInfo = DataProvider.ContentDao.GetContentInfoNotTrash(tableStyle, tableName, contentID);
                if (contentInfo != null)
                {
                    var pageUrl = PageUtilityWX.GetContentUrl(publishmentSystemInfo, contentInfo);
                    return $@"内容页：{contentInfo.Title}&nbsp;<a href=""{pageUrl}"" target=""blank"">查看</a>";
                }
            }
            else if (nodeID > 0)
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                if (nodeInfo != null)
                {
                    var nodeNames = NodeManager.GetNodeNameNavigation(publishmentSystemInfo.PublishmentSystemId, nodeID);
                    var pageUrl = PageUtilityWX.GetChannelUrl(publishmentSystemInfo, nodeInfo);
                    return $@"栏目页：{nodeNames}&nbsp;<a href=""{pageUrl}"" target=""blank"">查看</a>";
                }
            }
            return string.Empty;
        }

        public static string GetChannelOrContentSelectScript(PublishmentSystemInfo publishmentSystemInfo, int nodeID, int contentID)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
            if (nodeInfo != null)
            {
                if (contentID > 0)
                {
                    var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

                    var contentInfo = DataProvider.ContentDao.GetContentInfoNotTrash(tableStyle, tableName, contentID);

                    if (contentInfo != null)
                    {
                        var pageUrl = PageUtilityWX.GetContentUrl(publishmentSystemInfo, contentInfo);
                        return $@"contentSelect(""{contentInfo.Title}"", ""{nodeID}"", ""{contentID}"", ""{pageUrl}"")";
                    }
                }
                else if (nodeID > 0)
                {
                    var nodeNames = NodeManager.GetNodeNameNavigation(publishmentSystemInfo.PublishmentSystemId, nodeID);
                    var pageUrl = PageUtilityWX.GetChannelUrl(publishmentSystemInfo, nodeInfo);
                    return $"selectChannel('{nodeNames}', '{nodeID}', '{pageUrl}');";
                }
            }
            return string.Empty;
        }

        private static Dictionary<string, int> emotionDictionary = null;

        public static Dictionary<string, int>  EmotionDictionary
        {
            get
            {
                if (emotionDictionary == null)
                {
                    emotionDictionary = new Dictionary<string, int>();

                    emotionDictionary["微笑"] = 0;
                    emotionDictionary["撇嘴"] = 1;
                    emotionDictionary["色"] = 2;
                    emotionDictionary["发呆"] = 3;
                    emotionDictionary["得意"] = 4;
                    emotionDictionary["流泪"] = 5;
                    emotionDictionary["害羞"] = 6;
                    emotionDictionary["闭嘴"] = 7;
                    emotionDictionary["睡"] = 8;
                    emotionDictionary["大哭"] = 9;
                    emotionDictionary["尴尬"] = 10;
                    emotionDictionary["发怒"] = 11;
                    emotionDictionary["调皮"] = 12;
                    emotionDictionary["呲牙"] = 13;
                    emotionDictionary["惊讶"] = 14;
                    emotionDictionary["难过"] = 15;
                    emotionDictionary["酷"] = 16;
                    emotionDictionary["冷汗"] = 17;
                    emotionDictionary["抓狂"] = 18;
                    emotionDictionary["吐"] = 19;
                    emotionDictionary["偷笑"] = 20;
                    emotionDictionary["可爱"] = 21;
                    emotionDictionary["白眼"] = 22;
                    emotionDictionary["傲慢"] = 23;
                    emotionDictionary["饥饿"] = 24;
                    emotionDictionary["困"] = 25;
                    emotionDictionary["惊恐"] = 26;
                    emotionDictionary["流汗"] = 27;
                    emotionDictionary["憨笑"] = 28;
                    emotionDictionary["大兵"] = 29;
                    emotionDictionary["奋斗"] = 30;
                    emotionDictionary["咒骂"] = 31;
                    emotionDictionary["疑问"] = 32;
                    emotionDictionary["嘘"] = 33;
                    emotionDictionary["晕"] = 34;
                    emotionDictionary["折磨"] = 35;
                    emotionDictionary["衰"] = 36;
                    emotionDictionary["骷髅"] = 37;
                    emotionDictionary["敲打"] = 38;
                    emotionDictionary["再见"] = 39;
                    emotionDictionary["擦汗"] = 40;
                    emotionDictionary["抠鼻"] = 41;
                    emotionDictionary["鼓掌"] = 42;
                    emotionDictionary["糗大了"] = 43;
                    emotionDictionary["坏笑"] = 44;
                    emotionDictionary["左哼哼"] = 45;
                    emotionDictionary["右哼哼"] = 46;
                    emotionDictionary["哈欠"] = 47;
                    emotionDictionary["鄙视"] = 48;
                    emotionDictionary["委屈"] = 49;
                    emotionDictionary["快哭了"] = 50;
                    emotionDictionary["阴险"] = 51;
                    emotionDictionary["亲亲"] = 52;
                    emotionDictionary["吓"] = 53;
                    emotionDictionary["可怜"] = 54;
                    emotionDictionary["菜刀"] = 55;
                    emotionDictionary["西瓜"] = 56;
                    emotionDictionary["啤酒"] = 57;
                    emotionDictionary["篮球"] = 58;
                    emotionDictionary["乒乓"] = 59;
                    emotionDictionary["咖啡"] = 60;
                    emotionDictionary["饭"] = 61;
                    emotionDictionary["猪头"] = 62;
                    emotionDictionary["玫瑰"] = 63;
                    emotionDictionary["凋谢"] = 64;
                    emotionDictionary["示爱"] = 65;
                    emotionDictionary["爱心"] = 66;
                    emotionDictionary["心碎"] = 67;
                    emotionDictionary["蛋糕"] = 68;
                    emotionDictionary["闪电"] = 69;
                    emotionDictionary["炸弹"] = 70;
                    emotionDictionary["刀"] = 71;
                    emotionDictionary["足球"] = 72;
                    emotionDictionary["瓢虫"] = 73;
                    emotionDictionary["便便"] = 74;
                    emotionDictionary["月亮"] = 75;
                    emotionDictionary["太阳"] = 76;
                    emotionDictionary["礼物"] = 77;
                    emotionDictionary["拥抱"] = 78;
                    emotionDictionary["强"] = 79;
                    emotionDictionary["弱"] = 80;
                    emotionDictionary["握手"] = 81;
                    emotionDictionary["胜利"] = 82;
                    emotionDictionary["抱拳"] = 83;
                    emotionDictionary["勾引"] = 84;
                    emotionDictionary["拳头"] = 85;
                    emotionDictionary["差劲"] = 86;
                    emotionDictionary["爱你"] = 87;
                    emotionDictionary["NO"] = 88;
                    emotionDictionary["OK"] = 89;
                    emotionDictionary["爱情"] = 90;
                    emotionDictionary["飞吻"] = 91;
                    emotionDictionary["跳跳"] = 92;
                    emotionDictionary["发抖"] = 93;
                    emotionDictionary["怄火"] = 94;
                    emotionDictionary["转圈"] = 95;
                    emotionDictionary["磕头"] = 96;
                    emotionDictionary["回头"] = 97;
                    emotionDictionary["跳绳"] = 98;
                    emotionDictionary["挥手"] = 99;
                    emotionDictionary["激动"] = 100;
                    emotionDictionary["街舞"] = 101;
                    emotionDictionary["献吻"] = 102;
                    emotionDictionary["右太极"] = 103;
                    emotionDictionary["左太极"] = 104;
                }
                return emotionDictionary;
            }
        }

        public static string ParseEmotions(string content)
        {
            if (!string.IsNullOrEmpty(content) && content.IndexOf("&#47;") != -1)
            {
                foreach (var val in EmotionDictionary)
                {
                    content = StringUtils.Replace("&#47;" + val.Key, content,
                        $@"<img src=""img/face/{val.Value}.gif"" />");
                }
            }
            return content;
        }
    }
}

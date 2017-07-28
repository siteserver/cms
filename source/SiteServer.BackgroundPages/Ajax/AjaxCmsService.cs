using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Text;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxCmsService : Page
    {
        private const string TypeGetTitles = "GetTitles";
        private const string TypeGetWordSpliter = "GetWordSpliter";
        private const string TypeGetDetection = "GetDetection";
        private const string TypeGetDetectionReplace = "GetDetectionReplace";
        private const string TypeGetTags = "GetTags";

        public static string GetRedirectUrl(string type)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", type}
            });
        }

        public static string GetTitlesUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetTitles},
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"nodeID", nodeId.ToString()}
            });
        }

        public static string GetWordSpliterUrl(int publishmentSystemId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetWordSpliter},
                {"publishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetDetectionUrl(int publishmentSystemId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetDetection},
                {"publishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetDetectionReplaceUrl(int publishmentSystemId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetDetectionReplace},
                {"publishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetTagsUrl(int publishmentSystemId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetTags},
                {"publishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request["type"];
            var retString = string.Empty;

            if (type == TypeGetTitles)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request["publishmentSystemID"]);
                var channelId = TranslateUtils.ToInt(Request["channelID"]);
                var nodeId = TranslateUtils.ToInt(Request["nodeID"]);
                if (channelId > 0)
                {
                    nodeId = channelId;
                }
                var title = Request["title"];
                var titles = GetTitles(publishmentSystemId, nodeId, title);

                Page.Response.Write(titles);
                Page.Response.End();

                return;
            }
            if (type == TypeGetWordSpliter)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request["publishmentSystemID"]);
                var contents = Request.Form["content"];
                var tags = WordSpliter.GetKeywords(contents, publishmentSystemId, 10);

                Page.Response.Write(tags);
                Page.Response.End();

                return;
            }

            if (type == TypeGetTags)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request["publishmentSystemID"]);
                var tag = Request["tag"];
                var tags = GetTags(publishmentSystemId, tag);

                Page.Response.Write(tags);
                Page.Response.End();

                return;
            }

            if (type == TypeGetDetection)
            {
                var content = Request.Form["content"];
                var arraylist = DataProvider.KeywordDao.GetKeywordListByContent(content);
                var keywords = TranslateUtils.ObjectCollectionToString(arraylist);

                Page.Response.Write(keywords);
                Page.Response.End();
            }
            else if (type == TypeGetDetectionReplace)
            {
                var content = Request.Form["content"];
                var keywordList = DataProvider.KeywordDao.GetKeywordListByContent(content);
                var keywords = string.Empty;
                if (keywordList.Count > 0)
                {
                    var list = DataProvider.KeywordDao.GetKeywordInfoList(keywordList);
                    foreach (var keywordInfo in list)
                    {
                        keywords += keywordInfo.Keyword + "|" + keywordInfo.Alternative + ",";
                    }
                    keywords = keywords.TrimEnd(',');
                }
                Page.Response.Write(keywords);
                Page.Response.End();
            }

            Page.Response.Write(retString);
            Page.Response.End();
        }

        public string GetTitles(int publishmentSystemId, int nodeId, string title)
        {
            var retval = new StringBuilder();

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
            var titleList = BaiRongDataProvider.ContentDao.GetValueListByStartString(tableName, nodeId, ContentAttribute.Title, title, 10);
            if (titleList.Count > 0)
            {
                foreach (var value in titleList)
                {
                    retval.Append(value);
                    retval.Append("|");
                }
                retval.Length -= 1;
            }

            return retval.ToString();
        }

        public string GetTags(int publishmentSystemId, string tag)
        {
            var retval = new StringBuilder();

            var tagList = BaiRongDataProvider.TagDao.GetTagListByStartString(publishmentSystemId, tag, 10);
            if (tagList.Count > 0)
            {
                foreach (var value in tagList)
                {
                    retval.Append(value);
                    retval.Append("|");
                }
                retval.Length -= 1;
            }

            return retval.ToString();
        }
    }
}

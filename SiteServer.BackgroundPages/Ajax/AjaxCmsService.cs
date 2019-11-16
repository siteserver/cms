using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using SiteServer.CMS.Context;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxCmsService : Page
    {
        private const string TypeGetTitles = "GetTitles";
        private const string TypeGetWordSpliter = "GetWordSpliter";
        private const string TypeGetTags = "GetTags";

        public static string GetRedirectUrl(string type)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", type}
            });
        }

        public static string GetTitlesUrl(int siteId, int channelId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetTitles},
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()}
            });
        }

        public static string GetWordSpliterUrl(int siteId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetWordSpliter},
                {"siteId", siteId.ToString()}
            });
        }

        public static string GetTagsUrl(int siteId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCmsService), new NameValueCollection
            {
                {"type", TypeGetTags},
                {"siteId", siteId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request["type"];
            var retString = string.Empty;

            if (type == TypeGetTitles)
            {
                var siteId = TranslateUtils.ToInt(Request["siteId"]);
                var channelId = TranslateUtils.ToInt(Request["channelId"]);
                var title = Request["title"];
                var titles = GetTitlesAsync(siteId, channelId, title).GetAwaiter().GetResult();

                Page.Response.Write(titles);
                Page.Response.End();

                return;
            }
            if (type == TypeGetWordSpliter)
            {
                var siteId = TranslateUtils.ToInt(Request["siteId"]);
                var contents = Request.Form["content"];
                var tags = WordSpliter.GetKeywordsAsync(contents, siteId, 10).GetAwaiter().GetResult();

                Page.Response.Write(tags);
                Page.Response.End();

                return;
            }

            if (type == TypeGetTags)
            {
                var siteId = TranslateUtils.ToInt(Request["siteId"]);
                var tag = Request["tag"];
                var tags = GetTagsAsync(siteId, tag).GetAwaiter().GetResult();

                Page.Response.Write(tags);
                Page.Response.End();

                return;
            }

            Page.Response.Write(retString);
            Page.Response.End();
        }

        public async Task<string> GetTitlesAsync(int siteId, int channelId, string title)
        {
            var retVal = new StringBuilder();

            var site = await SiteManager.GetSiteAsync(siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channelId);

            var titleList = await DataProvider.ContentDao.GetValueListByStartStringAsync(tableName, channelId, ContentAttribute.Title, title, 10);
            if (titleList.Count > 0)
            {
                foreach (var value in titleList)
                {
                    retVal.Append(value);
                    retVal.Append("|");
                }
                retVal.Length -= 1;
            }

            return retVal.ToString();
        }

        public async Task<string> GetTagsAsync(int siteId, string tag)
        {
            var retVal = new StringBuilder();

            var tagList = await DataProvider.ContentTagDao.GetTagListByStartStringAsync(siteId, tag, 10);
            if (tagList.Count > 0)
            {
                foreach (var value in tagList)
                {
                    retVal.Append(value);
                    retVal.Append("|");
                }
                retVal.Length -= 1;
            }

            return retVal.ToString();
        }
    }
}

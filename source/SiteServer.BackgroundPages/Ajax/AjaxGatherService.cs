using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxGatherService : Page
    {
        public const string CacheTotalCount = "_TotalCount";
        public const string CacheCurrentCount = "_CurrentCount";
        public const string CacheMessage = "_Message";

        private const string TypeGetCountArray = "GetCountArray";
        private const string TypeGather = "Gather";
        private const string TypeGatherDatabase = "GatherDatabase";
        private const string TypeGatherFile = "GatherFile";

        public static string GetCountArrayUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxGatherService), new NameValueCollection
            {
                {"type", TypeGetCountArray }
            });
        }

        public static string GetGatherUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxGatherService), new NameValueCollection
            {
                {"type", TypeGather }
            });
        }

        public static string GetGatherParameters(int publishmentSystemId, string gatherRuleNameCollection, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"gatherRuleNameCollection", gatherRuleNameCollection},
                {"userKeyPrefix", userKeyPrefix},
            });
        }

        public static string GetGatherDatabaseUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxGatherService), new NameValueCollection
            {
                {"type", TypeGatherDatabase }
            });
        }

        public static string GetGatherDatabaseParameters(int publishmentSystemId, string gatherRuleNameCollection, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"gatherRuleNameCollection", gatherRuleNameCollection},
                {"userKeyPrefix", userKeyPrefix},
            });
        }

        public static string GetGatherFileUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxGatherService), new NameValueCollection
            {
                {"type", TypeGatherFile }
            });
        }

        public static string GetGatherFileParameters(int publishmentSystemId, string gatherRuleNameCollection, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"gatherRuleNameCollection", gatherRuleNameCollection},
                {"userKeyPrefix", userKeyPrefix},
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request.QueryString["type"];
            var userKeyPrefix = Request["userKeyPrefix"];
            var retval = new NameValueCollection();
            var body = new RequestBody();

            if (type == TypeGetCountArray)
            {
                retval = GetCountArray(userKeyPrefix);
            }
            else if (type == TypeGather)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.Form["publishmentSystemID"]);
                var gatherRuleNameCollection = Request.Form["gatherRuleNameCollection"];
                retval = Gather(publishmentSystemId, gatherRuleNameCollection, userKeyPrefix, body.AdministratorName);
            }
            else if (type == TypeGatherDatabase)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.Form["publishmentSystemID"]);
                var gatherRuleNameCollection = Request.Form["gatherRuleNameCollection"];
                retval = GatherDatabase(publishmentSystemId, gatherRuleNameCollection, userKeyPrefix, body.AdministratorName);
            }
            else if (type == TypeGatherFile)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.Form["publishmentSystemID"]);
                var gatherRuleNameCollection = Request.Form["gatherRuleNameCollection"];
                retval = GatherFile(publishmentSystemId, gatherRuleNameCollection, userKeyPrefix, body.AdministratorName);
            }

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(retval);
            Page.Response.Write(jsonString);
            Page.Response.End();
        }

        public NameValueCollection GetCountArray(string userKeyPrefix)//进度及显示
        {
            var retval = new NameValueCollection();
            if (CacheUtils.Get(userKeyPrefix + CacheTotalCount) != null && CacheUtils.Get(userKeyPrefix + CacheCurrentCount) != null && CacheUtils.Get(userKeyPrefix + CacheMessage) != null)
            {
                var totalCount = TranslateUtils.ToInt((string)CacheUtils.Get(userKeyPrefix + CacheTotalCount));
                var currentCount = TranslateUtils.ToInt((string)CacheUtils.Get(userKeyPrefix + CacheCurrentCount));
                var message = (string)CacheUtils.Get(userKeyPrefix + CacheMessage);
                retval = AjaxManager.GetCountArrayNameValueCollection(totalCount, currentCount, message);
            }
            return retval;
        }

        public NameValueCollection Gather(int publishmentSystemId, string gatherRuleNameCollection, string userKeyPrefix, string administratorName)
        {
            var resultBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var gatherRuleNameArrayList = TranslateUtils.StringCollectionToStringList(gatherRuleNameCollection);
            foreach (string gatherRuleName in gatherRuleNameArrayList)
            {
                GatherUtility.GatherWeb(publishmentSystemId, gatherRuleName, resultBuilder, errorBuilder, true, userKeyPrefix, administratorName);
            }
            return AjaxManager.GetProgressTaskNameValueCollection(resultBuilder.ToString(), errorBuilder.ToString());
        }

        public NameValueCollection GatherDatabase(int publishmentSystemId, string gatherRuleNameCollection, string userKeyPrefix, string administratorName)
        {
            var resultBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var gatherRuleNameArrayList = TranslateUtils.StringCollectionToStringList(gatherRuleNameCollection);
            foreach (string gatherRuleName in gatherRuleNameArrayList)
            {
                GatherUtility.GatherDatabase(publishmentSystemId, gatherRuleName, resultBuilder, errorBuilder, true, userKeyPrefix, administratorName);
            }
            return AjaxManager.GetProgressTaskNameValueCollection(resultBuilder.ToString(), errorBuilder.ToString());
        }

        public NameValueCollection GatherFile(int publishmentSystemId, string gatherRuleNameCollection, string userKeyPrefix, string administratorName)
        {
            var resultBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var gatherRuleNameArrayList = TranslateUtils.StringCollectionToStringList(gatherRuleNameCollection);
            foreach (string gatherRuleName in gatherRuleNameArrayList)
            {
                GatherUtility.GatherFile(publishmentSystemId, gatherRuleName, resultBuilder, errorBuilder, true, userKeyPrefix, administratorName);
            }
            return AjaxManager.GetProgressTaskNameValueCollection(resultBuilder.ToString(), errorBuilder.ToString());
        }
    }
}

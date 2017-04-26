using System;
using System.Collections.Specialized;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxCreateService : Page
    {
        public const string CacheTotalCount = "_TotalCount";
        public const string CacheCurrentCount = "_CurrentCount";
        public const string CacheMessage = "_Message";

        private const string TypeGetCountArray = "GetCountArray";
        private const string TypeCreatePublishmentSystem = "CreatePublishmentSystem";

        public static string GetCountArrayUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCreateService), new NameValueCollection
            {
                {"type", TypeGetCountArray }
            });
        }

        public static string GetCreatePublishmentSystemUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCreateService), new NameValueCollection
            {
                {"type", TypeCreatePublishmentSystem }
            });
        }

        public static string GetCreatePublishmentSystemParameters(int publishmentSystemId, bool isUseSiteTemplate, bool isImportContents, bool isImportTableStyles, string siteTemplateDir, bool isUseTables, string userKeyPrefix, bool isTop, string returnUrl)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"isUseSiteTemplate", isUseSiteTemplate.ToString()},
                {"isImportContents", isImportContents.ToString()},
                {"isImportTableStyles", isImportTableStyles.ToString()},
                {"siteTemplateDir", siteTemplateDir},
                {"isUseTables", isUseTables.ToString()},
                {"userKeyPrefix", userKeyPrefix},
                {"isTop", isTop.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
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
            //else if (type == "GetCountArrayForService")
            //{
            //    retval = GetCountArrayForService(userKeyPrefix);
            //}
            //    else if (type == "CreateChannels")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        retval = CreateChannels(publishmentSystemID, userKeyPrefix);
            //    }
            //    else if (type == "CreateChannelsOneByOne")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        bool isIncludeChildren = TranslateUtils.ToBool(base.Request.Form["isIncludeChildren"]);
            //        bool isCreateContents = TranslateUtils.ToBool(base.Request.Form["isCreateContents"]);
            //        retval = CreateChannelsOneByOne(publishmentSystemID, userKeyPrefix, isIncludeChildren, isCreateContents);
            //    }
            //    else if (type == "CreateContents")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        retval = CreateContents(publishmentSystemID, userKeyPrefix);
            //    }
            //    else if (type == "CreateContentsByService")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        int createTaskID = TranslateUtils.ToInt(base.Request.Form["createTaskID"]);
            //        retval = CreateContentsByService(publishmentSystemID, userKeyPrefix, createTaskID);
            //    }
            //    else if (type == "CreateContentsOneByOne")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        int nodeID = TranslateUtils.ToInt(base.Request.Form["nodeID"]);
            //        retval = CreateContentsOneByOne(publishmentSystemID, nodeID, userKeyPrefix);
            //    }
            //    else if (type == "CreateByTemplate")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        int templateID = TranslateUtils.ToInt(base.Request.Form["templateID"]);
            //        retval = CreateByTemplate(publishmentSystemID, templateID, userKeyPrefix);
            //    }
            //    else if (type == "CreateByIDsCollection")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        retval = CreateByIDsCollection(publishmentSystemID, userKeyPrefix);
            //    }
            //    else if (type == "CreateFiles")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        retval = CreateFiles(publishmentSystemID, userKeyPrefix);
            //    }
            if (type == TypeCreatePublishmentSystem)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.Form["publishmentSystemID"]);
                var isUseSiteTemplate = TranslateUtils.ToBool(Request.Form["isUseSiteTemplate"]);
                var isImportContents = TranslateUtils.ToBool(Request.Form["isImportContents"]);
                var isImportTableStyles = TranslateUtils.ToBool(Request.Form["isImportTableStyles"]);
                var siteTemplateDir = Request.Form["siteTemplateDir"];
                var isUseTables = TranslateUtils.ToBool(Request.Form["isUseTables"]);
                var returnUrl = Request.Form["returnUrl"];
                var isTop = TranslateUtils.ToBool(Request.Form["isTop"], false);
                retval = CreatePublishmentSystem(publishmentSystemId, isUseSiteTemplate, isImportContents, isImportTableStyles, siteTemplateDir, isUseTables, userKeyPrefix, returnUrl, isTop, body.AdministratorName);
            }
            //    else if (type == "CreateAll")
            //    {
            //        int publishmentSystemID = TranslateUtils.ToInt(base.Request.Form["publishmentSystemID"]);
            //        retval = CreateAll(publishmentSystemID, userKeyPrefix);
            //    }

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

        #region 创建站点

        public NameValueCollection CreatePublishmentSystem(int publishmentSystemId, bool isUseSiteTemplate, bool isImportContents, bool isImportTableStyles, string siteTemplateDir, bool isUseTables, string userKeyPrefix, string returnUrl, string administratorName)
        {
            var cacheTotalCountKey = userKeyPrefix + CacheTotalCount;
            var cacheCurrentCountKey = userKeyPrefix + CacheCurrentCount;
            var cacheMessageKey = userKeyPrefix + CacheMessage;

            CacheUtils.Max(cacheTotalCountKey, "3");//存储需要的页面总数
            CacheUtils.Max(cacheCurrentCountKey, "0");//存储当前的页面总数
            CacheUtils.Max(cacheMessageKey, string.Empty);//存储消息

            //返回“运行结果”、“错误信息”及“执行JS脚本”的字符串数组
            NameValueCollection retval;

            try
            {

                CacheUtils.Max(cacheCurrentCountKey, "1");//存储当前的页面总数
                CacheUtils.Max(cacheMessageKey, "正在创建站点...");//存储消息
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);


                CacheUtils.Max(cacheCurrentCountKey, "2");//存储当前的页面总数
                CacheUtils.Max(cacheMessageKey, "正在导入数据...");//存储消息
                if (isUseSiteTemplate && !string.IsNullOrEmpty(siteTemplateDir))
                {
                    SiteTemplateManager.Instance.ImportSiteTemplateToEmptyPublishmentSystem(publishmentSystemId, siteTemplateDir, isUseTables, isImportContents, isImportTableStyles, administratorName);
                }

                CacheUtils.Max(cacheCurrentCountKey, "3");//存储当前的页面总数
                CacheUtils.Max(cacheMessageKey, "创建成功！");//存储消息
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = PageUtils.AddQueryString(StringUtils.ValueFromUrl(returnUrl), "PublishmentSystemID", publishmentSystemId.ToString());
                    retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{publishmentSystemInfo.PublishmentSystemName}<strong> 创建成功!", string.Empty,
                        $"location.href='{returnUrl}';");
                }
                else
                {
                    retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{publishmentSystemInfo.PublishmentSystemName}<strong> 创建成功!", string.Empty,
                        $"top.location.href='{PageInitialization.GetRedirectUrl()}';");
                }
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.Clear();

            return retval;
        }

        public NameValueCollection CreatePublishmentSystem(int publishmentSystemId, bool isUseSiteTemplate, bool isImportContents, bool isImportTableStyles, string siteTemplateDir, bool isUseTables, string userKeyPrefix, string returnUrl, bool isTop, string administratorName)
        {
            var cacheTotalCountKey = userKeyPrefix + CacheTotalCount;
            var cacheCurrentCountKey = userKeyPrefix + CacheCurrentCount;
            var cacheMessageKey = userKeyPrefix + CacheMessage;

            CacheUtils.Max(cacheTotalCountKey, "3");//存储需要的页面总数
            CacheUtils.Max(cacheCurrentCountKey, "0");//存储当前的页面总数
            CacheUtils.Max(cacheMessageKey, string.Empty);//存储消息

            //返回“运行结果”、“错误信息”及“执行JS脚本”的字符串数组
            NameValueCollection retval;

            try
            {
                CacheUtils.Max(cacheCurrentCountKey, "1");//存储当前的页面总数
                CacheUtils.Max(cacheMessageKey, "正在创建站点...");//存储消息

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);




                CacheUtils.Max(cacheCurrentCountKey, "2");//存储当前的页面总数
                CacheUtils.Max(cacheMessageKey, "正在导入数据...");//存储消息
                if (isUseSiteTemplate && !string.IsNullOrEmpty(siteTemplateDir))
                {
                    SiteTemplateManager.Instance.ImportSiteTemplateToEmptyPublishmentSystem(publishmentSystemId, siteTemplateDir, isUseTables, isImportContents, isImportTableStyles, administratorName);
                }

                CreateManager.CreateAll(publishmentSystemId);

                CacheUtils.Max(cacheCurrentCountKey, "3");//存储当前的页面总数
                CacheUtils.Max(cacheMessageKey, "创建成功！");//存储消息
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = PageUtils.AddQueryString(StringUtils.ValueFromUrl(returnUrl), "PublishmentSystemID", publishmentSystemId.ToString());
                    retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{publishmentSystemInfo.PublishmentSystemName}<strong> 创建成功!", string.Empty, isTop ?
                            $"top.location.href='{returnUrl}';"
                            : $"location.href='{returnUrl}';");
                }
                else
                {
                    var initUrl = PageInitialization.GetRedirectUrl();
                    retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{publishmentSystemInfo.PublishmentSystemName}<strong> 创建成功!", string.Empty, isTop ?
                            $"location.href='{initUrl}';"
                            : $"top.location.href='{initUrl}';");
                }
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.Clear();

            return retval;
        }

        #endregion
    }
}

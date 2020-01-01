using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Web;
using SiteServer.CMS.Api;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin.Apis
{
    public class UtilsApi : IUtilsApi
    {
        private UtilsApi() { }

        private static UtilsApi _instance;
        public static UtilsApi Instance => _instance ?? (_instance = new UtilsApi());

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString);
        }

        public string FilterXss(string html)
        {
            return AttackUtils.FilterXss(html);
        }

        public string FilterSql(string sql)
        {
            return AttackUtils.FilterSql(sql);
        }

        public string GetTemporaryFilesPath(string relatedPath)
        {
            return PathUtils.GetTemporaryFilesPath(relatedPath);
        }

        public string GetRootUrl(string relatedUrl = "")
        {
            return PageUtils.GetRootUrl(relatedUrl);
        }

        public string GetAdminUrl(string relatedUrl = "")
        {
            return PageUtils.GetAdminUrl(relatedUrl);
        }

        public string GetHomeUrl(string relatedUrl = "")
        {
            return PageUtils.GetHomeUrl(relatedUrl);
        }

        public string GetApiUrl(string relatedUrl = "")
        {
            return ApiManager.GetInnerApiUrl(relatedUrl);
        }

        public void CreateZip(string zipFilePath, string directoryPath)
        {
            ZipUtils.CreateZip(zipFilePath, directoryPath);
        }

        public void ExtractZip(string zipFilePath, string directoryPath)
        {
            ZipUtils.ExtractZip(zipFilePath, directoryPath);
        }

        public string JsonSerialize(object obj)
        {
            return TranslateUtils.JsonSerialize(obj);
        }

        public T JsonDeserialize<T>(string json, T defaultValue = default(T))
        {
            return TranslateUtils.JsonDeserialize(json, defaultValue);
        }

        public IAuthenticatedRequest GetAuthenticatedRequest(HttpRequestMessage request)
        {
            throw new System.NotImplementedException();
        }

        public IAuthenticatedRequest GetAuthenticatedRequest()
        {
            return new AuthenticatedRequest(HttpContext.Current.Request);
        }
    }
}

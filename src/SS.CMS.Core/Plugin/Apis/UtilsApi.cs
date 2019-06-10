using SS.CMS.Abstractions;
using SS.CMS.Core.Api;
using SS.CMS.Core.Settings;
using SS.CMS.Utils;

namespace SS.CMS.Core.Plugin.Apis
{
    public class UtilsApi : IUtilsApi
    {
        private UtilsApi() { }

        private static UtilsApi _instance;
        public static UtilsApi Instance => _instance ?? (_instance = new UtilsApi());

        public string Encrypt(string inputString)
        {
            return AppContext.Encrypt(inputString);
        }

        public string Decrypt(string inputString)
        {
            return AppContext.Decrypt(inputString);
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
            return AppContext.GetTemporaryFilesPath(relatedPath);
        }

        public string GetRootUrl(string relatedUrl = "")
        {
            return AppContext.GetRootUrl(relatedUrl);
        }

        public string GetAdminUrl(string relatedUrl = "")
        {
            return AppContext.GetAdminUrl(relatedUrl);
        }

        public string GetHomeUrl(string relatedUrl = "")
        {
            return AppContext.GetHomeUrl(relatedUrl);
        }

        public string GetApiUrl(string relatedUrl = "")
        {
            return ApiManager.GetApiUrl(relatedUrl);
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
    }
}

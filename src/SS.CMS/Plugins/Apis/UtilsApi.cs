using SS.CMS;

using SS.CMS.Abstractions;

namespace SiteServer.CMS.Plugin.Apis
{
    public class UtilsApi
    {
        private UtilsApi() { }

        private static UtilsApi _instance;
        public static UtilsApi Instance => _instance ??= new UtilsApi();

        public string Encrypt(string inputString)
        {
            return WebConfigUtils.EncryptStringBySecretKey(inputString);
        }

        public string Decrypt(string inputString)
        {
            return WebConfigUtils.DecryptStringBySecretKey(inputString);
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

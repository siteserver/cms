using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Controllers.Stl
{
    public class Digg
    {
        public const string Route = "stl/digg/{publishmentSystemId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int relatedIdentity, int updaterId, EDiggType diggType, string goodText, string badText, string theme, bool isDigg, bool isGood)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            return PageUtils.AddQueryString(apiUrl, new NameValueCollection
            {
                {"relatedIdentity", relatedIdentity.ToString() },
                {"updaterId", updaterId.ToString() },
                {"diggType", EDiggTypeUtils.GetValue(diggType) },
                {"goodText", TranslateUtils.EncryptStringBySecretKey(goodText) },
                {"badText", TranslateUtils.EncryptStringBySecretKey(badText) },
                {"theme", theme },
                {"isDigg", isDigg.ToString() },
                {"isGood", isGood.ToString() }
            });
        }
    }
}
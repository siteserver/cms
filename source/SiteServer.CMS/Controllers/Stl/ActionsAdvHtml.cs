using BaiRong.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsAdvHtml
    {
        public const string Route = "stl/actions/adv_html/{publishmentSystemId}/{uniqueId}/{area}/{channelId}/{fileTemplateId}/{templateType}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int uniqueId, string area, int channelId, int fileTemplateId, ETemplateType templateType)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{uniqueId}", uniqueId.ToString());
            apiUrl = apiUrl.Replace("{area}", area);
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{fileTemplateId}", fileTemplateId.ToString());
            apiUrl = apiUrl.Replace("{templateType}", ETemplateTypeUtils.GetValue(templateType));
            return apiUrl;
        }
    }
}
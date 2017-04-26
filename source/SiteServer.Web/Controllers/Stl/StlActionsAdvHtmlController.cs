using System.Collections;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Advertisement;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsAdvHtmlController : ApiController
    {
        [HttpGet]
        [Route(ActionsAdvHtml.Route)]
        public void Main(int publishmentSystemId, int uniqueId, string area, int channelId, int fileTemplateId, string templateType)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var eTemplateType = ETemplateTypeUtils.GetEnumType(templateType);

            if (!string.IsNullOrEmpty(area) && !string.IsNullOrEmpty(templateType))
            {
                AdvInfo advInfo = null;
                if (eTemplateType == ETemplateType.IndexPageTemplate || eTemplateType == ETemplateType.ChannelTemplate || eTemplateType == ETemplateType.ContentTemplate)
                {
                    advInfo = AdvManager.GetAdvInfoByAdAreaName(eTemplateType, area, publishmentSystemId, channelId, 0);
                }
                else if (eTemplateType == ETemplateType.FileTemplate)
                {
                    advInfo = AdvManager.GetAdvInfoByAdAreaName(eTemplateType, area, publishmentSystemId, 0, fileTemplateId);
                }
                if (advInfo != null)
                {
                    ArrayList adMaterialInfoList;
                    var adMaterialInfo = AdvManager.GetShowAdMaterialInfo(publishmentSystemId, advInfo, out adMaterialInfoList);
                    if (advInfo.RotateType == EAdvRotateType.Equality || advInfo.RotateType == EAdvRotateType.HandWeight)
                    {
                        if (adMaterialInfo.AdMaterialType == EAdvType.HtmlCode)
                        {
                            HttpContext.Current.Response.Write($@"<!--
document.write('{StringUtils.ToJsString(adMaterialInfo.Code)}');
-->
");
                        }
                        else if (adMaterialInfo.AdMaterialType == EAdvType.Text)
                        {
                            var style = string.Empty;
                            if (!string.IsNullOrEmpty(adMaterialInfo.TextColor))
                            {
                                style += $"color:{adMaterialInfo.TextColor};";
                            }
                            if (adMaterialInfo.TextFontSize > 0)
                            {
                                style += $"font-size:{adMaterialInfo.TextFontSize}px;";
                            }
                            HttpContext.Current.Response.Write($@"<!--
document.write('<a href=""{PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, adMaterialInfo.TextLink))}"" target=""_blank"" style=""{style}"">{StringUtils.ToJsString(adMaterialInfo.TextWord)}</a>\r\n');
-->
");
                        }
                        else if (adMaterialInfo.AdMaterialType == EAdvType.Image)
                        {
                            var attribute = string.Empty;
                            if (adMaterialInfo.ImageWidth > 0)
                            {
                                attribute += $@" width=""{adMaterialInfo.ImageWidth}""";
                            }
                            if (adMaterialInfo.ImageHeight > 0)
                            {
                                attribute += $@" height=""{adMaterialInfo.ImageHeight}""";
                            }
                            if (!string.IsNullOrEmpty(adMaterialInfo.ImageAlt))
                            {
                                attribute += $@" title=""{adMaterialInfo.ImageAlt}""";
                            }
                            HttpContext.Current.Response.Write($@"<!--
document.write('<a href=""{PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, adMaterialInfo.ImageLink))}"" target=""_blank""><img src=""{PageUtility.ParseNavigationUrl(publishmentSystemInfo, adMaterialInfo.ImageUrl)}"" {attribute} border=""0"" /></a>\r\n');
-->
");
                        }
                        else if (adMaterialInfo.AdMaterialType == EAdvType.Flash)
                        {
                            var imageUrl = PageUtility.ParseNavigationUrl(publishmentSystemInfo,
                                adMaterialInfo.ImageUrl);
                            HttpContext.Current.Response.Write($@"<!--
document.write('<div id=""flashcontent_{uniqueId}""></div>');
var so_{uniqueId} = new SWFObject(""{imageUrl}"", ""flash_{uniqueId}"", ""{adMaterialInfo.ImageWidth}"", ""{adMaterialInfo.ImageHeight}"", ""7"", """");

so_{uniqueId}.addParam(""quality"", ""high"");
so_{uniqueId}.addParam(""wmode"", ""transparent"");

so_{uniqueId}.write(""flashcontent_{uniqueId}"");
-->
");
                        }
                    }
                }
            }

            HttpContext.Current.Response.End();
        }
    }
}

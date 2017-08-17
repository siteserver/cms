using System.Collections;
using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core.Advertisement;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "固定广告", Description = "通过 stl:ad 标签在模板中显示指定位置的广告")]
    public class StlAd
    {
        private StlAd() { }
        public const string ElementName = "stl:ad";

        public const string AttributeArea = "area";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeArea, "广告位"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            var area = string.Empty;
            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeArea))
                {
                    area = value;
                }
            }

            //var adAreaInfo = DataProvider.AdAreaDao.GetAdAreaInfo(area, pageInfo.PublishmentSystemId);
            var adAreaInfo = AdArea.GetAdAreaInfo(area, pageInfo.PublishmentSystemId);
            if (adAreaInfo != null)
            {
                if (adAreaInfo.IsEnabled)
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

                    var adMaterialInfoList = new ArrayList();
                    var advInfo = AdvManager.GetAdvInfoByAdAreaName(pageInfo.TemplateInfo.TemplateType, adAreaInfo.AdAreaName, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.TemplateInfo.TemplateId);
                    if (advInfo != null)
                    {
                        if (advInfo.RotateType == EAdvRotateType.Equality || advInfo.RotateType == EAdvRotateType.HandWeight)
                        {
                            var templateType = pageInfo.TemplateInfo.TemplateType;
                            if (templateType == ETemplateType.IndexPageTemplate || templateType == ETemplateType.ChannelTemplate || templateType == ETemplateType.ContentTemplate)
                            {
                                parsedContent =
                                    $"<script src='{ActionsAdvHtml.GetUrl(pageInfo.PublishmentSystemInfo.Additional.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.UniqueId, area, pageInfo.PageNodeId, 0, pageInfo.TemplateInfo.TemplateType)}' language='javascript'></script>";
                            }
                            else if (templateType == ETemplateType.FileTemplate)
                            {
                                parsedContent =
                                    $"<script src='{ActionsAdvHtml.GetUrl(pageInfo.PublishmentSystemInfo.Additional.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.UniqueId, area, 0, pageInfo.TemplateInfo.TemplateId, pageInfo.TemplateInfo.TemplateType)}' language='javascript'></script>";
                            }
                        }
                        else if (advInfo.RotateType == EAdvRotateType.SlideRotate)
                        {
                            parsedContent =
                                $@"{AdvManager.GetSlideAdvHtml(pageInfo.PublishmentSystemInfo, adAreaInfo, advInfo,
                                    adMaterialInfoList)}";
                        }
                    }
                }
            }

            // 如果是实体标签，返回empty
            if (contextInfo.IsCurlyBrace)
            {
                return string.Empty;
            }
            else
            {
                return parsedContent;
            }
        }
    }
}

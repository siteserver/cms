using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Advertisement;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

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

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var ie = node.Attributes?.GetEnumerator();

                var area = string.Empty;
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeArea))
                        {
                            area = attr.Value;
                        }
                    }
                }

                var adAreaInfo = DataProvider.AdAreaDao.GetAdAreaInfo(area, pageInfo.PublishmentSystemId);
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
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }
    }
}

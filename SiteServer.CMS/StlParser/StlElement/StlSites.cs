using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using System.Threading.Tasks;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "站点列表", Description = "通过 stl:sites 标签在模板中显示站点列表")]
    public class StlSites
    {
        public const string ElementName = "stl:sites";

        [StlAttribute(Title = "站点名称")]
        private const string SiteName = nameof(SiteName);

        [StlAttribute(Title = "站点文件夹")]
        private const string SiteDir = nameof(SiteDir);

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = ListInfo.GetListInfo(pageInfo, contextInfo, EContextType.Site);
            var siteName = listInfo.Others.Get(SiteName);
            var siteDir = listInfo.Others.Get(SiteDir);

            var dataSource = StlDataUtility.GetSitesDataSourceAsync(siteName, siteDir, listInfo.StartNum, listInfo.TotalNum, listInfo.Where, listInfo.Scope, listInfo.OrderByString).GetAwaiter().GetResult();

            if (contextInfo.IsStlEntity)
            {
                return ParseEntityAsync(dataSource).GetAwaiter().GetResult();
            }

            return ParseElement(pageInfo, contextInfo, listInfo, dataSource);
        }

        private static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, IDataReader dataSource)
        {
            var parsedContent = string.Empty;

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater
                {
                    ItemTemplate =
                        new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
                            listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
                            pageInfo, EContextType.Site, contextInfo)
                };

                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    rptContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    rptContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    rptContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
                }

                rptContents.DataSource = dataSource;
                rptContents.DataBind();

                if (rptContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                }
            }
            else
            {
                var pdlContents = new ParsedDataList();

                TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    pdlContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    pdlContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    pdlContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
                }

                pdlContents.DataSource = dataSource;
                pdlContents.DataBind();

                if (pdlContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                }
            }

            return parsedContent;
        }

        private static async Task<List<Site>> ParseEntityAsync(IDataReader dataSource)
        {
            var siteList = new List<Site>();

            while (dataSource.Read())
            {
                var siteId = dataSource.GetInt32(0);
                var site = await SiteManager.GetSiteAsync(siteId);

                if (site != null)
                {
                    siteList.Add(site);
                }
            }

            return siteList;
        }
    }
}

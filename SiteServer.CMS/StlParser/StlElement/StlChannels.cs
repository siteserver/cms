using System.Data;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "栏目列表", Description = "通过 stl:channels 标签在模板中显示栏目列表")]
    public class StlChannels
    {
        public const string ElementName = "stl:channels";

        public static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");			    //栏目索引
        public static readonly Attr ChannelName = new Attr("channelName", "栏目名称");				//栏目名称
        public static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别");						//上级栏目的级别
        public static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别");					    //从首页向下的栏目级别
        protected static readonly Attr IsTotal = new Attr("isTotal", "是否从所有栏目中选择");						//是否从所有栏目中选择（包括首页）
        protected static readonly Attr IsAllChildren = new Attr("isAllChildren", "是否显示所有级别的子栏目");			//是否显示所有级别的子栏目
        public static readonly Attr GroupChannel = new Attr("groupChannel", "指定显示的栏目组");		        //指定显示的栏目组
        public static readonly Attr GroupChannelNot = new Attr("groupChannelNot", "指定不显示的栏目组");	    //指定不显示的栏目组
        public static readonly Attr TotalNum = new Attr("totalNum", "显示栏目数目");					    //显示栏目数目
        public static readonly Attr StartNum = new Attr("startNum", "从第几条信息开始显示");					    //从第几条信息开始显示
        public static readonly Attr Order = new Attr("order", "排序");						    //排序
        public static readonly Attr IsImage = new Attr("isImage", "仅显示图片栏目");					    //仅显示图片栏目
        public static readonly Attr Where = new Attr("where", "获取栏目列表的条件判断");                           //获取栏目列表的条件判断
        public static readonly Attr CellPadding = new Attr("cellPadding", "填充");
        public static readonly Attr CellSpacing = new Attr("cellSpacing", "间距");
        public static readonly Attr Class = new Attr("class", "Css类");
        public static readonly Attr Columns = new Attr("columns", "列数");
        public static readonly Attr Direction = new Attr("direction", "方向");
        public static readonly Attr Height = new Attr("height", "整体高度");
        public static readonly Attr Width = new Attr("width", "整体宽度");
        public static readonly Attr Align = new Attr("align", "整体对齐");
        public static readonly Attr Layout = new Attr("layout", "指定列表布局方式");
        public static readonly Attr ItemHeight = new Attr("itemHeight", "项高度");
        public static readonly Attr ItemWidth = new Attr("itemWidth", "项宽度");
        public static readonly Attr ItemAlign = new Attr("itemAlign", "项水平对齐");
        public static readonly Attr ItemVerticalAlign = new Attr("itemVerticalAlign", "项垂直对齐");
        public static readonly Attr ItemClass = new Attr("itemClass", "项Css类");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            // 如果是实体标签则返回空
            if(contextInfo.IsStlEntity)
            {
                return string.Empty;
            }
            var listInfo = ListInfo.GetListInfoByXmlNode(pageInfo, contextInfo, EContextType.Channel);

            return ParseImpl(pageInfo, contextInfo, listInfo);
        }

        public static DataSet GetDataSource(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(IsTotal.Name));

            if (TranslateUtils.ToBool(listInfo.Others.Get(IsAllChildren.Name)))
            {
                listInfo.Scope = EScopeType.Descendant;
            }

            return StlDataUtility.GetChannelsDataSource(pageInfo.SiteId, channelId, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.Scope, isTotal, listInfo.Where);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            var dataSource = GetDataSource(pageInfo, contextInfo, listInfo);

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater();

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
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
                }

                rptContents.ItemTemplate = new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
                    listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
                    pageInfo, EContextType.Channel, contextInfo);

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

                //设置显示属性
                TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

                //设置列表模板
                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
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
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
                }

                pdlContents.DataSource = dataSource;
                pdlContents.DataKeyField = ChannelAttribute.Id;
                pdlContents.DataBind();

                if (pdlContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                }
            }

            return parsedContent;
        }
    }
}

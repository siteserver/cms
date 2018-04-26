using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Controls
{
    public class PagerParam
    {
        public Repeater ControlToPaginate { get; set; }
        public string TableName { get; set; }
        public string ReturnColumnNames { get; set; }
        public string WhereSqlString { get; set; }
        public string OrderSqlString { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }

    public class Pager : Control
    {
        public const string QueryNamePage = "page";

        public PagerParam Param { get; set; }

        public override void DataBind()
        {
            if (Param == null) return;

            Param.Page = Param.Page > 1 ? Param.Page : 1;

            base.DataBind();

            if (Param.ControlToPaginate == null) return;

            var sqlString = DataProvider.DatabaseDao.GetPageSqlString(Param.TableName, Param.ReturnColumnNames, Param.WhereSqlString, Param.OrderSqlString, (Param.Page - 1) * Param.PageSize, Param.PageSize);
            var dataSource = DataProvider.DatabaseDao.GetDataReader(WebConfigUtils.ConnectionString, sqlString);

            Param.ControlToPaginate.DataSource = dataSource;
            Param.ControlToPaginate.DataBind();
        }

        private string GetPageUrl(int page)
        {
            var queryString = new NameValueCollection(Page.Request.QueryString);
            if (page > 1)
            {
                queryString[QueryNamePage] = page.ToString();
            }
            else
            {
                queryString.Remove(QueryNamePage);
            }
            return PageUtils.AddQueryString(PageUtils.GetUrlWithoutQueryString(Page.Request.RawUrl), queryString);
        }

        protected override void Render(HtmlTextWriter output)
        {
            if (Param == null || Param.PageSize <= 0) return;

            var pageCount = (int)Math.Ceiling((double)Param.TotalCount / Param.PageSize);

            if (pageCount <= 1) return;

            var isFirst = Param.Page - 1 > 0;
            var isPrevious = Param.Page - 1 > 0;
            var isNext = Param.Page + 1 <= pageCount;
            var isLast = Param.Page + 1 <= pageCount;

            var dropdownBuilder = new StringBuilder();
            for (var i = 1; i <= pageCount; i++)
            {
                dropdownBuilder.Append($@"<a class=""dropdown-item {(i == Param.Page ? "active" : string.Empty)}"" href=""{GetPageUrl(i)}"">第 {i} 页</a>");
            }

            var top = -(pageCount * 28 + 10);
            if (pageCount > 10)
            {
                top = 0;
            }

            output.Write($@"
<div class=""clearfix"">
    <ul class=""pagination float-left"">
        <li class=""page-item {(isFirst ? string.Empty : "disabled")}"">
        <a class=""page-link"" href=""{(isFirst ? GetPageUrl(1) : "javascript:;")}"">首 页</a>
        </li>
        <li class=""page-item {(isPrevious ? string.Empty : "disabled")}"">
        <a class=""page-link"" href=""{(isPrevious ? GetPageUrl(Param.Page - 1) : "javascript:;")}"">上一页</a>
        </li>
        <li class=""page-item {(isNext ? string.Empty : "disabled")}"">
        <a class=""page-link"" href=""{(isNext ? GetPageUrl(Param.Page + 1) : "javascript:;")}"">下一页</a>
        </li>
        <li class=""page-item {(isLast ? string.Empty : "disabled")}"">
        <a class=""page-link"" href=""{(isLast ? GetPageUrl(pageCount) : "javascript:;")}"">末 页</a>
        </li>
    </ul>
    <span class=""btn-group dropup float-right"">
        <button id=""btnPager"" type=""button"" class=""btn btn-light text-secondary dropdown-toggle"">
            第 {Param.Page} 页（共 {pageCount} 页）
        </button>
        <div id=""dropdown-pager"" class=""dropdown-menu"" style=""position: absolute;left: 0px;top: {top}px;margin-bottom: 20px;"">
        {dropdownBuilder}
        </div>
    </span>
</div>
<script type=""text/javascript"">
$(document).ready(function () {{
  $(""body"").height($(window).height() - 40);
  $(""#btnPager"").click(function(event){{
    event.stopPropagation();
    $('#dropdown-pager').toggle();
  }});
  $(document).click(function() {{
    $('#dropdown-pager').hide();
  }});
}});
</script>
");
        }
    }
}

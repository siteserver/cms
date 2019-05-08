<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationCrossSiteTransChannels" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationCrossSiteTrans.aspx?siteId=<%=SiteId%>">跨站转发审核设置</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;">跨站转发栏目设置</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts text="在此设置各栏目中内容向其他站点跨站转发的选项，如果不指定跨站转发类型将使用站点的默认跨站转发设置" runat="server" />

    <div class="card-box">
      <table class="table-tree tablesaw m-t-20 table table-hover m-b-0 tablesaw-stack">
        <thead>
          <tr>
            <th>栏目名</th>
            <th>跨站转发设置</th>
            <th width="120"></th>
          </tr>
        </thead>
        <tbody>
          <asp:Repeater ID="RptContents" runat="server">
            <itemtemplate>
              <asp:Literal id="ltlHtml" runat="server" />
            </itemtemplate>
          </asp:Repeater>
        </tbody>
      </table>

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->
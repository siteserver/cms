<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannelsGroup" %>
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
        <li class="nav-item active">
          <a class="nav-link" href="pageNodeGroup.aspx?siteId=<%=SiteId%>">栏目组管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageContentGroup.aspx?siteId=<%=SiteId%>">内容组管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageContentTags.aspx?siteId=<%=SiteId%>">内容标签管理</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="m-t-0 header-title">
        <asp:Literal ID="LtlChannelGroupName" runat="server"></asp:Literal>
      </div>
      <p class="text-muted font-13 m-b-25"></p>

      <div class="panel panel-default m-t-10">
        <div class="panel-body p-0">
          <div class="table-responsive">
            <table class="table tablesaw table-hover m-0">
              <thead>
                <tr>
                  <th>栏目名</th>
                  <th width="300">栏目索引</th>
                  <th width="120">添加日期</th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlItemChannelName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlItemChannelIndex" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlItemAddDate" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </itemtemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <hr />

      <asp:Button class="btn" text="返 回" onclick="Return_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->
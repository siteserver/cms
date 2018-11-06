<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentTags" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
  <style type="text/css">
    .tag_popularity_1 {
      FONT-SIZE: 14px;
      font-weight: normal !important;
      COLOR: #104d6c;
    }

    .tag_popularity_2 {
      FONT-SIZE: 14px;
      FONT-WEIGHT: bold;
      COLOR: #104d6c;
    }

    .tag_popularity_3 {
      FONT-WEIGHT: bold;
      COLOR: #ff0f6f;
      font-size: 16px !important;
    }

    .tag_popularity_4 {
      FONT-WEIGHT: bold;
      font-size: 18px !important;
      COLOR: #ff0f6f !important
    }
  </style>
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageNodeGroup.aspx?siteId=<%=SiteId%>">栏目组管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageContentGroup.aspx?siteId=<%=SiteId%>">内容组管理</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;">内容标签管理</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="panel panel-default">
        <div class="panel-body p-0">
          <div class="table-responsive">
            <table id="contents" class="table tablesaw table-hover m-0">
              <thead>
                <tr>
                  <th>标签 </th>
                  <th width="100">使用次数 </th>
                  <th width="60"></th>
                  <th width="60"></th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlTagName" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlCount" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </itemtemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <ctrl:sqlPager id="SpContents" runat="server" class="table table-pager" />

      <hr />

      <asp:Button class="btn btn-primary" id="BtnAddTag" Text="添加标签" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->
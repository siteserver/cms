<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUtilityParameter" %>
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
          <a class="nav-link" href="pageUtilityCache.aspx">系统缓存</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageUtilityParameter.aspx">系统参数查看</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityEncrypt.aspx">加密字符串</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityJsMin.aspx">JS脚本压缩</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityDbLogDelete.aspx">清空数据库日志</a>
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
                <tr class="thead">
                  <th width="150" class="text-center text-nowrap">参数名称</th>
                  <th>值</th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td class="text-center text-nowrap">
                        <asp:Literal ID="ltlName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlValue" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </itemtemplate>
                </asp:Repeater>
              </tbody>
            </table>

          </div>
        </div>
      </div>

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->
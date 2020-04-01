<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUtilityCache" %>
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
          <a class="nav-link" href="pageUtilityCache.aspx">系统缓存</a>
        </li>
        <li class="nav-item">
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
      <p class="text-muted font-13 m-b-25">
        当前缓存：
        <asp:Literal id="LtlCount" runat="server" /> 个
        <asp:Button class="btn btn-success m-l-5" id="Submit" Text="清除缓存" OnClick="Submit_OnClick" runat="server" />
      </p>

      <div class="panel panel-default">
        <div class="panel-body p-0">
          <div class="table-responsive">
            <table id="contents" class="table tablesaw table-hover m-0">
              <thead>
                <tr class="thead">
                  <th>缓存键</th>
                  <th>缓存值</th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater id="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td style="word-break: break-all;">
                        <span>
                          <asp:Literal id="ltlKey" runat="server"></asp:Literal>
                        </span>
                      </td>
                      <td style="word-break: break-all;">
                        <span>
                          <asp:Literal id="ltlValue" runat="server"></asp:Literal>
                        </span>
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

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->